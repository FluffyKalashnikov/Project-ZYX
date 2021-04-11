using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Free For All", menuName = "ZYX Assets/Gamemodes/Free For All")]
public class ModeFFA : Gamemode
{
    [Space(10, order = 0)]
    [Header("Free For All", order = 1)]
    [SerializeField] private float PointsToWin = 10f;
    [SerializeField] private float PointsPerKill = 2f;
    
//  LOGIC
    protected override IEnumerator OnTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        // 1. GIVE SCORE TO KILLER
        Tank Dealer = (Tank) DamageInfo.Dealer;
        Dealer.Score += PointsPerKill;

        // 2. CHECK IF WON
        if (Dealer.Score >= PointsToWin)
        StopGame();

        // 3. IF ONLY ONE ALIVE, SPAWN ALL
        if (Game.AliveList.Count <= 1)
        Game.SpawnTanks(4f);

        
        yield return null;
    }

//  LIFE CYCLE
    protected override IEnumerator BeginPlay()
    {
        Game.ResetScore();
        Game.AddCountdown(5f);
        Game.SpawnTanks(5f);

        yield return new WaitForSeconds(MatchLength);

        StopGame();
    }
    protected override IEnumerator StopPlay()
    {
        Game.MatchCleanup();

        yield return new WaitForSeconds(2f);

        Game.SetActiveState(Game.EState.WinScreen);
    }

}   
