using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Widget : MonoBehaviour
{
    [Header("REFERENCES")]
    public  GameObject  DefaultElement = null;
    public  GameObject  CurrentElement = null;
    public  Action      OnSelected     = null;

    private Animator    Animator       = null;

    private static IEnumerator  IE_SetSelected = null;
    private static List<Widget> OverlayWidgets = new List<Widget>();

    private void Awake()
    {
        // 1. GET REFERENCES
        Animator = GetComponent<Animator>();
        // 2. INIT
        CurrentElement = DefaultElement;
    }

    // WIDGET FUNCTIONS
    public void Enable()
    {
        gameObject.SetActive(true);
        SetSelectedElement(DefaultElement, null);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public bool ShownFirst()
    {
        return transform.GetSiblingIndex() == 0;
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
            Animator?.Play("OnSelected");
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
    public static void RemoveWidget(Widget widget)
    {
        // 1. DISABLE
        widget.ShowLast();
        widget.Disable();

        // 2. REMOVE FROM LIST
        if (OverlayWidgets.Contains(widget))
        OverlayWidgets.Remove(widget);
    }
    public static void RemoveOverlays()
    {
        for (int i = OverlayWidgets.Count-1; i>=0; i--)
        RemoveWidget(OverlayWidgets[i]);
    }

    public void SetOrder(int order)
    {
        Transform root = Game.Instance.transform.Find("UI");
        transform.SetSiblingIndex(order);
    }
    public void ShowFirst()
    {
        Transform root = Game.Instance.transform.Find("UI");
        transform.SetAsFirstSibling();
    }
    public void ShowLast()
    {
        Transform root = Game.Instance.transform.Find("UI");
        transform.SetAsFirstSibling();
    }
}