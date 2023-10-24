using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Mover
{
    // Pathfinding Settings
    public float moveSpeed = 5f;
    public Transform transform;
    public Transform targetTransform;

    // Path
    private List<Vector2> vectorPath = null;
    private int currentPathIndex = 0;

    // Variables
    private Pathfinding PF;

    private bool atANode = true;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    private Vector2 targetGridPosition;
    private Vector2 previoustargetGridPosition;


    public Mover(Transform transform, Transform TargetTransform)
    {
        this.transform = transform;
        targetTransform = TargetTransform;

        PF = new Pathfinding();
        getPositions();
        vectorPath = PF.FindPath(currentPosition, targetPosition);
    }
    public void FixedUpdate()
    {
        UpdatePath();
        MoveTowardsNextTarget();
    }

    void getPositions()
    {
        // Get updated position of target and self  
        currentPosition = new Vector2(transform.position.x, transform.position.y);
        targetPosition = new Vector2(targetTransform.position.x, targetTransform.position.y);
        targetGridPosition = PF.GetGrid().GetXY(targetPosition);
    }

    void UpdatePath()
    {
        // Update path if target has moved
        // (but wait until we're at a node to avoid weird looking movement)
        getPositions();

        if (atANode & (targetGridPosition != previoustargetGridPosition))
        {
            atANode = false;
            currentPathIndex = 0;
            previoustargetGridPosition = targetGridPosition;
            vectorPath = PF.FindPath(currentPosition, targetPosition);
        }
    }

    private void MoveTowardsNextTarget()
    {
        if (vectorPath != null)
        {
            Vector2 targetPosition = vectorPath[currentPathIndex];
            float distanceFromTarget = Vector2.Distance(targetPosition, currentPosition);

            if (distanceFromTarget >= moveSpeed * Time.fixedDeltaTime)
            {
                // Move towards targetNode
                Vector2 moveDir = (targetPosition - currentPosition).normalized;
                Vector2 moveVec = moveDir * moveSpeed * Time.fixedDeltaTime;
                transform.position += new Vector3(moveVec.x, moveVec.y, 0);
                atANode = false;
            }
            else
            {
                // Go to next node, if one is availiable 
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
