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
    
    private IEnumerator IE_SetSelected = null;

    private void Awake()
    {
        // 1. GET REFERENCES
        canvas     = GetComponent<Canvas>(); 
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
    public static Widget Find(string name)
    {
        // 1. TRY FIND WIDGET
        foreach (var i in FindObjectsOfType<Widget>())
        {
            // IF NAME FOUND, RETURN
            if (i.gameObject.name == name)
            return i;
        }
        // 2. IF NO WIDGET FOUND
        Debug.LogError($"No Widget \"{name}\" found.");
        return null;
    }

    // WIDGET SELECTION
    public void SetSelected(GameObject element, bool ignoreNull = true)
    {
        if (ignoreNull && element == null) return;

        // 1. STOP ACTIVE SETTERS
        if (IE_SetSelected != null)
        GameUI.Instance.StopCoroutine (IE_SetSelected);
        GameUI.Instance.StartCoroutine(IE_SetSelected = Logic());

        // 2. SET ELEMENT WHEN EXISTS
        IEnumerator Logic()
        {
            yield return new WaitUntil(() => defaultElement.activeInHierarchy);

            EventSystem.current.SetSelectedGameObject(defaultElement);
        }
    }
    public static void SetSelected(Widget widget, bool ignoreNull = true)
    {
        if (ignoreNull && widget == null) return;

        // 1. STORE ALL WIDGETS
        Widget[] widgets = FindObjectsOfType<Widget>();

        // 2. DEACTIVATE ALL WIDGETS
        foreach (var i in widgets) i.Disable();
        
        // 3. ENABLE SELECTED WIDGET
        widget.Enable();
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
