using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class Widget : MonoBehaviour
{
    [Header("REFERENCES")]
    public Canvas      canvas         = null;
    public GameObject  defaultElement = null;
    public GameObject  currentElement = null;
    
    private static IEnumerator  IE_SetSelected = null;
    private static List<Widget> OverlayWidgets = new List<Widget>();

    private void Awake()
    {
        // 1. GET REFERENCES
        canvas         = GetComponent<Canvas>(); 
        currentElement = defaultElement;
    }




    // WIDGET FUNCTIONS
    public void Enable()
    {
        canvas.enabled = true;
        SetSelectedElement(defaultElement);
    }
    public void Disable()
    {
        canvas.enabled = false;
    }
    public bool ShownFirst()
    {
        return transform.GetSiblingIndex() == 0;
    }

    // WIDGET SELECTION
    public void SetSelectedElement(GameObject element, bool ignoreNull = true)
    {
        if (ignoreNull && element == null) return;

        // 1. STOP ACTIVE SETTERS
        if (IE_SetSelected != null)
        Game.Instance.StopCoroutine (IE_SetSelected);
        Game.Instance.StartCoroutine(IE_SetSelected = Logic());

        // 2. SET ELEMENT WHEN EXISTS
        IEnumerator Logic()
        {
            yield return new WaitUntil(() => canvas.enabled);

            EventSystem.current.SetSelectedGameObject(defaultElement);
        }
    }
    public static void SetSelectedWidget(Widget widget, bool ignoreNull = false)
    {
        if (widget == null && !ignoreNull)
        {
            Debug.LogError("Null Widget detected!");
        }
        
        // 1. STORE ALL WIDGETS
        Widget[] widgets = FindObjectsOfType<Widget>();

        // 2. DEACTIVATE ALL WIDGETS
        foreach (var i in widgets) i.Disable();
        
        // 3. ENABLE SELECTED WIDGET
        widget?.Enable();
    }
    
    // WIDGET ORDER
    public static void AddWidget(Widget widget)
    {
        // 1. ENABLE
        widget.ShowFirst();
        widget.Enable();

        // 2. ADD TO LIST
        if (!OverlayWidgets.Contains(widget))
        OverlayWidgets.Add(widget);
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
        {
            RemoveWidget(OverlayWidgets[i]);
        }
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