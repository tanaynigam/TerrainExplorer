using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public int POIIndex;

    public float rotationsPerMinute;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,6f * rotationsPerMinute * Time.deltaTime,0);
    }
}
