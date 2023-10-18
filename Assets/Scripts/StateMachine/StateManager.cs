using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> CurrentState;
    protected bool isTransitioningState = false;

    private void Start()
    {
        CurrentState.EnterState();
    }
    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (isTransitioningState && nextStateKey.Equals(CurrentState.StateKey)){
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState(nextStateKey);
        }
    }

    private void TransitionToState(EState nextStateKey)
    {
        isTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[nextStateKey];
        CurrentState.EnterState();
        isTransitioningState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }
    private void OnTriggerExit(Collider other) 
    {
        CurrentState.OnTriggerExit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }
}
