using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script that, when another GameObject (e.g. the Lab Cart) hits this script's collider,
///     will call MakeMeasurement() of the MainController
/// </summary>
public class AddHeightVelocityPointToGraph : MonoBehaviour {

    //The GameObject that has the main controller for the Cart Lab as a component
    public GameObject mainControllerGameObject;

    //The main controller for the Cart Lab
    private CartLabController mainController;

    private void Start()
    {
        mainController = mainControllerGameObject.GetComponent<CartLabController>(); 
    }


    /// <summary>
    /// Call MakeMeasurement() of the main controller, passing it the Rigidbody of the GameObject that
    ///     hit the collider box (which should be the lab cart)
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody cartRigidbody = other.GetComponent<Rigidbody>();
        mainController.MakeMeasurement(cartRigidbody);
        
    }

}
