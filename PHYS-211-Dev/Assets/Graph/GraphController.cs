using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Script to control the rendering of data in a scatter plot
/// This script should be a component of the ScatterPlot prefab
/// 
/// Clicking the F key will clear all glyphs from the ScatterPlot this controller is controlling
/// </summary>
public class GraphController : MonoBehaviour {

    
    public GameObject plottingArea; //The cubic area that we will plot the glpyhs on top of, set in Unity
    public GameObject srcDataPointGlyph; //The object that we will use to represent each data point (as a glyph), set in Unity



    //Graph labels, set in Unity
    public Text titleText;
    public Text xAxisText;
    public Text xMinText;
    public Text xMaxText;
    public Text yAxisText;
    public Text yMinText;
    public Text yMaxText;

    //Numerical bounds for the x and y variable of the graph
    private float X_RANGE_MIN = Mathf.Infinity;
    private float X_RANGE_MAX = -Mathf.Infinity;
    private float Y_RANGE_MIN = Mathf.Infinity;
    private float Y_RANGE_MAX = -Mathf.Infinity;


    private Vector3 plottingAreaOrigin; //The "origin" point of the plotting area (bottom left corner of the GameObject)
    private List<GameObject> glyphList; //List of all the glyphs currently rendered

 

    void Start()
    {
        glyphList = new List<GameObject>();
        plottingAreaOrigin = CalcPlotAreaOrigin(plottingArea);
    }

    void Update()
    {
     
    }


    /// <summary>
    /// Return the world coordinates of the bottom left of the plotting area (i.e. the "origin" of the graph)
    /// </summary>
    /// <param name="plottingArea">The GameObject serving as the plotting area</param>
    /// <returns></returns>
    private Vector3 CalcPlotAreaOrigin(GameObject plottingArea)
    {
        Vector3 plottingDimensions = PlottingAreaDimensions(plottingArea);
        float compensatedXCoord = plottingArea.transform.position.x - (plottingDimensions.x / 2);
        float compensatedYCoord = plottingArea.transform.position.y - (plottingDimensions.y / 2);
        float compensatedZCoord = plottingArea.transform.position.z - (plottingDimensions.z / 2);

        return new Vector3(compensatedXCoord, compensatedYCoord, compensatedZCoord);
    }

    /// <summary>
    /// Get the dimensions of the plotting area's Renderer as a Vector3(x, y, z)
    /// </summary>
    /// <param name="plottingArea">The GameObject serving as the plotting area</param>
    /// <returns></returns>
    private Vector3 PlottingAreaDimensions(GameObject plottingArea)
    {
        Renderer plottingRenderer = plottingArea.GetComponent<Renderer>();
        return plottingRenderer.bounds.size;
    }

    /// <summary>
    /// Set the minimum and maximum range for the x values in the graph
    /// </summary>
    /// <param name="xMin">The minimum acceptable x value</param>
    /// <param name="xMax">The minimum acceptable x value</param>
    public void SetXRange(float xMin, float xMax)
    {
        X_RANGE_MIN = xMin;
        X_RANGE_MAX = xMax;

        xMinText.text = xMin.ToString();
        xMaxText.text = xMax.ToString();
    }


    /// <summary>
    /// Set the minimum and maximum range for the y values in the graph
    /// </summary>
    /// <param name="xMin">The minimum acceptable y value</param>
    /// <param name="xMax">The minimum acceptable y value</param>
    public void SetYRange(float yMin, float yMax)
    {
        Y_RANGE_MIN = yMin;
        Y_RANGE_MAX = yMax;

        yMinText.text = yMin.ToString();
        yMaxText.text = yMax.ToString();
    }


    /// <summary>
    /// Add an (x,y) data point as a glyph on the scatter plot
    /// 
    /// If either the x or y coordinate is out of their respective ranges
    ///     ( as was set by previous calls to SetXRange() and SetYRange() ), then the point is not added to the graph
    /// </summary>
    /// <param name="x">The x-value within the x-range of this Graph</param>
    /// <param name="y">A y value within the y-range of this Graph</param>
    /// <returns>true if the glyph was added to the graph, false otherwise</returns>
    public bool AddPoint(float x, float y)
    { 
        if(!IsWithinRange(x, X_RANGE_MIN, X_RANGE_MAX) || !IsWithinRange(y, Y_RANGE_MIN, Y_RANGE_MAX))
        {
            return false;
        }

    
        //scale the position of the glyph to be a percentage of the range for each variable
        float rangeScaledX = Mathf.Abs(x / (X_RANGE_MAX - X_RANGE_MIN));
        float rangeScaledY = Mathf.Abs(y / (Y_RANGE_MAX - Y_RANGE_MIN));

        //scale the position of the glyph again, to now be a percentage of the height and width of the plotting area
        Vector3 plottingAreaDimensions = PlottingAreaDimensions(plottingArea);
        float plotScaledX = rangeScaledX * plottingAreaDimensions.x;
        float plotScaledY = rangeScaledY * plottingAreaDimensions.y;
        Vector3 glyphPlotTranslation = new Vector3(plotScaledX, plotScaledY, 0);

        GameObject dataPointGlyph = Instantiate(srcDataPointGlyph);
        dataPointGlyph.transform.SetParent(transform, false);
        dataPointGlyph.transform.rotation = gameObject.transform.rotation;
        dataPointGlyph.transform.position = plottingAreaOrigin;
        dataPointGlyph.transform.Translate(glyphPlotTranslation);

        glyphList.Add(dataPointGlyph);

        return true;
    }

    /// <summary>
    /// Return true if "value" is within [rangeMin, rangeMax], false otherwise
    /// </summary>
    /// <param name="value">The value to test</param>
    /// <param name="rangeMin">The minimum bound of the range (inclusive)</param>
    /// <param name="rangeMax">The maximum bound of the range (inclusive)</param>
    /// <returns></returns>
    private bool IsWithinRange(double value, double rangeMin, double rangeMax)
    {
        return value >= rangeMin && value <= rangeMax;
    }

    /// <summary>
    /// Remove all currently rendered glyphs from the graph
    /// 
    /// This will result in the data glyphs to explode off of the graph, then disappear after 
    /// <param name="numSecondDelay"/>The number of seconds to wait before Destroy()'ing the glyph Game Object</param>
    /// </summary>
    public void ClearGlyphs(int numSecondDelay)
    {
        foreach(GameObject point in glyphList)
        {
            point.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(point, numSecondDelay);
        }
        glyphList.Clear();
    }

    /// <summary>
    /// Set the text for the main title of the graph
    /// </summary>
    /// <param name="title">The new title for the graph</param>
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    /// <summary>
    /// Set the text for the x-axis label
    /// </summary>
    /// <param name="label"></param>
    public void SetXLabel(string label)
    {
        xAxisText.text = label;
    }

    /// <summary>
    /// Set the text for the y-axis label
    /// </summary>
    /// <param name="label"></param>
    public void SetYLabel(string label)
    {
        yAxisText.text = label;
    }

}
