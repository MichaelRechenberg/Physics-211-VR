using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//A Measure is a function that takes a Rigidbody and a dictionary of arguments as input, and returns a double (the measured value) as output
public delegate double Measure(Rigidbody rb, Dictionary<string, double> args);

/// <summary>
/// Represent a collection of Measures, which take in a RigidBody and a Dictionary of arguments and return a double as the measured value.
/// </summary>
public class MeasureCollection
{

    private Dictionary<string, Measure> measureRegistry; //Registry to map keys to the Measures associated to each key

    /// <summary>
    /// Construct an empty MeasureCollection
    /// </summary>
    public MeasureCollection()
    {
        measureRegistry = new Dictionary<string, Measure>();
    }

    /// <summary>
    /// Add a Measure to the MeasureCollection 
    /// </summary>
    /// <param name="key">The key uniquely defining the measure.  If a Measure under
    ///     the same key already exists, the old Measure will be overidden by this Measure</param>
    /// <param name="measure">The Measure to add to the MeasureCollection</param>
    public void AddMeasure(string key, Measure measure)
    {
        measureRegistry.Add(key, measure);
    }

    /// <summary>
    /// Remove a Measure from the MeasureCollection that was stored at a given key.
    /// 
    /// If no measure was stored at the given key, the MeasureCollection is uchanged
    /// </summary>
    /// <param name="key">The key to the Measure you want to remove</param>
    public void RemoveMeasure(string key)
    {
        measureRegistry.Remove(key);
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
    /// Return a List of all of the keys currently in the MeasureCollection
    /// </summary>
    /// <returns>A List of all the keys currently in the MeasureCollection</returns>
    public List<string> GetKeys()
    {
        return measureRegistry.Keys.ToList<string>();
    }


    /// <summary>
    /// Take a RigidBody and return an ordered list of the results obtained by applying the Measures 
    ///     keyed by keys in keyList in the order that they appear in keyList.
    ///    
    /// Example: 
    /// 
    ///    Measure kineticEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => KineticEnergy(rb, args);
    ///    Measure gravPotEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => PotentialEnergy(rb, args);
    ///    Measure totalEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => KineticEnergy(rb, args) + PotentialEnergy(rb, args);
    ///    measureList = new MeasureCollection();
    ///    measureList.AddMeasure("KE", kineticEnergyMeasure);
    ///    measureList.AddMeasure("GPE", gravPotEnergyMeasure);
    ///    measureList.AddMeasure("Total Energy", totalEnergyMeasure);
    ///    
    /// 
    ///    //Construct the arguments to pass to the potential energy and total energy measure
    ///    Dictionary<string, double> argsPE = new Dictionary<string, double>();
    ///    double gravity = 9.81;
    ///    argsPE.Add("gravity", gravity);
    ///    double height = 10
    ///    argsPE.Add("height", height);
    ///    
    ///    //Determine which Measures we want to calculate
    ///    //Note that we are considering a subset of the total Measures registered and that we can define the order
    ///    //   of measurements to be diffent to the order that the Measures were added
    ///    List<string> keyList = new List<string>();
    ///    keyList.Add("GPE");
    ///    keyList.Add("KE");
    ///    //Construct a list of the arguments we want to pass to each measure, in the same order as they are within keyList
    ///    List<Dictionary<string, double>> argList = new List<Dictionary<string, double>>();
    ///    argList.Add(argsPE); //args to give to PE
    ///    argList.Add(new Dictionary<string, double>()); //args to give to KE 
    ///    argList.Add(argsPE); //args to give to Total Energy 
    ///    
    ///    //Assume Rigidbody rb has mass 5, velocity 4
    ///    
    ///    measureList.CalculateMeasures(rb, keyList, argList) ----> [490.5, 40]
    ///    
    /// 
    ///     
    /// </summary>
    /// 
    /// <param name="rb">The RigidBody to measure</param>
    /// <param name="keyList">An ordered List of keys to Measures within the MeasureCollection.  This can be a subset of all the keys within MeasureCollection</param>
    /// <param name="argList">A list of Dictionarys that contain the arguments to give to the Measure with the 
    ///     key given at idx i. If the Measure does not require any arugments, pass in an empty dictionary</param>
    /// <returns>An ordered List of measurements, where the i'th element in the List is the measured value from the Measure keyed by the i'th key in keyList.
    /// If the count of elements in keyList does not match the count of elements in argList, null is returned</returns>
    public List<double> CalculateMeasures(Rigidbody rb, List<string> keyList, List<Dictionary<string, double>> argList)
    {
        if (keyList.Count() != argList.Count())
        {
            return null;
        }

        List<double> measuredResults = new List<double>();

        for (int i = 0; i < keyList.Count(); i++)
        {
            string key = keyList[i];
            Dictionary<string, double> args = argList[i];
            Measure measure = measureRegistry[key];
            measuredResults.Add(measure(rb, args));
        }

        return measuredResults;

    }

}
