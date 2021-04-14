using System.Collections;
using UnityEngine;
using Cinemachine;
public class Cam : MonoBehaviour
{
    public static Camera Camera = null;
    public static CinemachineTargetGroup Target = null;
    public static CinemachineVirtualCamera MatchCamera = null;
    public static CinemachineVirtualCamera LobbyCamera = null;
    public static Cam Instance = null;


    private IEnumerator IE_Shake   = null;
    private static float constFreq = 0f;
    private static float constAmpl = 0f;
    private CinemachineBasicMultiChannelPerlin ShakeComp = null;


    private void Awake()
    {
        Instance = this;

        Camera    = GetComponentInChildren<Camera>();
        Target    = GetComponentInChildren<CinemachineTargetGroup>();
        ShakeComp = GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        MatchCamera = transform.Find("VC MatchMode").GetComponent<CinemachineVirtualCamera>();
        LobbyCamera = transform.Find("VC LobbyMode").GetComponent<CinemachineVirtualCamera>();

    #if UNITY_EDITOR
        if (!Camera)      Debug.LogError("No Camera found!",          this);
        if (!Target)      Debug.LogError("No Target found!",          this);
        if (!ShakeComp)   Debug.LogError("No Shake Component found!", this);
        if (!MatchCamera) Debug.LogError("No Match Camera found!",    this);
        if (!LobbyCamera) Debug.LogError("No Lobby Camera found!",    this);
    #endif
    }
    
//  SHAKE FUNCTIONS
    public static void Shake(float amplitude, float frequency, float time = 1f)
    {
        if (Instance.IE_Shake != null)
        Instance.StopCoroutine (Instance.IE_Shake);
        Instance.StartCoroutine(Instance.IE_Shake = Shaker());

        IEnumerator Shaker()
        {
            // SETS INITIAL VALUES
            float initAmp  = Instance.ShakeComp.m_AmplitudeGain = amplitude;
            float initFreq = Instance.ShakeComp.m_FrequencyGain = frequency;

            while (amplitude != 0f && frequency != 0f)
            {
                // UPDATES VALUES
                amplitude = Mathf.MoveTowards(amplitude, 0f, (initAmp  * Time.unscaledDeltaTime)/time);
                frequency = Mathf.MoveTowards(frequency, 0f, (initFreq * Time.unscaledDeltaTime)/time);

                // UPDATES SHAKE
                if (constAmpl <= amplitude)
                Instance.ShakeComp.m_AmplitudeGain = amplitude;
                if (constFreq <= frequency)
                Instance.ShakeComp.m_FrequencyGain = frequency;

                yield return null;
            }
        }
    }
    public static void StartShake(float amplitude, float frequency)
    {   
        if (Instance.ShakeComp.m_AmplitudeGain <= constAmpl)
        Instance.ShakeComp.m_AmplitudeGain = constAmpl = amplitude;
        if (Instance.ShakeComp.m_FrequencyGain <= constFreq)
        Instance.ShakeComp.m_FrequencyGain = constFreq = frequency;
    }
    public static void StopShake()
    {
        if (Instance.IE_Shake != null)
        Instance.StopCoroutine(Instance.IE_Shake);

        Instance.ShakeComp.m_AmplitudeGain = constAmpl = 0f;
        Instance.ShakeComp.m_FrequencyGain = constFreq = 0f;
    }

//  CAMERA FUNCTIONS
    public static void SetActiveCamera(CinemachineVirtualCamera Camera)
    {
        foreach (var i in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            i.gameObject.SetActive(false);
        }
        Camera.gameObject.SetActive(true);
    }
}
