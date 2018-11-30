using UnityEngine;

namespace StateMachineInternals
{
    // Create a class stateMachine of type T.
    public class stateMachine<T>
    {
        // The public class stateMachine has a reference to the abstract class State.
        public State<T> currentState { get; private set; }
        // This is the object that will be using the state machine.
        public T Owner;

        public stateMachine(T o)
        {
            // Parse o as the new owner.
            Owner = o;
            // Set the current state to null. This is a new
            // class and should begin blank.
            currentState = null;
        }

        // This is how the states change. 
        // Take a state object of type T and parse it through newState.
        public void ChangeState(State<T> newState)
        {
            // Check if the current state is not null.
            if (currentState != null)
                // Enter the exit state.
                currentState.exitState(Owner);
            // Set the current state to the new state.
            currentState = newState;
            // Enter the new state.
            currentState.enterState(Owner);
        }

        public void Update()
        {
            // Check if the current state is not null.
            if (currentState != null)
            {
                currentState.updateState(Owner);
            }
        }
    }

    public abstract class State<T>
    {
        public abstract void enterState(T owner);
        public abstract void exitState(T owner);
        public abstract void updateState(T owner);
    }
}
