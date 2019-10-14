using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCheckCollider : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter : " + other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision enter : " + collision.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exit : " + other.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("collision exit : " + collision.gameObject.name);
    }
}
