using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Capture The Flag", menuName = "ZYX Assets/Gamemodes/Capture The Flag")]
public class ModeCTF : Gamemode
{
    protected override void BeginPlay()
    {
        base.BeginPlay();

        Game.SpawnTanks();
    }
    protected override void StopPlay()
    {
        base.StopPlay();
        
        Game.MatchCleanup();
    }
}
