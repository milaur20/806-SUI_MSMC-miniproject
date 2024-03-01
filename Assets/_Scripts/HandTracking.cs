using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;

    public Vector3 rightHandLoc;
    public Vector3 leftHandLoc;
    public Vector3 rightHandRot;
    public Vector3 leftHandRot;

    void Start()
    {
        leftHand = GameObject.Find("LeftHandAnchor");
        rightHand = GameObject.Find("RightHandAnchor");
    }

    void Update()
    {
        // Get the pose of the specified hand
        GetHandLocation();
        GetHandRotation();
    }

    // Get the position of each hand
    public void GetHandLocation()
    {
        rightHandLoc = rightHand.transform.position;
        leftHandLoc = leftHand.transform.position;
        //Debug.Log("Right Hand Location: " + rightHandLoc + " Left Hand Location: " + leftHandLoc);
    }

    // Get the rotation of each hand
    public void GetHandRotation()
    {
        rightHandRot = rightHand.transform.rotation.eulerAngles;
        leftHandRot = leftHand.transform.rotation.eulerAngles;
        //Debug.Log("Right Hand Rotation: " + rightHandRot + " Left Hand Rotation: " + leftHandRot);

        Vector3 handForward = rightHand.transform.rotation * Vector3.forward;
        Vector3 groundNormal = Vector3.up; // Assuming the ground is oriented in the Y-up direction
        float angle = Vector3.Angle(handForward, groundNormal);

    }
}
