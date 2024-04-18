using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    public Vector3 targetPoint; // The point to move towards
    public float speed = 1.0f; // Speed modifier
    public LaserPointer laserPointer; // The laser pointer

    void Start()
    {
        // Get the LaserPointer component attached to the GameObject
        laserPointer = FindObjectOfType<LaserPointer>();
    }
    private void Update()
    {
        targetPoint = laserPointer.endPoint;
        // Check if the target point is set
        if (targetPoint != null)
        {
            // Calculate the direction towards the target point
            Vector3 direction = targetPoint - transform.position;

            // Calculate the distance to the target point
            float distance = direction.magnitude;

            // Check if the object is not already at the target point
            if (distance > 0.001f)
            {
                // Calculate the movement towards the target point using lerp
                Vector3 movement = Vector3.Lerp(transform.position, targetPoint, speed * Time.deltaTime);

                // Move the object towards the target point
                transform.position = movement;
            }
        }
    }
}
