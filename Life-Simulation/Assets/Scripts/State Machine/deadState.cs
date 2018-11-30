using UnityEngine;
using StateMachineInternals;


public class deadState : State<AI>
{

    // Static variable declared once.
    private static deadState instance;

    // Constructor.
    private deadState()
    {
        // If our state already exists, return null.
        if (instance != null)
        {
            return;
        }

        // Else, set the instance to one instance of the state.
        instance = this;
    }


    // Accessor function.
    public static deadState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new deadState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    public override void enterState(AI owner)
    {
        Debug.Log("DEAD State entered");
    }

    public override void exitState(AI owner)
    {
        Debug.Log("DEAD State exited");
    }

    public override void updateState(AI owner)
    {
        if (owner.switchState)
        {
            owner.stateMachine.ChangeState(idleState.Instance);
        }
    }
}
