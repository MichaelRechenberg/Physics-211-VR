using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main Controller script for the Cart Lab
/// 
/// Determines how to respond to user input (reset car, clear graph) as well as initial lab setup
/// 
/// </summary>
public class CartLabController : MonoBehaviour {

    //The cart that will roll down the ramp and be tracked in the experiment
    //This cart can be reset to the top of the ramp via ResetCart()
    //The mass of this cart will dictate the velocity that is tracked
    public GameObject labCart;
    //The hinged ramp that labCart will "roll" down
    public GameObject labRamp;
    //The top of the floor, used for calculating the "height" of the cart
    public GameObject floorTop;
    //Respawn point of the cart, w.r.t. the ramp
    public GameObject labCartRespawnPoint;

    //How far along the "up" vector of the labCartRespawnPoint we want to respawn the labCart at
    private float RESPAWN_DISPLACEMENT_FACTOR = 0.15f;

    //GraphController of the (final velocity v. initial height) graph
    public GraphController heightVelocityGraphController;

    //TODO: Determine good bounds for the graph
    //Bounds for the x and y axes on the graph
    private float GRAPH_MIN_VELOCITY = 0.0f;
    private float GRAPH_MAX_VELOCITY = 20.0f;
    private float GRAPH_MIN_HEIGHT = 0.0f;
    private float GRAPH_MAX_HEIGHT = 10.0f;


    //Text element for showing the initial height of a trial. Updated in MakeMeasurement()
    public Text trialHeightText;
    //Text element for showing the final velocity of a trial. Updated in MakeMeasurement()
    public Text trialVelocityText;


    //TODO: List of all (initHeight, finalVelocity) pairs from all the trials?

	// Use this for initialization
	void Start () {
        InitializeLab();	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCart(labCart, labCartRespawnPoint, labRamp);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            heightVelocityGraphController.ClearGlyphs(1);
        }
    }

    private void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Make a height and velocity measurement of the labCart, using the y coordinates of the floorTop
    ///     and the labCartRespawnPoint for determining the initial "height" of the labCart
    ///     
    /// This should be called by external script that calls MakeMeasurement() once the Lab Cart
    ///     enters a collision box
    ///     
    /// Also renders the measurements via RenderMeasurements()
    ///     
    /// </summary>
    /// <param name="cartRigidBody">The Rigidbody of the lab cart</param>
    public void MakeMeasurement(Rigidbody cartRigidBody)
    {
        float velocity = cartRigidBody.velocity.magnitude;
        float initialHeight = (labCartRespawnPoint.transform.position - floorTop.transform.position).y;

        RenderMeasurements(initialHeight, velocity);

    }

    /// <summary>
    /// Render the initialHeight and final velocity measurements of the cart to the graph
    ///     as well as the feedback text elements (e.g. trialVelocityText)
    /// </summary>
    /// <param name="initialHeight">The measured initial height for the trial</param>
    /// <param name="velocity">The final velocity of the cart</param>
    private void RenderMeasurements(float initialHeight, float velocity)
    {

        heightVelocityGraphController.AddPoint(initialHeight, velocity);

        trialVelocityText.text = string.Format("Final Velocity: {0} J", velocity);
        trialHeightText.text = string.Format("Initial Height: {0} m", initialHeight);

    }

    /// <summary>
    /// Initialize the lab for when the scene is first loaded
    /// </summary>
    private void InitializeLab()
    {
        ResetCart(labCart, labCartRespawnPoint, labRamp);
        InitializeHeightVelocityGraph(heightVelocityGraphController);
    }

    /// <summary>
    /// Initialize the velocity vs. height graph
    /// </summary>
    /// <param name="controller">The GraphController of the velocity v. height graph</param>
    private void InitializeHeightVelocityGraph(GraphController controller)
    {
        controller.SetTitle("Final Velocity vs. Init Height");
        controller.SetXLabel("Init Height");
        controller.SetXRange(GRAPH_MIN_HEIGHT, GRAPH_MAX_HEIGHT);
        controller.SetYLabel("Final Velocity");
        controller.SetYRange(GRAPH_MIN_VELOCITY, GRAPH_MAX_VELOCITY);
    }
	


    /// <summary>
    /// Reset the cart to its initial position on top of the ramp, RESPAWN_DISPLACMENT_FACTOR units "up" w.r.t. the 
    ///     labCartRespawnPoint's local coordinate system.
    /// </summary>
    /// <param name="labCart">The lab cart</param>
    /// <param name="labCartRespawnPoint">The respawn point of the lab cart</param>
    /// <param name="ramp">The ramp of the lab</param>
    void ResetCart(GameObject labCart, GameObject labCartRespawnPoint, GameObject ramp)
    {
        labCart.transform.rotation = ramp.transform.rotation;

        Vector3 spawnPointDirectionVector = labCartRespawnPoint.transform.up;
        Vector3 newCartPosition = labCartRespawnPoint.transform.position + spawnPointDirectionVector*RESPAWN_DISPLACEMENT_FACTOR;
        labCart.transform.position = newCartPosition;


    }
}
