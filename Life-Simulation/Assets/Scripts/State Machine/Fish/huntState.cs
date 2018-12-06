using UnityEngine;
using StateMachineInternals;

public class huntState : State<AI>
{

    // Static variable declared once.
    private static huntState instance;


    // Start time at 0.
    public float time = 0.0f;
    // Interpolate every one second.
    public float interpolationPeriod = 1.0f;
    public float rotationTime = 0.0f;

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

    void hungrySwim(AI owner)
    {
        owner.fishRigidBody = owner.GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        owner.fishRigidBody.MovePosition(owner.transform.position - owner.transform.up * owner.speed);

        // If Transform.rotation.x is ever anything but -90, change it back to -90
        // I can't believe this worked, it's solved days of frustration, I'm over the moon
        // The ONLY downside to this is it ruins the amazing effect I had of a dead fish falling
        // hopelessly with no gravity and ending up on it's side...
        if (owner.transform.eulerAngles.x > 270 || owner.transform.eulerAngles.x < 270)
        {
            float x = 270.0f;
            float y = owner.transform.eulerAngles.y;
            float z = owner.transform.eulerAngles.z;

            Vector3 maintainHeading = new Vector3(x, y, z);

            owner.transform.rotation = Quaternion.Euler(maintainHeading);
        }
    }
    void hungryRotation(AI owner)
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

    void seekFood(AI owner)
    {
        // if food exists, approachfood
        // if food doesn't exist, swim idly until it does
        owner.fishRigidBody = owner.GetComponent<Rigidbody>();
        // Start by declaring an array of food.
        GameObject[] food;
        food = GameObject.FindGameObjectsWithTag("food");

        if (food.Length == 0)
        {
            // THERE IS NO FOOD!!
            hungrySwim(owner);
        }
        else
        {
            approachFood(owner);
        }
    }

    void approachFood(AI owner)
    {
        // Access the array of food. We know there are food because this function
        // was called.
        GameObject[] food;
        food = GameObject.FindGameObjectsWithTag("food");

        // New transform for the food location.
        Transform foodLocation;
        // Set the minimum distance to infinity.
        float distance = Mathf.Infinity;
        // Set the closest food object to null.
        GameObject closestFood = null;
        // Create a new vector 3.
        Vector3 position = owner.transform.position;
        // For each food in the food array.
        foreach (GameObject foodi in food)
        {
            // Calculate the difference between 
            Vector3 diff = foodi.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestFood = foodi;
                distance = curDistance;
                foodLocation = closestFood.transform;

                // TO-DO: MAKE THIS PRETTIER, GODDAMN.
                // Normalize the direction we'll take towards the food location.
                Vector3 direction = foodLocation.position - owner.transform.position;

                owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.Euler(direction), 3 * Time.deltaTime);
                // Simply move the fish towards the food.
                owner.transform.position = Vector3.MoveTowards(owner.transform.position, foodLocation.position, owner.speed);
            }
        }
    }

    public void Eat(AI owner)
    {
        owner.hunger = owner.hunger - 10;
    }

    void ticks(AI owner)
    {
        time += Time.deltaTime;
        if (time >= interpolationPeriod)
        {
            // Reset time to 0.
            time = 0.0f;
            owner.hunger++;
            owner.energy--;
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
        // This transformation is necessary for the Lerp movement.
        if (owner.targetRot != new Vector3(0, 0, 0))
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, Quaternion.Euler(owner.targetRot), 3 * Time.deltaTime);
        if (owner.targetRot == owner.transform.eulerAngles)
            owner.targetRot = new Vector3(0, 0, 0);
        ticks(owner);
        seekFood(owner);

        rotationTime += 1 * Time.deltaTime;
        if (rotationTime >= 5)
        {
            rotationTime = 0;
            hungryRotation(owner);
        }
    }
}
