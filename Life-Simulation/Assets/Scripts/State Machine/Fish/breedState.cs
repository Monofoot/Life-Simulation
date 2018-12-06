using UnityEngine;
using StateMachineInternals;

public class breedState : State<AI>
{
    // Start time at 0.
    public float time = 0.0f;
    // Interpolate every one second.
    public float interpolationPeriod = 1.0f;

    // Static variable declared once.
    private static breedState instance;

    // Constructor.
    private breedState()
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
    public static breedState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new breedState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    void identifyPartner(AI owner)
    {
        // Search through array of fish.
        owner.fishRigidBody = owner.GetComponent<Rigidbody>();
        GameObject[] partners;
        partners = GameObject.FindGameObjectsWithTag("potentialMate");

        // Store the transform of the partner.
        Transform partnerLocation;
        // Set the minimum distance to search. Set this to infinity,
        // though it would be interesting to make this a variable
        // at some point.
        float distance = Mathf.Infinity;
        // Set the closest food object to null.
        GameObject selectedPartner = null;
        // Create a new vector 3 to store the position of the current fish.
        Vector3 position = owner.transform.position;
        
        // For each  potential partner in the array:
        foreach (GameObject partner in partners)
        {
            // Calculate the difference between 
            Vector3 diff = partner.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                selectedPartner = partner;
                distance = curDistance;
                partnerLocation = selectedPartner.transform;

                Vector3 direction = partnerLocation.position + owner.transform.position;

                owner.transform.LookAt(partner.transform.position);
                // Simply move the fish towards the food.
                owner.transform.position = Vector3.MoveTowards(owner.transform.position, partnerLocation.position, owner.speed);
            }
        }


        // Identify the one with the strongest "score" <- subject to change
        // Swim towards the fish, collide, spawn an egg.
    }

    public void spawnEgg(AI owner)
    {
        // Spawn a new egg at the fish location. Because gravity is enabled,
        // the fish will fall to the ocean floor quite realistically.
        Vector3 eggPos = owner.transform.position;
        GameObject egg = GameObject.Instantiate(owner.egg, eggPos, Quaternion.identity);

        // After 10 seconds destroy the egg. After 9 seconds a fish will spawn.
        Vector3 fishPos = eggPos;
        GameObject.Destroy(egg, 10.0f);
    }

    // Swim idle
    void SearchingForMate(AI owner)
    {
        // Swim forward. The fish shouldn't be without a partner for so long for this not to be an issue.
        owner.fishRigidBody.MovePosition(owner.transform.position - owner.transform.up * owner.speed);
    }

    // Regulate time ticks.
    void ticks(AI owner)
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            // Reset time to 0.
            owner.hunger++;
            owner.energy--;
            owner.totalAge++;
        }
    }
    // Also enforce a one-time penalty. Love is hard.
    void penalty(AI owner)
    {
        owner.hunger = owner.hunger + 30;
        owner.hunger = owner.energy - 40;
    }

    public override void enterState(AI owner)
    {
        owner.gameObject.tag = "potentialMate";
    }

    public override void exitState(AI owner)
    {
        owner.gameObject.tag = "fish";
    }

    public override void updateState(AI owner)
    {
        identifyPartner(owner);
        SearchingForMate(owner);
        // Clamp the rotation.
        if (owner.transform.eulerAngles.x > 270 || owner.transform.eulerAngles.x < 270)
        {
            float x = 270.0f;
            float y = owner.transform.eulerAngles.y;
            float z = owner.transform.eulerAngles.z;

            Vector3 maintainHeading = new Vector3(x, y, z);

            owner.transform.rotation = Quaternion.Euler(maintainHeading);
        }
    }
}