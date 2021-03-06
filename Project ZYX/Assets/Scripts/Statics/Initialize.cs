using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Initialize : MonoBehaviour
{
    [SerializeField] private float      timeDelay = 0f;
    [SerializeField] private UnityEvent initialization;
    private void Awake()
    {
        StartCoroutine(Init());
        IEnumerator Init()
        {
            yield return new WaitForSeconds(timeDelay);
            initialization.Invoke();
        }
    }
}
