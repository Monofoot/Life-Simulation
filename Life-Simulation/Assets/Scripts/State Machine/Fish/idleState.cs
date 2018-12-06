using UnityEngine;
using StateMachineInternals;


public class idleState : State<AI>
{
    // Start time at 0.
    public float time = 0.0f;
    public float rotationTime = 0.0f;
    // Interpolate every one second.
    public float interpolationPeriod = 1.0f;

    // Static variable declared once.
    private static idleState instance;

    // Constructor.
    private idleState()
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

    // Swim idly.
    void idleSwim(AI owner)
    {
        owner.fishRigidBody = owner.GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        owner.fishRigidBody.MovePosition(owner.transform.position - owner.transform.up * owner.speed);
    }

    void rotateFish(AI owner)
    {
        owner.fishRigidBody = owner.GetComponent<Rigidbody>();
        // Use the one vector 3 variable so we have a random range of x, y and z which
        // both correlate to position and rotation.
        // Store 3 floats of random ranges, X, Y and Z.
        float randomRangesZ = Random.Range(0.0f, 180.0f);

        // Store these 3 floats in a new vector 3 for both our rotation and position transformations.
        Vector3 newHeading = new Vector3(0.0f, 0.0f, randomRangesZ);
        
        owner.targetRot = (owner.transform.rotation * Quaternion.Euler(newHeading)).eulerAngles;
        if (owner.targetRot == owner.transform.eulerAngles)
            owner.targetRot = new Vector3(0, 0, 0);

    }

    // Regulate time ticks.
    void ticks(AI owner)
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            // Reset time to 0.
            time = 0.0f;
            owner.hunger++;
            owner.energy++;
            owner.age++;
            owner.totalAge++;
        }
    }

    public override void enterState(AI owner)
    {
    }

    public override void exitState(AI owner)
    {
    }

    public override void updateState(AI owner)
    {
        // While we're idling our energy and hunger is increasing.
        // Energy is great - hunger not so.
        // Roundabout way of saying every second:
        ticks(owner);
        // This transformation is necessary for the Lerp movement.
        if (owner.targetRot != new Vector3(0, 0, 0))
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.Euler(owner.targetRot), 3 * Time.deltaTime);
        if (owner.targetRot == owner.transform.eulerAngles)
            owner.targetRot = new Vector3(0, 0, 0);

        rotationTime += 1 * Time.deltaTime;
        if (rotationTime >= Random.Range(2.0f, 5.0f))
        {
            rotationTime = 0;
            rotateFish(owner);
        }

        // Make sure that when we're idling our tag is always fish.
        owner.gameObject.tag = "fish";

        // Maintain an idle swim motion.
        idleSwim(owner);
    }
}

// rewrite AI, to use an int
// make a tick function, stick in each state