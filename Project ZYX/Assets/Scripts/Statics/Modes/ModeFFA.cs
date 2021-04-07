using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Free For All", menuName = "ZYX Assets/Gamemodes/Free For All")]
public class ModeFFA : Gamemode
{
    [Space(10, order = 0)]
    [Header("Free For All", order = 1)]
    [SerializeField] private float PointsPerKill = 2f;
    
//  LOGIC
    protected override void OnTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        base.OnTankKill(Tank, DamageInfo);
        
        // 1. GIVE SCORE TO KILLER
        Tank Dealer = (Tank) DamageInfo.Dealer;
        Dealer.Score += PointsPerKill;

        // 2. IF ONLY ONE ALIVE, SPAWN ALL
        if (Game.AliveList.Count == 1)
        Game.SpawnTanks(4f);

        Debug.Log($"[{Name}]: \"{DamageInfo.Reciever}\" was killed by \"{DamageInfo.Dealer}\"");
    }

//  LIFE CYCLE
    protected override IEnumerator BeginPlay()
    {
        Game.ResetScore();
        Game.AddCountdown(5f);
        Game.SpawnTanks(5f);
        
        yield return null;
    }
    protected override IEnumerator StopPlay()
    {
        Game.MatchCleanup();
        Game.SetActiveFocus(Game.EFocus.Lobby);

        yield return null;
    }

}   
