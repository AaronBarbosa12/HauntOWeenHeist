using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyStateMachine : StateManager<EnemyStateMachine.EnemyState>
{
    private Rigidbody2D rb;


    public enum EnemyState
    {
        Move
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        States.Add(EnemyState.Move, new EnemyMoveState(EnemyState.Move, rb));

        // Set the initial state
        CurrentState = States[EnemyState.Move];
        CurrentState.EnterState();
    }
}
