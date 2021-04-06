using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetSwitcher : MonoBehaviour
{
    public  int          ActiveIndex
    {
        get { return m_ActiveIndex; }
        set 
        {
            // 1. CLAMP VALUE
            if (Widgets.Count == 0) return;
            value = Mathf.Clamp(value, 0, Widgets.Count);

            // 2. DISABLE/ENABLE
            Widgets[m_ActiveIndex]        ?.Disable();
            Widgets[m_ActiveIndex = value]?.Enable();
        }
    }
    public  Widget       ActiveWidget
    {
        get { return Widgets[ActiveIndex]; }
        set 
        {
            // 1. FIND INDEX
            int index = -1; 
            for (int i = 0; i < Widgets.Count; i++)
            {
                if (Widgets[i] == value)
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                Debug.Log($"[WIDGET SWITCH]: \"{value}\" Widget not found!", this);
                return;
            }

            // 2. SET WIDGET
            ActiveIndex = index;
        }
    }
    public  List<Widget> Widgets
    {
        get { return m_Widgets; }
        set 
        {
            m_Widgets = value;
        }
    }

    private int          m_ActiveIndex = 0;
    private List<Widget> m_Widgets     = new List<Widget>(0);


    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        Widgets.Add(transform.GetChild(i).GetComponent<Widget>());
        ActiveIndex = ActiveIndex;
    }
 
    public void SetIndex(int Index)
    {
        ActiveIndex = Index;
    }
    public void SetWidget(Widget Widget)
    {
        ActiveWidget = Widget;
    }
}
