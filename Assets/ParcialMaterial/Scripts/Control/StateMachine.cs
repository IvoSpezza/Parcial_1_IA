using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private CreatureState _currentState;

    private Dictionary<Enum, CreatureState> _states;
    public StateMachine()
    {
        _states = new Dictionary<Enum, CreatureState>();
    }

    public void MachineUpdate()
    {
        _currentState.Update();
    }
    public void AddState(CreatureState state, Enum key)
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
