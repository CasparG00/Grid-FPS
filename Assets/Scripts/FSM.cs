using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private readonly Stack<State> states = new();
    public delegate void State(FSM fsm, GameObject gameObject); 
    
    public void Update (GameObject gameObject) 
    {
        if (states.Count == 0) return;
        states.Peek()?.Invoke(this, gameObject);
    }

    public void PushState(State state)
    {
        states.Push(state);
    }

    public void PopState()
    {
        states.Pop();
    }
}