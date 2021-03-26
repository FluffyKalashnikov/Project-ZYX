using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioPreview : Editor
{
    private AudioSource source = null;

    public void OnEnable()
    {
        source = EditorUtility.CreateGameObjectWithHideFlags("Preview Audio Source", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
    }
    public void OnDisable()
    {
        DestroyImmediate(source.gameObject);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioEvent audioEvent = (AudioEvent)target;
        GUILayout.BeginHorizontal();

        GUI.enabled = false;
        GUILayout.TextField("Previewing:", GUILayout.Width(75));
        GUILayout.Space(5);
        GUILayout.TextField($"{(source.clip ? source.clip.name : string.Empty)}");
        GUI.enabled = true;


        if (GUILayout.Button("Preview Audio", GUILayout.ExpandWidth(false), GUILayout.Width(100)))
        {
            audioEvent.Play(source);                            
        }
        if (GUILayout.Button("Stop",          GUILayout.ExpandWidth(false), GUILayout.Width(50)))
        {
            source.Stop();
            source.clip = null;
        }

        GUILayout.EndHorizontal();
    }
}
