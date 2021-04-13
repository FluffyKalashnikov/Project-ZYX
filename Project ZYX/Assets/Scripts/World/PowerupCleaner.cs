using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupCleaner : MonoBehaviour
{
    private GameObject[] powerupsHP;
    private GameObject[] powerupsSeamine;
    private GameObject[] powerupsEMP;
    private GameObject[] powerupsQuickCharge;
    private GameObject[] powerupsMultishot;
    private GameObject[] powerupsSpeedBoost;
    private void Awake()
    {
        Game.OnNewMatch += () => PowerupDeleter();
    }
    private void PowerupDeleter()
    {
        #region HP
        powerupsHP = GameObject.FindGameObjectsWithTag("PU-HP");

        for (int i = 0; i < powerupsHP.Length; i++)
        {
            Destroy(powerupsHP[i].gameObject);
        }
        #endregion

        #region Seamine
        powerupsSeamine = GameObject.FindGameObjectsWithTag("PU-Seamine");

        for (int i = 0; i < powerupsSeamine.Length; i++)
        {
            Destroy(powerupsSeamine[i].gameObject);
        }
        #endregion

        #region EMP (BouncyBullets)
        powerupsEMP = GameObject.FindGameObjectsWithTag("PU-EMP");

        for (int i = 0; i < powerupsEMP.Length; i++)
        {
            Destroy(powerupsEMP[i].gameObject);
        }
        #endregion

        #region QuickCharge
        powerupsQuickCharge = GameObject.FindGameObjectsWithTag("PU-QuickCharge");

        for (int i = 0; i < powerupsQuickCharge.Length; i++)
        {
            Destroy(powerupsQuickCharge[i].gameObject);
        }
        #endregion

        #region Multishot
        powerupsMultishot = GameObject.FindGameObjectsWithTag("PU-Multishot");

        for (int i = 0; i < powerupsMultishot.Length; i++)
        {
            Destroy(powerupsMultishot[i].gameObject);
        }
        #endregion

        #region SpeedBoost
        powerupsSpeedBoost = GameObject.FindGameObjectsWithTag("PU-SpeedBoost");

        for (int i = 0; i < powerupsSpeedBoost.Length; i++)
        {
            Destroy(powerupsSpeedBoost[i].gameObject);
        }
        #endregion
    }
}
