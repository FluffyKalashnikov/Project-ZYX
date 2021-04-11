using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchContext : MonoBehaviour
{
    public  static MatchContext I 
    { 
        get 
        { 
            if (_i) return _i;
            var a = new GameObject("MatchContext", typeof(MatchContext));
            a.hideFlags = HideFlags.HideInHierarchy;
            return _i = a.GetComponent<MatchContext>();
        }
        set{_i = value;}
    }
    private static MatchContext _i = null;

    public static IEnumerator IE_Timer     = null;
    public static IEnumerator IE_Countdown = null;


    public static void Add(IEnumerator Enumerator)
    {
        if (Game.IsPlaying())
        I.StartCoroutine(Enumerator);
    }
    public static void Remove(IEnumerator Enumerator)
    {
        I.StopCoroutine(Enumerator);
    }
    public static void Stop()
    {
        I.StopAllCoroutines();
    }
}
