using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTextOrientation : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform.position);
        gameObject.transform.Rotate(new Vector3(0, 180, 0));        
    }
}
