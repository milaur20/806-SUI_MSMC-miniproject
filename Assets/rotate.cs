using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        // Rotate the object continuously around its up axis (Y-axis)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
