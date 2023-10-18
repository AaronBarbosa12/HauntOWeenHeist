using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyStateMachine : StateManager<EnemyStateMachine.EnemyState>
{
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Pathfinding PF;
    private Transform playerTransform;

    // Pathfinding
    private List<Vector2> vectorPath = null;
    private int currentPathIndex = 0;
    private Vector2 currentPosition;
    private Vector2 playerPosition;
    private Vector2 previousPlayerPosition;
    private bool atANode = false;
    public float playerMovementThreshold = 0.1f;

    public enum EnemyState
    {
        Idle,
        WalkToTeasure
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        PF = new Pathfinding();
        playerTransform = GameObject.Find("Treasure").GetComponent<Transform>();

        getPositions();
        vectorPath = PF.FindPath(currentPosition, playerPosition);
        previousPlayerPosition = playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void getPositions()
    {
        currentPosition = new Vector2(rb.position.x, rb.position.y);
        playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        getPositions();

        /*
        if (atANode & (Vector2.Distance(playerPosition, previousPlayerPosition) > playerMovementThreshold))
        {
            atANode = false;
            currentPathIndex = 0;
            previousPlayerPosition = playerPosition;
            vectorPath = PF.FindPath(currentPosition, playerPosition);
        }
        */
        MoveTowardsPosition();
    }

    private void MoveTowardsPosition()
    {
        // Update path if player has moved
        if (vectorPath != null)
        {
            Vector2 targetPosition = vectorPath[currentPathIndex];

            if (Vector2.Distance(targetPosition, currentPosition) > 0.1f)
            {
                Vector2 moveDir = (targetPosition - currentPosition).normalized;
                rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
                atANode = false;
            }
            else
            {
                currentPathIndex++;
                atANode = true;
                if (currentPathIndex >= vectorPath.Count)
                {
                    vectorPath = null;
                    currentPathIndex = 0;
                }
            }
        }
    }

}
