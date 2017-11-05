using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWhenTriggered : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sensor Bar Was Triggered");
        //Make it so forces no longer move the colliding object (stopping it)
        other.GetComponent<Rigidbody>().isKinematic = true;
    }
}
