using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    public QuizSystem quizSystem;
    public float lineLength = 5f; // Length of the line
    private LineRenderer lineRenderer;
    public Vector3 endPoint;
    public bool isHitting;
    public bool enableRayCast;

    void Start()
    {
        // Get the LineRenderer component attached to the GameObject
        lineRenderer = GetComponent<LineRenderer>();

        quizSystem = FindAnyObjectByType<QuizSystem>();
        // Set up the LineRenderer
        lineRenderer.startWidth = 0.1f; // Set the width of the line
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.red; // Set the color of the line
    }

    void Update()
    {
        if(quizSystem.quizQuestions[quizSystem.questionIndex].challengeType == 3)
            // Check if the ray intersects with any collider
            if(enableRayCast)
            {
            // Calculate the endpoint of the line based on rotation
            endPoint = transform.position + transform.up * lineLength;

            // Create a ray from the current position in the direction of transform.up
            Ray ray = new Ray(transform.position, transform.up);
            RaycastHit hit;
                if (Physics.Raycast(ray, out hit, lineLength))
                {
                    isHitting = true;
                    endPoint = hit.point; // Set the endpoint to the hit point

                    // Update the LineRenderer positions
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, endPoint);
                    lineRenderer.enabled = true;
                }
                else
                {
                    isHitting = false;
                    lineRenderer.enabled = false;
                }
            }
            else
            {
                isHitting = false;
            }
    }

    public void resetIsHitting()
    {
        isHitting = false;
    }

    public void enableRay()
    {
        enableRayCast = true;
        lineRenderer.enabled = true;
    }

    public void disableRay()
    {
        enableRayCast = false;
        isHitting = false;
        lineRenderer.enabled = false;
    }
}
