using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8;
    [SerializeField] private float snapDistance = 0.01f;

    private Vector3 targetPosition;
    
    private readonly FSM fsm = new();
    private FSM.State idle, moving;
    
    private void OnEnable()
    {
        EventSystem.AddListener(EventTypes.endTurn, OnTurnEnd);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventTypes.endTurn, OnTurnEnd);
    }

    private void Start()
    {
        CreateMovingState();
    }

    private void Update()
    {
        fsm?.Update(gameObject);
    }

    private void OnTurnEnd()
    {
        targetPosition = transform.position + transform.forward * 2;
        fsm.PushState(moving);
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
            }
        };
    }
}
