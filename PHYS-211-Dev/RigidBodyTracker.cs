using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Required to use Func
using System;
using System.Linq;
//Defining Measure to a function that takes a Rigid body and returns a float
using Measure = System.Func<UnityEngine.Rigidbody, float>;


public class RigidBodyTracker : MonoBehaviour {


    public GameObject trackedGameObject = null;
    private Rigidbody trackedRigidBody = null;
	// Use this for initialization
	void Start () {
        trackedRigidBody = trackedGameObject.GetComponent<Rigidbody>();

        //TODO: Make measures and try it out hyperlul
        Measure kineticEnergyMeasure = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        
    }
}

public class MeasureList
{
    private Dictionary<string, Measure> measureRegistry;

    public MeasureList()
    {
        measureRegistry = new Dictionary<string, Measure>();
    }

    /// <summary>
    /// Add a Measure to the MeasureList 
    /// </summary>
    /// <param name="key">The key uniquely defining the measure.  If a Measure under
    /// the same key already exists, that measure will be overidden</param>
    /// <param name="measure">The Measure to add to the MeasureList</param>
    public void AddMeasure(string key, Measure measure)
    {
        measureRegistry.Add(key, measure); 
    }

    /// <summary>
    /// Return the Measure stored at a given key, or null if that key does not exist
    /// </summary>
    /// <param name="key">The key for the Measure</param>
    /// <returns>The Measure stored at a given key, or null if that key does not exist</returns>
    public Measure GetMeasure(string key)
    {
        Measure measure = null;
        measureRegistry.TryGetValue(key, out measure);
        return measure;
    }

    /// <summary>
    /// Return a List of all of the keys currently in the MeasureList
    /// </summary>
    /// <returns>A List of all the keys currently in the MeasureList</returns>
    public List<string> GetKeys()
    {
        return measureRegistry.Keys.ToList<string>();
    }


    //TODO: Provide example in doc
    /// <summary>
    /// Take in a RigidBody and a List of keys and return an ordered list of 
    ///     the calculated measures on the given RigidBody.
    ///     
    /// 
    /// </summary>
    /// <param name="rb">The RigidBody to perform the measure on</param>
    /// <param name="keys">An ordered List of keys to measures</param>
    /// <returns></returns>
    public List<float> CalculateMeasures(Rigidbody rb, List<string> keys)
    {
        return keys.Select( key => measureRegistry[key](rb) ).ToList<float>();
    }
}
