using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Change the color of a GameObject (that has a RigidBody) to be in a gradient between
///     two colors, depending on the ratio of some measure within [0, 1]
///     
/// For example, we can show at what points the kinetic energy of an object is highest
///     when the object is mostly HOT_COLOR and when the kinetic energy of the object
///     is lowest when it is mostly COLD_COLOR
/// </summary>
public class EnergyHue : MonoBehaviour {

    //Color gradient for the energy colors
    private Gradient colorGradient = null;
    //Color to be used when the color temperature is lowest (0)
    private Color COLD_COLOR = Color.blue;
    //Color to be used when teh color temperature is hottest (1)
    private Color HOT_COLOR = Color.red;

    //Used to calculate KE and PE
    private MeasureCollection measureCollection = null;

    //The GameObject (and corresponding RigidBody) that we are tracking. Set in Unity
    public GameObject trackedGameObject = null;
    private Rigidbody trackedRigidBody = null;

    //Correction for the height of the trackedRigidBody because its transform.y coordinate
    //  is from the center of the 3D object...otherwise there would be non-zero non-trivial
    //  amount of PE calculuated when the tracked game object is resting on the ground
    //This is an approximation
    private float HEIGHT_CORRECTION = 0.2f;


	// Use this for initialization
	void Start ()
    {

        colorGradient = InitGradient();

        //Init Measure collection
        Measure kineticEnergyMeasure = Assets.Measures.KineticEnergy;
        Measure gravEnergyMeasure = Assets.Measures.GravitationalPotentialEnergy;
        Measure totalEnergyMeasure = (Rigidbody rb, Dictionary<string, double> args) => Assets.Measures.KineticEnergy(rb, args) + Assets.Measures.GravitationalPotentialEnergy(rb, args);

        measureCollection = new MeasureCollection();
        measureCollection.AddMeasure("KE", kineticEnergyMeasure);
        measureCollection.AddMeasure("GPE", gravEnergyMeasure);
        measureCollection.AddMeasure("Total Energy", totalEnergyMeasure);


        trackedRigidBody = trackedGameObject.GetComponent<Rigidbody>();

    }



    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate()
    {
        List<string> keyList = new List<string>();
        keyList.Add("KE");
        keyList.Add("GPE");
        keyList.Add("Total Energy");

        List<Dictionary<string, double>> argList = new List<Dictionary<string, double>>();
        //The KE measure does not require any arguments, pass in an empty Dictionary
        argList.Add(new Dictionary<string, double>());

        //Construct the arguments to pass to the potential energy and total energy Measure
        Dictionary<string, double> argsPE = new Dictionary<string, double>();
        double gravity = Physics.gravity.magnitude;
        argsPE.Add("gravity", gravity);
        double height = trackedRigidBody.transform.position.y - HEIGHT_CORRECTION;
        argsPE.Add("height", height);

        argList.Add(argsPE);
        argList.Add(argsPE);

        List<double> measureResults = measureCollection.CalculateMeasures(trackedRigidBody, keyList, argList);

        double kineticEnergy = measureResults[0];
        double potentialEnergy = measureResults[1];
        double totalEnergy = measureResults[2];

        //This shows when there is the most kinetic energy
        //float gradientTemperature = DetermineGradientTemperature(kineticEnergy, totalEnergy, 0.0F);
        //This shows when there is the most potential energy
        float gradientTemperature = DetermineGradientTemperature(potentialEnergy, totalEnergy, 6.0F);

        trackedGameObject.GetComponent<Renderer>().material.color = colorGradient.Evaluate(gradientTemperature);


    }

    /// <summary>
    /// Determine the temperature for the energy hue (a number between 0 and 1)
    /// 
    /// The temperature is the ratio between focalEnergy to totalEnergy (unless the absolute value
    ///     of totalEnergy is less than minTotalEnergy, which in that case 0 is returned)
    /// 
    /// </summary>
    /// <param name="focalEnergy">The measure of energy that we want to focus on (KE, PE, etc.)  Joules </param>
    /// <param name="totalEnergy">The total energy of the Rigidbody which also contains all of the focalEnergy.  Joules</param>
    /// <param name="activationEnergy">The minimum threshold of total energy required to return a non-zero temperature.  If the absolute
    ///     value of totalEnergy is less than this threshold, 0 is returned.  Otherwise, we return the regular, calculated temperature</param>
    /// <returns></returns>
    private float DetermineGradientTemperature(double focalEnergy, double totalEnergy, double activationEnergy)
    { 
        if(Mathf.Abs((float)totalEnergy) < activationEnergy)
        {
            return 0.0F;
        }
        else
        {
            return (float)(focalEnergy / totalEnergy);
        }
    }

    /// <summary>
    /// Initalizes a linear color Gradient between COLD_COLOR and HOT_COLOR
    /// </summary>
    /// <returns>The initalized Gradient</returns>
    private Gradient InitGradient()
    {
        //Init gradient
        GradientColorKey[] gck = new GradientColorKey[2];
        gck[0].color = COLD_COLOR;
        gck[0].time = 0.0F;
        gck[0].color = HOT_COLOR;
        gck[0].time = 1.0F;


        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gak[0].alpha = 1.0F;
        gak[0].time = 0.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;

        Gradient gradient = new Gradient();
        gradient.SetKeys(gck, gak);

        return gradient;
    }

}
