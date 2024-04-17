using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    public float lineLength = 5f; // Length of the line
    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the LineRenderer component attached to the GameObject
        lineRenderer = GetComponent<LineRenderer>();

        // Set up the LineRenderer
        lineRenderer.startWidth = 0.1f; // Set the width of the line
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.red; // Set the color of the line
    }

    void Update()
    {
        // Calculate the endpoint of the line based on rotation
        Vector3 endPoint = transform.position + transform.up * lineLength;

        // Create a ray from the current position in the direction of transform.up
        Ray ray = new Ray(transform.position, transform.up);

        // Check if the ray intersects with any collider
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, lineLength))
        {
            endPoint = hit.point; // Set the endpoint to the hit point
        }

        // Update the LineRenderer positions
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}
