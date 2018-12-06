using UnityEngine;
using StateMachineInternals;

public class sharkHuntState : State<sharkAI>
{


    // Start time at 0.
    public float time = 0.0f;
    // Interpolate every one second.
    public float interpolationPeriod = 1.0f;
    public float rotationTime = 0.0f;

    // Static variable declared once.
    private static sharkHuntState instance;

    // Constructor.
    private sharkHuntState()
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
    public static sharkHuntState Instance
    {
        get
        {
            // If there isn't an instance,
            if (instance == null)
            {
                // Call our constructor.
                new sharkHuntState();
            }
            // Once an instance exists, return it.
            return instance;
        }
    }

    void hungrySwim(sharkAI owner)
    {
        owner.sharkRigidBody = owner.GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        owner.sharkRigidBody.MovePosition(owner.transform.position + owner.transform.forward * owner.speed);

        // If Transform.rotation.x is ever anything but -90, change it back to -90
        // I can't believe this worked, it's solved days of frustration, I'm over the moon
        // The ONLY downside to this is it ruins the amazing effect I had of a dead fish falling
        // hopelessly with no gravity and ending up on it's side...
        if (owner.transform.eulerAngles.x > 0 || owner.transform.eulerAngles.x < 0)
        {
            float x = 0.0f;
            float y = owner.transform.eulerAngles.y;
            float z = owner.transform.eulerAngles.z;

            Vector3 maintainHeading = new Vector3(x, y, z);

            owner.transform.rotation = Quaternion.Euler(maintainHeading);
        }
    }
    void rotateFish(sharkAI owner)
    {
        owner.sharkRigidBody = owner.GetComponent<Rigidbody>();
        // Use the one vector 3 variable so we have a random range of x, y and z which
        // both correlate to position and rotation.
        // Store 3 floats of random ranges, X, Y and Z.
        float randomRangesY = Random.Range(0.0f, 180.0f);

        // Store these 3 floats in a new vector 3 for both our rotation and position transformations.
        Vector3 newHeading = new Vector3(0.0f, randomRangesY, 0.0f);

        owner.targetRot = (owner.transform.rotation * Quaternion.Euler(newHeading)).eulerAngles;
        if (owner.targetRot == owner.transform.eulerAngles)
            owner.targetRot = new Vector3(0, 0, 0);
    }

    void seekFish(sharkAI owner)
    {
        // if food exists, approachfood
        // if food doesn't exist, swim idly until it does
        owner.sharkRigidBody = owner.GetComponent<Rigidbody>();
        // Start by declaring an array of food.
        GameObject[] fish;
        fish = GameObject.FindGameObjectsWithTag("fish");

        if (fish.Length == 0)
        {
            // THERE IS NO FOOD!!
            hungrySwim(owner);
        }
        else
        {
            huntFish(owner);
        }
    }

    void huntFish(sharkAI owner)
    {
        // Access the array of food. We know there are food because this function
        // was called.
        GameObject[] fish;
        fish = GameObject.FindGameObjectsWithTag("fish");

        // New transform for the food location.
        Transform fishLocation;
        // Set the minimum distance to infinity.
        float distance = Mathf.Infinity;
        // Set the closest food object to null.
        GameObject closestFish = null;
        // Create a new vector 3.
        Vector3 position = owner.transform.position;
        // For each food in the food array.
        foreach (GameObject fishi in fish)
        {
            // Calculate the difference between 
            Vector3 diff = fishi.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestFish = fishi;
                distance = curDistance;
                fishLocation = closestFish.transform;
                
                // Normalize the direction we'll take towards the food location.
                Vector3 direction = fishLocation.position + owner.transform.position;

                owner.transform.LookAt(fishLocation.transform.position);
                // Simply move the fish towards the food.
                owner.transform.position = Vector3.MoveTowards(owner.transform.position, fishLocation.position, owner.speed);
            }
        }
    }

    public void Eat(sharkAI owner)
    {
        owner.hunger = owner.hunger - 30;
    }

    // Regulate time ticks.
    void ticks(sharkAI owner)
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            // Reset time to 0.
            time = 0.0f;
            owner.hunger++;
            owner.energy++;
        }
    }

    public override void enterState(sharkAI owner)
    {
    }

    public override void exitState(sharkAI owner)
    {
    }

    public override void updateState(sharkAI owner)
    {
        ticks(owner);
        // This transformation is necessary for the Lerp movement.
        if (owner.targetRot != new Vector3(0, 0, 0))
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.Euler(owner.targetRot), 3 * Time.deltaTime);
        if (owner.targetRot == owner.transform.eulerAngles)
            owner.targetRot = new Vector3(0, 0, 0);
        seekFish(owner);

        rotationTime += 1 * Time.deltaTime;
        if (rotationTime >= 5)
        {
            rotationTime = 0;
            rotateFish(owner);
        }
        if (owner.transform.eulerAngles.x > 0 || owner.transform.eulerAngles.x < 0)
        {
            float x = owner.transform.eulerAngles.x;
            float y = owner.transform.eulerAngles.y;
            float z = 0.0f;

            Vector3 maintainHeading = new Vector3(x, y, z);

            owner.transform.rotation = Quaternion.Euler(maintainHeading);
        }

        // Maintain an idle swim motion.
        hungrySwim(owner);
    }
}
