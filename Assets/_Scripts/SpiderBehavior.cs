using UnityEngine;

public enum SpiderState
{
    Idle,
    Walk,
    Attack,
    Die
}

public enum PathMovementStyle
{
    Continuous,
    Slerp,
    Lerp,
}

public class SpiderBehavior : MonoBehaviour
{
    public float MovementSpeed;
    public float RotationSpeed; 
    public Transform PathContainer;
    public PathMovementStyle MovementStyle;
    public bool LoopThroughPoints;
    public bool StartAtFirstPointOnAwake;

    private Transform[] points;
    private int currentTargetIdx = 0;
    private Animator animator;
    public SpiderState currentState = SpiderState.Idle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SetupPath();
    }

    private void Update()
    {
        switch (currentState)
        {
            case SpiderState.Idle:
                animator.SetBool("IsWalking", false);
                break;
            case SpiderState.Walk:
                MoveOnPath();
                animator.SetBool("IsWalking", true);
                break;
            case SpiderState.Attack:
                animator.SetTrigger("spiderHiss");
                break;
            case SpiderState.Die:
                // Implement die behavior
                break;
        }
    }

    private void MoveOnPath()
    {
        if (points == null || points.Length == 0) return;

        var distance = Vector3.Distance(transform.position, points[currentTargetIdx].position);
        if (Mathf.Abs(distance) < 0.1f)
        {
            currentTargetIdx++;
            if (currentTargetIdx >= points.Length)
            {
                if (LoopThroughPoints)
                    currentTargetIdx = 0;
                else
                    currentTargetIdx = points.Length - 1;
            }
        }

        switch (MovementStyle)
        {
            default:
            case PathMovementStyle.Continuous:
                transform.position = Vector3.MoveTowards(transform.position, points[currentTargetIdx].position, MovementSpeed * Time.deltaTime);
                break;
            case PathMovementStyle.Lerp:
                transform.position = Vector3.Lerp(transform.position, points[currentTargetIdx].position, MovementSpeed * Time.deltaTime);
                break;
            case PathMovementStyle.Slerp:
                transform.position = Vector3.Slerp(transform.position, points[currentTargetIdx].position, MovementSpeed * Time.deltaTime);
                break;
        }

        Vector3 direction = (points[currentTargetIdx].position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private void SetupPath()
    {
        if (PathContainer == null) return;
        
        points = PathContainer.GetComponentsInChildren<Transform>();
        if (StartAtFirstPointOnAwake)
        {
            transform.position = points[0].position;
        }
    }

    private void Attack()
    {
        // Implement attack logic here

        // For demonstration purposes, let's switch back to walk state after a short delay
        Invoke("ChangeToWalkState", 2.0f);
    }

    private void ChangeToWalkState()
    {
        // Switch back to walk state
        ChangeState(SpiderState.Walk);
    }

    private void ChangeState(SpiderState newState)
    {
        if (newState != currentState)
        {
            // Handle exit logic for current state
            switch (currentState)
            {
                case SpiderState.Idle:
                    // Exit idle state
                    break;
                case SpiderState.Walk:
                    // Exit walk state
                    break;
                case SpiderState.Attack:
                    // Exit attack state
                    break;
                case SpiderState.Die:
                    // Exit die state
                    break;
            }

            // Update state
            currentState = newState;

            // Handle enter logic for new state
            switch (currentState)
            {
                case SpiderState.Idle:
                    // Enter idle state
                    break;
                case SpiderState.Walk:
                    // Enter walk state
                    break;
                case SpiderState.Attack:
                    // Enter attack state
                    break;
                case SpiderState.Die:
                    // Enter die state
                    break;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (points == null || points.Length == 0) return;
        var idx = 0;
        foreach (var point in points)
        {
            Gizmos.color = idx < currentTargetIdx ? Color.red : (idx > currentTargetIdx ? Color.green : Color.yellow);
            Gizmos.DrawWireSphere(point.position, 1f);
            idx++;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, points[currentTargetIdx].position);
    }
#endif
}