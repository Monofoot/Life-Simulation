using UnityEngine;
using StateMachineInternals;

public class huntState : State<AI>
{

    // Static variable declared once.
    private static huntState instance;

    // Constructor.
    private huntState()
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
    public static huntState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new huntState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    public override void enterState(AI owner)
    {
        Debug.Log("HUNT State entered");
    }

    public override void exitState(AI owner)
    {
        Debug.Log("HUNT State exited");
    }

    public override void updateState(AI owner)
    {
        if (!owner.switchState)
        {
            owner.stateMachine.ChangeState(idleState.Instance);
        }
    }
}
