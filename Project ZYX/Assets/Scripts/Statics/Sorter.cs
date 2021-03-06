using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
[ExecuteAlways]
#endif
public class Sorter : MonoBehaviour
{
    #if UNITY_EDITOR
    [SerializeField] private string path   = string.Empty;
    [SerializeField] private bool   active = false;

    [SerializeField] private bool   activeDuringRuntime = false;

    [ContextMenu("Sort GameObject")]
    void Start()
    {
        // 1. Check if in editor
        if (!active) return;
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
        if (Application.isPlaying) 
        if (!activeDuringRuntime) return;

        // 2. Set parent to last object
        GameObject[] gameObjects = GetGameObjects();
        transform.SetParent(gameObjects.Last().transform);
    }
    



    private GameObject[] GetGameObjects()
    {
        if (path.Length == 0) return null;

        List<GameObject> list        =  new List<GameObject>();
        string[]         paths       =  path.Split('/');
        string[]         childpaths  =  paths.Skip(1).ToArray();
        GameObject       parent      =  null;


        // 1. GET PARENT
        parent = GameObject.Find(paths[0]);

        // 2. IF NO PARENT, CREATE INSTANCE
        if (parent == null)
        {
            // CREATES PARENT
            parent = new GameObject(paths[0]);
            list.Add(parent);

            // CREATES CHILDREN
            foreach(string childpath in childpaths)
            {
                GameObject child = new GameObject(childpath);
                child.transform.SetParent(parent.transform);
                list.Add(child);
            }

            // DEBUGS MESSAGE
            Debug.LogWarning("[SORTER] \"" + path + "\" was not found, instance has been created.");
            return list.ToArray();
        }

        // 3. ELSE IF PARENT, CREATE MISSING CHILDS
        Transform lastChild = parent.transform;
        list.Add(parent);
        bool success = true;
        foreach(string childpath in childpaths)
        {
            // 1. IF NO CHILD EXIST, CREATE NEW
            GameObject child = lastChild.Find(childpath)?.gameObject;
            if (child == null)
            {
                child = new GameObject(childpath);
                child.transform.SetParent(lastChild);
                success = false;
            }

            // 2. ADD TO LIST, UPDATE CURRENT CHILD
            list.Add(child);
            lastChild = child.transform;
        }
        if (!success) 
        {
            Debug.LogWarning("[SORTER] \"" + path + "\" was not found, instance has been created.");
        }

        return list.ToArray();
    }
    #endif
}
