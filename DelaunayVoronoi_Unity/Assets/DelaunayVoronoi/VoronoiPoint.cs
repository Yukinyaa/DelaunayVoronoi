using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DelaunayVoronoi;

[ExecuteInEditMode]
public class VoronoiPoint : MonoBehaviour {
    [Range(0,10)]
    public float weight = 1;
    void Update()
    {
        if (transform.hasChanged)
        {
            Debug.Log("moved!");
            VoronoiGenerator.UpdatePoints();
        }
        transform.hasChanged = false;
    }

    public Point p;
    
}
