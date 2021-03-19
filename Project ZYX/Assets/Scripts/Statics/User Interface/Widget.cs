using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class Widget : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Canvas      canvas         = null;
    [SerializeField] private GameObject  defaultElement = null;
    [SerializeField] private GameObject  currentElement = null;
    
    private static IEnumerator IE_SetSelected = null;

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
        SetSelected(defaultElement);
    }
    public void Disable()
    {
        canvas.enabled = false;
    }

    // WIDGET SELECTION
    public void SetSelected(GameObject element, bool ignoreNull = true)
    {
        if (ignoreNull && element == null) return;

        // 1. STOP ACTIVE SETTERS
        if (IE_SetSelected != null)
        Game.Instance.StopCoroutine (IE_SetSelected);
        Game.Instance.StartCoroutine(IE_SetSelected = Logic());

        // 2. SET ELEMENT WHEN EXISTS
        IEnumerator Logic()
        {
            yield return new WaitUntil(() => defaultElement.activeInHierarchy && Tank.EventSystem);

            Tank.EventSystem.SetSelectedGameObject(defaultElement);
        }
    }
    public static void SetSelected(Widget widget, bool ignoreNull = false)
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
        widget.Enable();
        widget.ShowFirst();
    }
    public static void RemoveWidget(Widget widget)
    {
        widget.Disable();
        widget.ShowLast();
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