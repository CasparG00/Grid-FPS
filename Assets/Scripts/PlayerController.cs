using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cellSize;
    private float HalfSize => cellSize * 0.5f;

    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float snapDistance = 0.01f;
    [Space]
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float snapAngle = 0.01f;

    private Vector3 targetPosition;
    private Vector3 targetForward;

    private bool isMoving;
    private bool isTurning;
    
    private readonly FSM fsm = new();
    private FSM.State idle, moving, turning;

    private void Start()
    {
        CreateIdleState();
        CreateMovingState();
        CreateTurningState();
        
        fsm.PushState(idle);
    }
    
    private void Update()
    {
        fsm.Update(gameObject);
    }

    private bool CanMove(Vector3 direction)
    {
        for (var i = -HalfSize; i <= HalfSize; i += HalfSize)
        {
            var offset = -Vector3.Cross(direction, Vector3.up).normalized * i;
            var origin = transform.position + Vector3.up * 0.5f + offset;
            if (Physics.Raycast(origin, direction, cellSize * 1.5f))
            {
                return false;
            }
        }
        
        return true;
    }

    private void Move(Vector3 direction)
    {
        if (CanMove(direction))
        {
            targetPosition = transform.position + direction * cellSize;
            fsm.PopState();
            fsm.PushState(moving);
            EventSystem.Invoke(EventTypes.endTurn);
        }
    }

    private void CreateIdleState()
    {
        idle = (finiteStateMachine, gameObj) =>
        {
            if (Input.GetKey(KeyCode.W))
            {
                var direction = transform.forward;
                Move(direction);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                var direction = -transform.right;
                Move(direction);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                var direction = -transform.forward;
                Move(direction);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                var direction = transform.right;
                Move(direction);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                targetForward = -transform.right;
                fsm.PopState();
                fsm.PushState(turning);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                targetForward = transform.right;
                fsm.PopState();
                fsm.PushState(turning);
            }
        };
    }

    private void CreateMovingState()
    {
        moving = (finiteStateMachine, gameObj) =>
        {
            var rounded = Vector3Int.RoundToInt(targetPosition);
            
            var delta = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, rounded, delta);
            if (Vector3.Distance(transform.position, rounded) < snapDistance)
            {
                transform.position = rounded;
                fsm.PopState();
                fsm.PushState(idle);
            }
        };
    }
    
    private void CreateTurningState()
    {
        turning = (finiteStateMachine, gameObj) =>
        {
            var delta = rotationSpeed * Time.deltaTime;
            var targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, delta);
            if (Vector3.Angle(transform.forward, targetForward) < snapAngle)
            {
                transform.forward = targetForward;
                fsm.PopState();
                fsm.PushState(idle);
            } 
        };
    }
}
