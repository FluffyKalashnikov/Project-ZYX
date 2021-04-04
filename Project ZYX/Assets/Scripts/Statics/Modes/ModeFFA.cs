using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Free For All", menuName = "ZYX Assets/Gamemodes/Free For All")]
public class ModeFFA : Gamemode
{
    float PointsPerKill = 2f;

//  LOGIC
    public override void OnTankKill(Tank Tank, DamageInfo DamageInfo)
    {
        base.OnTankKill(Tank, DamageInfo);
        
        Tank Dealer = (Tank) DamageInfo.Dealer;
        Dealer.Score++;
        Tank.Spawn(3f);
        Debug.Log($"[{Name}]: \"{DamageInfo.Reciever}\" was killed by \"{DamageInfo.Dealer}\"");
    }
    public override void OnTankSpawn(Tank Tank)
    {
        base.OnTankSpawn(Tank);
    }

//  EVENTS
    public override void BeginPlay()
    {
        base.BeginPlay();

        Game.SpawnTanks();
    }
    public override void StopPlay()
    {
        base.StopPlay();
        
        Game.MatchCleanup();
    }
}
