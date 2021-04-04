using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    #region Stats
    [Header("Timer text component")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer values")]
    public int minutesLeft;
    public int secondsLeft;

    private bool timerActivator = false;
    private bool takingAway = false;
    #endregion

    public void Start()
    {
        #region TIME SET
        //E.G "10:10"
        if (minutesLeft >= 10 && secondsLeft >= 10)
        {
            timerText.SetText($"{minutesLeft} : {secondsLeft}");
        }

        //E.G "09:10"
        else if (minutesLeft < 10 && secondsLeft >= 10)
        {
            timerText.SetText($"0{minutesLeft} : {secondsLeft}");
        }

        //E.G "10:09"
        else if (minutesLeft >= 10 && secondsLeft < 10)
        {
            timerText.SetText($"{minutesLeft} : 0{secondsLeft}");
        }

        //E.G "09:09"
        else if (minutesLeft < 10 && secondsLeft < 10)
        {
            timerText.SetText($"0{minutesLeft} : 0{secondsLeft}");
        }

        #endregion

        //ACTIVATE TIMER
        timerActivator = true;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        if (timerActivator == true)
        {
            //E.G "00:10" or "01:00" etc
            if (takingAway == false && secondsLeft > 0 || takingAway == false && minutesLeft > 0)
            {
                StartCoroutine(TimerTake());
            }
            //E.G "00:00"
            else if (takingAway == false && secondsLeft == 0 && minutesLeft == 0)
            {
                StartCoroutine(TimerEnd());
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //E.G "01:00" -> "00:59"
        if (minutesLeft > 0 && secondsLeft == 0)
        {
            minutesLeft -= 1;
            secondsLeft += 59;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region TIME SET
        //E.G "10:10"
        if (minutesLeft >= 10 && secondsLeft >= 10)
        {
            timerText.SetText($"{minutesLeft} : {secondsLeft}");
        }

        //E.G "09:10"
        else if (minutesLeft < 10 && secondsLeft >= 10)
        {
            timerText.SetText($"0{minutesLeft} : {secondsLeft}");
        }

        //E.G "10:09"
        else if (minutesLeft >= 10 && secondsLeft < 10)
        {
            timerText.SetText($"{minutesLeft} : 0{secondsLeft}");
        }

        //E.G "09:09"
        else if (minutesLeft < 10 && secondsLeft < 10)
        {
            timerText.SetText($"0{minutesLeft} : 0{secondsLeft}");
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Set bool to false
        takingAway = false;
    }

    IEnumerator TimerEnd()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Game Over!");
        //END MATCH METHOD
    }
}
