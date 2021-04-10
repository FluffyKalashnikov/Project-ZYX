using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Widget : MonoBehaviour
{
    [Header("REFERENCES")]
    public  WidgetSwitcher Parent         = null;
    public  GameObject     DefaultElement = null;
    public  GameObject     CurrentElement = null;
    public  Action         OnEnabled      = null;
    public  Action         OnDisabled     = null;
    
    private Animator       Animator       = null;

    private static IEnumerator  IE_SetSelected = null;
    private static List<Widget> OverlayWidgets = new List<Widget>();

    private void Awake()
    {
        // 1. GET REFERENCES
        Animator = GetComponent<Animator>();
        Parent   = transform.parent.GetComponent<WidgetSwitcher>();
        // 2. INIT
        CurrentElement = DefaultElement;
    }

    // WIDGET FUNCTIONS
    public void Enable()
    {
        if (IsActive()) return;
        gameObject.SetActive(true);
        OnEnabled?.Invoke(); 
        ResetSelection(() => { if (Animator) Animator.Play("OnSelected"); });
    }
    public void Disable()
    {
        if (!IsActive()) return;
        gameObject.SetActive(false);
        OnDisabled?.Invoke();
    }
    public bool ShownFirst()
    {
        return Parent 
         ? transform.GetSiblingIndex() == Parent.Widgets.Count-1 
         : false;
    }

    // WIDGET SELECTION
    public void SetSelectedElement(GameObject Element,  Action OnComplete)
    {
        if (Element == null) return;

        // 1. STOP ACTIVE SETTERS
        if (IE_SetSelected != null)
        Game.Instance.StopCoroutine (IE_SetSelected);
        Game.Instance.StartCoroutine(IE_SetSelected = Logic());

        // 2. SET ELEMENT WHEN EXISTS
        IEnumerator Logic()
        {
            yield return new WaitUntil(() => gameObject.activeInHierarchy);

            EventSystem.current.SetSelectedGameObject(Element);
            OnComplete?.Invoke();
        }
    }
    public void ResetSelection   (Action OnComplete)
    {
        SetSelectedElement(DefaultElement, OnComplete);
    }
    public void ContinueSelection(Action OnComplete)
    {
        SetSelectedElement(CurrentElement, OnComplete);
    }

    // WIDGET ORDER
    public static void AddWidget   (Widget Widget)
    {
        // 1. ENABLE
        Widget.ShowFirst();
        Widget.Enable();

        // 2. ADD TO LIST
        if (!OverlayWidgets.Contains(Widget))
        OverlayWidgets.Add(Widget);
    }
    public static void RemoveWidget(Widget Widget)
    {
        // 1. DISABLE
        Widget.Disable();

        // 2. REMOVE FROM LIST
        if (OverlayWidgets.Contains(Widget))
        OverlayWidgets.Remove(Widget);
    }
    public static void RemoveOverlays()
    {
        if (OverlayWidgets.Count == 0) return;
        for (int i = OverlayWidgets.Count-1; i>=0; i--)
        RemoveWidget(OverlayWidgets[i]);
    }

    public void SetIndex(int Index)
    {
        Parent?.SetWidgetIndex(this, Index);
    }
    public void ShowFirst()
    {
        Parent?.ShowWidgetFirst(this);
    }
    public void ShowLast()
    {
        Parent?.ShowWidgetLast(this);
    }
    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
}