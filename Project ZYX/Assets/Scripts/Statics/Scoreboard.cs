using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class Scoreboard : MonoBehaviour
{
    [HideInInspector]
    public List<ScoreWidget> Elements = new List<ScoreWidget>(0);


//  INITIALIZATION
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var a = transform.GetChild(i).GetComponent<ScoreWidget>();
            if (a) Elements.Add(a);
        }
    }

//  PARENTING
    public void AddWidget(ScoreWidget Widget)
    {
        // 1. REPARENT AND ADD TO LIST
        Widget.transform.SetParent(this.transform);
        Elements.Add(Widget);
        Widget.UpdateElement();
    }
    public void RemoveWidget(ScoreWidget Widget)
    {
        // 1. FIND AND DESTROY WIDGET
        var i = Elements.Find(w => w == Widget);
        if (i) 
        {
            Elements.Remove(i);
            Destroy(i.gameObject);
        }
    }
    public void UpdateWidget(ScoreWidget Widget)
    {
        // 1. FIND AND UPDATE ELEMENT
        var i = Elements.Find(w => w == Widget);
        if (i) i.UpdateElement();
    }
    public void UpdateWidgets()
    {
        // 1. UPDATE EVERY ELEMENT
        foreach (var i in Elements)
        i?.UpdateElement();
    }
    public void Reset()
    {
        // 1. DESTROYS ALL WIDGETS
        for (int i = Elements.Count-1; i >= 0; i--)
        Destroy(Elements[i].gameObject);
        Elements.Clear(); 
    }

    public void Validate()
    {
        for (int i = Elements.Count-1; i >= 0; i--)
        if (!Elements[i].Owner)
        {
            Destroy(Elements[i].gameObject);
            Elements.Remove(Elements[i]);
        }
    }

//  SORTING
    public void Sort()
    {
        // 1. SORT ELELMENTS
        Elements.Sort(SortAlgorithm);

        // 2. ORDER ELEMENTS
        foreach (var Element in Elements)
        Element.transform.SetAsFirstSibling();
    }
    private static int SortAlgorithm(ScoreWidget a, ScoreWidget b)
    {
        if      (a.Score == b.Score) return 0;
        else if (a.Score >  b.Score) return 1;
                                     return-1;
    }
}
