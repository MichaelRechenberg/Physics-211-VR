using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExperimentControl : MonoBehaviour
{


    //Set in Unity
    public GameObject trackedGameObject = null;
    private Rigidbody trackedRigidBody = null;


    //The MeasureCollection used in this experiment
    private MeasureCollection measureCollection = null;

    //The total number of observations we want to make
    private static int TOTAL_NUM_OBSERVATIONS = 200;
    //Number of observations currently made
    private static int observationCount = 0;


    //Scale of the cube/sphere in y dimension, used for determining height between the tracked object and the floor
    private static double PRIMITIVE_OBJECT_SCALE = 1;
    //Height correction between the center of the cube to the bottom of the cube
    private static double HEIGHT_CORRECTION = PRIMITIVE_OBJECT_SCALE / 2;


    // Use this for initialization
    void Start()
    {

        //Capture the RigidBody we want to track
        trackedRigidBody = trackedGameObject.GetComponent<Rigidbody>();


        //Define and add our measures to a MeasureCollection
        Measure kineticEnergyMeasure = Assets.Measures.KineticEnergy;
        Measure gravPotEnergyMeasure = Assets.Measures.GravitationalPotentialEnergy;
        //Since Measure is just a synonym for Func, lambdas with the same signature as Measure are valid as well
        Measure totalEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => Assets.Measures.KineticEnergy(rb, args) + Assets.Measures.GravitationalPotentialEnergy(rb, args);

        measureCollection = new MeasureCollection();
        measureCollection.AddMeasure("KE", kineticEnergyMeasure);
        measureCollection.AddMeasure("GPE", gravPotEnergyMeasure);
        measureCollection.AddMeasure("Total Energy", totalEnergyMeasure);

        //TODO: Be able to install CsvHelper in VR Lab machines :/
        //Set up the CSV writer


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //Construct the arguments to pass to the potential energy and total energy measure
        Dictionary<string, double> argsPE = new Dictionary<string, double>();
        double gravity = Physics.gravity.magnitude;
        argsPE.Add("gravity", gravity);
        double height = trackedRigidBody.transform.position.y - HEIGHT_CORRECTION;
        argsPE.Add("height", height);

        //Determine which Measures we want to calculate
        List<string> keyList = new List<string>();
        keyList.Add("GPE");
        keyList.Add("KE");
        keyList.Add("Total Energy");

        //Construct a list of the arguments we want to pass to each measure, in the same order as they are within keyList
        List<Dictionary<string, double>> argList = new List<Dictionary<string, double>>();
        argList.Add(argsPE); //args to give to PE
        argList.Add(new Dictionary<string, double>()); //args to give to KE 
        argList.Add(argsPE); //args to give to Total Energy


        //Make some observations and print them to the console for demo purposes
        if (observationCount < TOTAL_NUM_OBSERVATIONS)
        {
            List<double> measures = measureCollection.CalculateMeasures(trackedRigidBody, keyList, argList);
            for (int i = 0; i < measures.Count(); i++)
            {
                Debug.Log("Measure " + keyList[i] + " had measure " + measures[i]);
            }
            observationCount++;
        }


    }

}



