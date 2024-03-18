using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    // Speed variable that can be adjusted in the Unity Inspector
    public float rotationSpeed = 50f;

    void Update()
    {
        // Rotate the object continuously around its up axis (Y-axis)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
