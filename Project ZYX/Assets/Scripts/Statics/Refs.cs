using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Reference Asset", menuName = "ZYX Assets/Game/Reference Asset")]
public class Refs : ScriptableObject
{
    private static Refs _i = null;
    public static Refs i
    {
        get 
        {
            if (_i) return _i;
            return _i = Resources.Load<Refs>("Reference Asset");
        }
    }
    

    
    [Header("Prefabs")]
    public GameObject ScoreboardElement = null;
}
