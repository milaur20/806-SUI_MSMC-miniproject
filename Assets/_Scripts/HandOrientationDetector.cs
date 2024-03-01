using Unity.VisualScripting;
using UnityEngine;

public class HandOrientationDetector : MonoBehaviour
{
    // Adjust this angle according to your sensitivity
    public float horizontalThresholdAngle = 15f;
    public float angle;

    public GameObject cube;
    public Camera centerCam;
    RaycastHit hitInfo;
    public GameObject LastObjHit;
    // Reference to the hand tracking script
    HandTracking handTrackingScript;

    void Start()
    {
        // Find the center camera
        centerCam = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
        // Find the hand tracking script
        handTrackingScript = gameObject.GetComponent<HandTracking>();
    }

    void Update()
    {
        // Check if hand tracking is available
        
        if (handTrackingScript == null)
        {
            Debug.LogWarning("Hand tracking script not found.");
            return;
        }
        
        // Check hand orientation
        Vector3 handForward = handTrackingScript.leftHand.transform.rotation * Vector3.forward;
        Vector3 groundNormal = Vector3.up; // Assuming the ground is oriented in the Y-up direction
        angle = Vector3.Angle(handForward, groundNormal);

        // Determine if hand is held horizontally
        if (angle < horizontalThresholdAngle)
        {
            // Cast a ray from the center of the screen
            Ray ray = centerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            // Draw the ray
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

            // Perform raycast
            if(Physics.Raycast(ray, out hitInfo))
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }
    }

    void ShowMenu()
    {
       // Check if the ray hits an object
        GameObject hitObject = hitInfo.collider.gameObject;
        if (hitObject.transform.Find("Cube"))
        {
            hitObject.transform.Find("Cube").gameObject.SetActive(true);
            LastObjHit = hitObject;
        }      
    }

    void HideMenu()
    {
        if(LastObjHit != null && LastObjHit.transform.Find("Cube"))
        {
            LastObjHit.transform.Find("Cube").gameObject.SetActive(false);
        }
    }
}

