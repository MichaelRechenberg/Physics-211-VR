using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Demo for the ScatterPlot prefab and GraphController
/// 
/// This script will freeze the tracked rigid body on Start() (by setting
///     its isKinematic to false)
///     
/// To start the trial, hit the "s" key on the keyboard.  This will
///     allow the rigid body to fall and for the collected data to be shown
///     on the graphs
/// </summary>
public class GraphDemoScript : MonoBehaviour
{


    //Set in Unity
    public GameObject trackedGameObject = null;
    private Rigidbody trackedRigidBody = null;


    //The MeasureCollection used in this experiment
    private MeasureCollection measureCollection = null;

    //Graph Controllers to render the various energy readings, set in Unity
    public GraphController kineticEnergyGraphController;
    public GraphController potentialEnergyGraphController;
    public GraphController totalEnergyGraphController;


    //Flag if we are currently collecting data
    private static bool collectingData = false;
    //The total number of observations we want to make
    private static int TOTAL_NUM_OBSERVATIONS = 100;
    //Number of observations currently made
    private static int observationCount = 0;


    //Scale of the cube/sphere in y dimension, used for determining height between the tracked object and the floor
    private static double PRIMITIVE_OBJECT_SCALE = 1;
    //Height correction between the center of the cube to the bottom of the cube
    private static double HEIGHT_CORRECTION = PRIMITIVE_OBJECT_SCALE / 2;


    // Use this for initialization
    void Start()
    {

        //Capture the RigidBody we want to track and set it to be kinematic until we start the trial
        trackedRigidBody = trackedGameObject.GetComponent<Rigidbody>();
        trackedRigidBody.isKinematic = true;


        //Define and add our measures to a MeasureCollection
        Measure kineticEnergyMeasure = Assets.Measures.KineticEnergy;
        Measure gravPotEnergyMeasure = Assets.Measures.GravitationalPotentialEnergy;
        //Since Measure is just a synonym for Func, lambdas with the same signature as Measure are valid as well
        Measure totalEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => Assets.Measures.KineticEnergy(rb, args) + Assets.Measures.GravitationalPotentialEnergy(rb, args);

        measureCollection = new MeasureCollection();
        measureCollection.AddMeasure("KE", kineticEnergyMeasure);
        measureCollection.AddMeasure("GPE", gravPotEnergyMeasure);
        measureCollection.AddMeasure("Total Energy", totalEnergyMeasure);



        InitGraphs();


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            collectingData = true;
            trackedRigidBody.isKinematic = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Clearing glyphs");
            kineticEnergyGraphController.ClearGlyphs(3);
            totalEnergyGraphController.ClearGlyphs(3);
            potentialEnergyGraphController.ClearGlyphs(3);
        }
    }


    /// <summary>
    /// Set the appropriate GraphController variable for each energy Graph, and set
    ///     the ranges and titles for each graph
    /// </summary>
    private void InitGraphs()
    {
        kineticEnergyGraphController.SetXRange(0, TOTAL_NUM_OBSERVATIONS * Time.fixedDeltaTime);
        kineticEnergyGraphController.SetYRange(0, 80);
        kineticEnergyGraphController.SetTitle("Kinetic Energy");
        kineticEnergyGraphController.SetYLabel("KE (Joules)");
        kineticEnergyGraphController.SetXLabel("Time (seconds)");

        potentialEnergyGraphController.SetXRange(0, TOTAL_NUM_OBSERVATIONS * Time.fixedDeltaTime);
        potentialEnergyGraphController.SetYRange(0, 80);
        potentialEnergyGraphController.SetTitle("Potential Energy");
        potentialEnergyGraphController.SetYLabel("PE (Joules)");
        potentialEnergyGraphController.SetXLabel("Time (seconds)");

        totalEnergyGraphController.SetXRange(0, TOTAL_NUM_OBSERVATIONS * Time.fixedDeltaTime);
        totalEnergyGraphController.SetYRange(0, 80);
        totalEnergyGraphController.SetTitle("Total Energy");
        totalEnergyGraphController.SetYLabel("TE (Joules)");
        totalEnergyGraphController.SetXLabel("Time (seconds)");
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
        keyList.Add("KE");
        keyList.Add("GPE");
        keyList.Add("Total Energy");

        //Construct a list of the arguments we want to pass to each measure, in the same order as they are within keyList
        List<Dictionary<string, double>> argList = new List<Dictionary<string, double>>();
        argList.Add(new Dictionary<string, double>()); //args to give to KE 
        argList.Add(argsPE); //args to give to PE
        argList.Add(argsPE); //args to give to Total Energy (same as PE's args)


        //Graph each observaton
        if (collectingData && observationCount < TOTAL_NUM_OBSERVATIONS)
        {
            List<double> measures = measureCollection.CalculateMeasures(trackedRigidBody, keyList, argList);

            float experimentTime = observationCount * Time.fixedDeltaTime;
            //The array indexing is based off the order of keys in keyList
            float kineticEnergy = (float) measures[0];
            float potentialEnergy = (float)measures[1];
            float totalEnergy = (float)measures[2];

            kineticEnergyGraphController.AddPoint(experimentTime, kineticEnergy);
            potentialEnergyGraphController.AddPoint(experimentTime, potentialEnergy);
            totalEnergyGraphController.AddPoint(experimentTime, totalEnergy);

            observationCount++;

        }


    }



}



