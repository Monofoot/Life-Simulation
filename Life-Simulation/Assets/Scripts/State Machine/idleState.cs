using UnityEngine;
using StateMachineInternals;


public class idleState : State<AI>
{
    
    // Static variable declared once.
    private static idleState instance;

    // Constructor.
    private idleState()
    {
        // If our state already exists, return null.
        if(instance != null)
        {
            return;
        }

        // Else, set the instance to one instance of the state.
        instance = this;
    }


    // Accessor function.
    public static idleState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new idleState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    public override void enterState(AI owner)
    {
        Debug.Log("IDLE State entered");
    }

    public override void exitState(AI owner)
    {
        Debug.Log("IDLE State exited");
    }

    public override void updateState(AI owner)
    {
        if (owner.switchState)
        {
            owner.stateMachine.ChangeState(huntState.Instance);
        }
    }
}
