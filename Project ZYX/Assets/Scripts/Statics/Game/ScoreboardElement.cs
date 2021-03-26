using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardElement : MonoBehaviour
{
    // VARIABLES
    public string Name 
    { 
        get {return Owner? Owner.Name : "NULL PLAYER"; }
    }
    public float  Score
    {
        get { return Owner? Owner.Score : 42069; }
    }
    public Color  Color 
    {
        get { return Owner? Owner.Color : Color.black; }
    }

    // REFERENCES
    [HideInInspector] public Tank  Owner       = null;
    [HideInInspector] public Image ColorImage  = null;
    [HideInInspector] public Image BorderImage = null;

    [HideInInspector] public TextMeshProUGUI NameText  = null;
    [HideInInspector] public TextMeshProUGUI ScoreText = null;



//  INIT LOGIC
    private void Awake()
    {
        // 1. ADD TO LIST
        Game.ScoreElements.Add(this);

        // 2. GER REFS
        {
            var i = GetComponentsInChildren<Image>(true);
            BorderImage = i[0];
            ColorImage  = i[1];
        }
        {
            var i = GetComponentsInChildren<TextMeshProUGUI>(true);
            NameText  = i[0];
            ScoreText = i[1];
        }   
    }
    private void OnDestroy()
    {
        // 1. REMOVE FROM LIST
        Game.ScoreElements.Remove(this);
    }
    public void Init(Tank Player)
    {
        Owner = Player;
        UpdateElement();
    }

//  UPDATE LOGIC
    public void UpdateElement()
    {
        SetName(Name);
        SetScore(Score);
        SetColor(Color);
    }
    public static void UpdateScores()
    {
        foreach(var i in Game.ScoreElements)
        i.UpdateElement();
    }
    public static int SortFunction(ScoreboardElement a, ScoreboardElement b)
    {
        if      (a.Score > b.Score)  return -1;
        else if (a.Score == b.Score) return 0;
                                     return 1;
    }
    public static void SortElements()
    {
        Game.ScoreElements.Sort(SortFunction);
        for (int i = 0; i < Game.ScoreElements.Count; i++)
        {
            Game.ScoreElements[i].transform.SetAsLastSibling();
        }
    }

//  SET LOGIC
    private void SetName(string Name)
    {
        NameText.SetText(Name);
    }
    private void SetScore(float Score)
    {
        ScoreText.SetText($"{Score}");/*$"{0:0}", Score*/
    }
    private void SetColor(Color Color)
    {
        ColorImage.color = Color;
    }
}
