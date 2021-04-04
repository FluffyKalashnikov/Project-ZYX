using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Capture The Flag", menuName = "ZYX Assets/Gamemodes/Capture The Flag")]
public class ModeCTF : Gamemode
{
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
