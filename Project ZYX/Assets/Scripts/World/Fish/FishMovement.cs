using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "FishDestroyer")
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }
}
