﻿using UnityEngine;
using StateMachineInternals;

public class sharkDeadState : State<sharkAI>
{
    // Static variable declared once.
    private static sharkDeadState instance;

    private sharkDeadState()
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
    public static sharkDeadState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new sharkDeadState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    void Die(sharkAI owner)
    {
        // TO-DO: Give the fish anutrient level which increases based on fitness score.
        // Nutriest level remains on the corpse, allowing others to eat the amount of nutrients left.
        // TO-DO: Make the fish rotate to face belly-up.
        // Float to the bottom of the scene.
        owner.sharkRigidBody = owner.GetComponent<Rigidbody>();

        // Set gravity to true, allowing the fish to fall endlessly to it's doom.
        owner.sharkRigidBody.useGravity = true;

        Renderer rend = owner.GetComponent<Renderer>();

        // Set the fish a shade of white. It's dead.
        rend.material.SetColor("_Color", Color.white);

        // Set the shark tag to food. New fodder for the masses.
        owner.gameObject.tag = "food";
    }

    public override void enterState(sharkAI owner)
    {
    }

    public override void exitState(sharkAI owner)
    {
    }

    public override void updateState(sharkAI owner)
    {
        Die(owner);
    }
}