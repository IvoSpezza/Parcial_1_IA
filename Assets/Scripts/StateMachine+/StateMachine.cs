using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private State _currentState;

    private Dictionary<Enum, State> _states;
    public StateMachine()
    {

    }

    public void MachineUpdate()
    {
        _currentState.Update();
    }
    public void AddState(State state, Enum key)
    {
        _states.Add(key, state);
    }
    public void ChangeState(Enum key)
    {
        _currentState?.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}
