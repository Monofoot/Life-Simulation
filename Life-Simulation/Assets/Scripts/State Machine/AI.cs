using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachineInternals;

public class AI : MonoBehaviour {

    public bool switchState = false;
    public float gameTimer;

    // Set an initial health pool for each fish.
    float health;

    // Set an initial speed for the fish to move.
    float speed = 0.1f;

    // Set an initial hunger integer for the fish which decreases by 1
    // every second.
    public int hunger = 0;

    // Hold a reference to the fish rigid body.
    Rigidbody fishRigidBody;

    // Set a new empty vector 3 for the rotations. This is referred to often
    // and globally in many functions.
    Vector3 targetRot = new Vector3(0, 0, 0);

    // Start by initializing time at 0.
    float time = 0;

    public stateMachine<AI> stateMachine { get; set; }

    void setHealth()
    {
        // Set health by a random integer.

        health = Random.Range(50, 110);
        Debug.Log("My health is: " + health);
    }

    void idleSwim()
    {
        fishRigidBody = GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        fishRigidBody.MovePosition(transform.position - transform.up * speed);

        // While swimming, hunger increases.
        if (Time.time > gameTimer + 1)
        {
            gameTimer = Time.time;
            hunger++;
        }
    }

    void rotateFish()
    {
        fishRigidBody = GetComponent<Rigidbody>();
        // Use the one vector 3 variable so we have a random range of x, y and z which
        // both correlate to position and rotation.
        // Store 3 floats of random ranges, X, Y and Z.
        float randomRangesZ = Random.Range(0.0f, 180.0f);

        // Store these 3 floats in a new vector 3 for both our rotation and position transformations.
        Vector3 newHeading = new Vector3(0.0f, 0.0f, randomRangesZ);

        targetRot = (transform.rotation * Quaternion.Euler(newHeading)).eulerAngles;
        if (targetRot == transform.eulerAngles)
            targetRot = new Vector3(0, 0, 0);
    }

    void hunt()
    {
        fishRigidBody = GetComponent<Rigidbody>();
        // Start by declaring an array of food.
        GameObject[] food;
        food = GameObject.FindGameObjectsWithTag("food");

        if (food.Length == 0)
        {
            Debug.Log("No food found.");
        }
        else
        {
            Debug.Log("Found x amount of food: ");
            print(food.Length);

            // New transform for the food location.
            Transform foodLocation;
            // Set the minimum distance to infinity.
            float distance = Mathf.Infinity;
            // Set the closest game object to null.
            GameObject closestFood = null;
            // Create a new vector 3.
            Vector3 position = transform.position;
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

                    // Normalize the direction we'll take towards the food location.
                    Vector3 direction = (foodLocation.position - transform.position).normalized;

                    // Simply move and rotate the fish towards the food.
                    fishRigidBody.MovePosition(transform.position + direction - transform.up * speed / 30);
                }
            }
        }
    }
    void Eat()
    {
        hunger--;
    }

    void Die()
    {
        // TO-DO: Give the fish anutrient level which increases based on fitness score.
        // Nutriest level remains on the corpse, allowing others to eat the amount of nutrients left.
        // TO-DO: Make the fish rotate to face belly-up.
        // Float to the bottom of the scene.
        fishRigidBody = GetComponent<Rigidbody>();

        // Set gravity to true, allowing the fish to fall endlessly to it's doom.
        fishRigidBody.useGravity = true;

        // TO-DO: 
        // TO-DO: make decompose interact with a fitness function
        float decompose = 10.0f;
        // After x amount of seconds, 
        Destroy(gameObject, decompose);
    }

    // Handle collision triggers.
    void OnTriggerEnter(Collider col)
    {
        // TO-DO: fix this...
        // If the fish hits a boundary:
        if (col.gameObject.tag == "boundary")
        {
            // 180 the fish by using the global targetRot vector.
            targetRot = (transform.rotation * Quaternion.Euler(new Vector3(180, 180, 0))).eulerAngles;
            Debug.Log("Spinning to prevent collision!");
        }
        if (col.gameObject.tag == "food")
        {
            Eat();
            Debug.Log("I'm eating!");
        }
    }

    private void Start()
    {
        setHealth();
        // Create a new instance of state machine with this.
        stateMachine = new stateMachine<AI>(this);
        // Change the state to the idle state - this is the initial form.
        stateMachine.ChangeState(idleState.Instance);
        gameTimer = Time.time;

    }

    private void Update()
    {
        // If we're in the idle state.
        if (stateMachine.currentState == idleState.Instance)
        {
            idleSwim();
            // Use this code if the fish collides with a boundary. This will flip the fish in an ever so
            // elegant 180 and send it back to the slaughter fields.
            if (targetRot != new Vector3(0, 0, 0)) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), 3 * Time.deltaTime);
            if (targetRot == transform.eulerAngles)
                targetRot = new Vector3(0, 0, 0);

            // Every 5 seconds rotate the fish.
            time += 1 * Time.deltaTime;
            if (time >= Random.Range(5.0f, 10.0f))
            {
                time = 0;
                rotateFish();
            }
            Debug.Log("TEST");
        }
        // If we're in the hunting state.
        if (stateMachine.currentState == huntState.Instance)
        {
            hunt();
            Debug.Log("HUNTING");

            if (hunger <= 0)
            {
                stateMachine.ChangeState(idleState.Instance);
            }
        }
        // If we're in the dead state.
        if (stateMachine.currentState == deadState.Instance)
        {
            Die();
            Debug.Log("DEAD.");
        }



        // Hunger passively increases at a pace of 1 per second.
        if (Time.time > gameTimer + 1)
        {
            gameTimer = Time.time;
            health--;
        }

        // If a hunger threshhold is reached, change states to hunting.
        if (hunger >= 100)
        {
            // Switch to hunt state. Fish needs food.
            stateMachine.ChangeState(huntState.Instance);
        }

        if (health <= 0)
        {
            stateMachine.ChangeState(deadState.Instance);
        }

        stateMachine.Update();
    }
}
