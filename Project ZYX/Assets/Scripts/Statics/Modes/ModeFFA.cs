using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Free For All", menuName = "ZYX Assets/Gamemodes/Free For All")]
public class ModeFFA : Gamemode
{
    [Space(10, order = 0)]
    [Header("Free For All", order = 1)]
    [SerializeField] private float PointsToWin   = 10f;
    [SerializeField] private float PointsPerKill = 2f;

    private TextMeshProUGUI TimerText = null;
    
//  LOGIC
    protected override IEnumerator OnTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        // 1. GIVE SCORE TO KILLER
        Tank Dealer = (Tank) DamageInfo.Dealer;
        if (Dealer != null)
        {
            Dealer.Score += PointsPerKill;
            if (Dealer.Score >= PointsToWin)
            StopGame();
        }
        
        // 2. IF ONLY ONE ALIVE, SPAWN ALL
        if (Game.AliveList.Count <= 1)
        {
            yield return new WaitForSeconds(4f);
            Game.SpawnTanks();
        }
        yield return null;
    }

//  LIFE CYCLE
    protected override IEnumerator BeginPlay()
    {
        Game.ResetScore();
        Game.RespawnTanks();
        Game.LookOnly();
        Game.AddCountdown(5f);
        yield return new WaitForSeconds(5f);
        this.StartTimer();
        Game.OpenHUD();
        Game.EnableInput();
        yield return new WaitForSeconds(MatchLength-5f);
        Game.AddCountdown(5f);
        yield return new WaitForSeconds(5f);
        StopGame();
    }
    protected override IEnumerator StopPlay()
    {
        Game.LookOnly();
        Game.CloseHUD();
        yield return new WaitForSeconds(3f);
        Game.DisableInput();
        Game.SetActiveState(Game.EState.WinScreen);
    }

//  WIDGET LOGIC
    protected override void InitWidget(GameObject Widget)
    {
        TimerText = Widget.GetComponentInChildren<TextMeshProUGUI>(true);
        TimerText.SetText(Game.SecToTimer(MatchLength));
    }
    private void StartTimer()
    {
        float Time = MatchLength;
        MatchContext.Add(Timer());
        IEnumerator Timer()
        {
            while (Time > 0)
            {
                yield return new WaitForSeconds(1f);
                TimerText.SetText(Game.SecToTimer(--Time));
            }
        }
    }
}   
