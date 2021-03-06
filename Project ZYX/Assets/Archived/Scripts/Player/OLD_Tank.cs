using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Tank", menuName = "Create Asset/Tank")]
public class OLD_Tank : ScriptableObject
{
    [Header("Tank Essentials")]
    public new string  name                  = "Tank name";
    public     Mesh    bodyMesh              = null;
    public     Mesh    turretMesh            = null;
    [Space(10, order = 0)]
    [Header("Tank Stats", order = 1)]
    public     float   health                =  5f;
    public     float   maxSpeed              =  7f;
    public     float   acceleration          =  11f; 
    public     float   deceleration          =  3f;
    public     float   boostedDeceleration   =  14f;
}
