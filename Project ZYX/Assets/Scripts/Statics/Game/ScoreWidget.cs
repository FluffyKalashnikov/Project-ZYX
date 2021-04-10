using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreWidget : MonoBehaviour
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
    [HideInInspector] 
    public Tank  Owner       = null;
    public Image ColorImage  = null;
    public Image BorderImage = null;

    public TextMeshProUGUI NameText  = null;
    public TextMeshProUGUI ScoreText = null;



//  INIT LOGIC
    private void Awake()
    {
        // 1. ADD TO LIST
        Game.ScoreElements.Add(this);
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

//  SET LOGIC
    private void SetName(string Name)
    {
        NameText.SetText(Name);
    }
    private void SetScore(float Score)
    {
        ScoreText.SetText($"{Score}");
    }
    private void SetColor(Color Color)
    {
        ColorImage.color = Color;
    }
}
