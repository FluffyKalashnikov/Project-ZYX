using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public Vector3    Position
    {
        get { return transform.position; }
    }
    public Quaternion Rotation
    {
        get { return transform.rotation; }
    }
    public float      Distance
    {
        get 
        { 
            float ClosestDistance = Mathf.Infinity;
            foreach (var Player in Game.AliveList)
            {
                // 1. GET CURRENT DISTANCE
                float CurrentDistance = Vector3.Distance
                (
                    this  .Position,
                    Player.Position
                );
                // 2. STORE CLOSEST
                ClosestDistance = Mathf.Min
                (
                    ClosestDistance, 
                    CurrentDistance
                );
            }
            return ClosestDistance; 
        }
    }
    
    public static List<Spawnpoint> List = new List<Spawnpoint>();
    

    private void Awake()
    {
        List.Add(this);
        GetComponentInChildren<MeshRenderer>().forceRenderingOff = true;
    }
    private void OnDestroy()
    {
        List.Remove(this);
    }
}
