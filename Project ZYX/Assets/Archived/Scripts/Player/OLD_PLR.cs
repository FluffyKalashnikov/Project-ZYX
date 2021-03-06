using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OLD_PLR : MonoBehaviour
{
    [Header("Variables")]
    public static List<OLD_PLR> PlayerList  =  new List<OLD_PLR>();
    public        OLD_Tank      tank        =  null;
    public        float         health      =  5f;
    public        Color         color       =  new Color();


    [Header("REFERENCES")]
                     public  PlayerInput playerInput      = null;
                     public  PLR_UI      UI               = null;
    [SerializeField] private MeshFilter  bodyMeshFilter   = null;
    [SerializeField] private MeshFilter  turretMeshFilter = null;




    public void LoadTank()
    {
        bodyMeshFilter  .mesh = tank.bodyMesh;
        turretMeshFilter.mesh = tank.turretMesh;
        health                = tank.health;
    }
}
