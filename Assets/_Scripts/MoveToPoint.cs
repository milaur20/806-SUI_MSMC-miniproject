using UnityEngine;

public enum LaserPointers
{
    Inactive,
    LeftLaserPointer,
    RightLaserPointer,
}
public class MoveToPoint : MonoBehaviour
{
    private QuizSystem quizSystem;
    public Transform startingPosition;
    public GameObject goalPosition;
    public LaserPointers currentState;
    public Vector3 targetPoint; // The point to move towards
    public float speed = 1.0f; // Speed modifier
    public LaserPointer laserPointerRight; // The laser pointer
    public LaserPointer laserPointerLeft; // The laser pointer

    void Start()
    {
        // Get the LaserPointer component attached to the GameObject
        laserPointerLeft = GameObject.Find("LaserPointer_l").GetComponentInChildren<LaserPointer>();

        laserPointerRight = GameObject.Find("LaserPointer_r").GetComponentInChildren<LaserPointer>();
        
        quizSystem = FindAnyObjectByType<QuizSystem>();

        transform.position = startingPosition.position;
    }
    private void Update()
    {
        // Get the LaserPointer component attached to the GameObject
        laserPointerLeft = GameObject.Find("LaserPointer_l").GetComponentInChildren<LaserPointer>();

        //laserPointerRightObj = GameObject.Find("LaserPointer_r");

        if(!laserPointerLeft || !laserPointerRight)
        {
            return;
        }
        else if(laserPointerRight || laserPointerLeft)
        {
            if(laserPointerRight.enableRayCast)
            {
                currentState = LaserPointers.RightLaserPointer;
            }
            else if (laserPointerLeft.enableRayCast)
            {
                currentState = LaserPointers.LeftLaserPointer;
            }
        }

        switch (currentState)
        {
            case LaserPointers.Inactive:
                break;
            case LaserPointers.LeftLaserPointer:
                UpdateLeftLaser();
                break;
            case LaserPointers.RightLaserPointer:
                UpdateRightLaser();
                break;
        }
                //check if collider is hitting goal object
        if (gameObject.GetComponent<Collider>().bounds.Intersects(goalPosition.GetComponent<Collider>().bounds))
        {
            Debug.Log("Goal Reached");
            quizSystem.EvaluateAnswer(true);
        }
        //check if collider is hitting object with respawn tag
        else if (gameObject.GetComponent<Collider>().bounds.Intersects(GameObject.FindGameObjectWithTag("Respawn").GetComponent<Collider>().bounds))
        {
            Debug.Log("Respawn");
            transform.position = startingPosition.position;
        }
    }
    private void UpdateRightLaser()
    {
        targetPoint = laserPointerRight.endPoint;
        // Check if the target point is set
        if (targetPoint != null)
        {
            // Calculate the direction towards the target point
            Vector3 direction = targetPoint - transform.position;

            // Calculate the distance to the target point
            float distance = direction.magnitude;

            transform.position += direction.normalized * speed * Time.deltaTime;

            //check if distance between transform.position and targetPoint is less than 0.1f
            if (distance < 0.001f)
            {
                //set transform.position to targetPoint
                transform.position = targetPoint;
            }
        }

    }
    void UpdateLeftLaser()
    {
        targetPoint = laserPointerLeft.endPoint;
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
