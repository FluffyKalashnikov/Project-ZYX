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
    protected override void BeginPlay()
    {
        base.BeginPlay();
        Game.ResetScore();
        Game.SpawnTanks();
    }
    protected override void StopPlay()
    {
        base.StopPlay();
        Game.MatchCleanup();
        Game.SetActiveFocus(Game.EFocus.Lobby);
    }
}   
