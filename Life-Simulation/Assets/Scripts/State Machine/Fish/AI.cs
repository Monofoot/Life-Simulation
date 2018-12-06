using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachineInternals;

public class AI : MonoBehaviour {

    public GameObject egg;

    // Public variables which hold the fish genes.
    public float energy;
    public float hunger;
    public float speed;
    public float age;
    public float totalAge;

    // Interpolate every x second.
    public float interpolationPeriod = 1.0f;
    public float interpolationPeriodSpawn = 10.0f;

    // Set a new empty vector 3 for the rotations. This is referred to often
    // and globally in many functions.
    public Vector3 targetRot = new Vector3(0, 0, 0);

    public Rigidbody fishRigidBody;

    public stateMachine<AI> stateMachine { get; set; }

    // Function to move the fish to a new position.
    // This is mainly used after the fish dives to eat - it's beneficial
    // to have a function forcing the fish to swim higher, otherwise it'll stay
    // there for the duration of the game.
    void newPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-40.0f, 50.0f), Random.Range(20.0f, 60.0f), Random.Range(-40.0f, 50.0f));
        transform.Rotate(0.0f, 50.0f, Random.Range(0.0f, 180.0f));
        transform.position = Vector3.MoveTowards(transform.position, randomPos, speed);
    }

    // Handle triggers on the monobehaviour script.
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "boundary")
        {
            // 180 the fish by using the global targetRot vector.
            // This prevents unrealistic collisions with the boundary walls.
            targetRot = (transform.rotation * Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f))).eulerAngles;
        }
        if (col.gameObject.tag == "food" && stateMachine.currentState == huntState.Instance)
        {
            huntState.Instance.Eat(this);
            newPosition();
            // Destroy the game object after x seconds.
            Destroy(col.gameObject, Random.Range(5.0f, 30.0f));
        }
        if (col.gameObject.tag == "potentialMate" && stateMachine.currentState == breedState.Instance)
        {
            // Make sure to subtract this immediately - if not, it will cause an engine-breaking
            // recursive spawn. Not fun!
            age = age - 50;
            breedState.Instance.spawnEgg(this);
        }
    }

    private void Start()
    {
        // Create a new instance of state machine with this.
        stateMachine = new stateMachine<AI>(this);
        // Change the state to the idle state - this is the initial form.
        stateMachine.ChangeState(idleState.Instance);

        // Begin with a random speed between 0.1 and 0.2
        speed = Random.Range(0.1f, 0.2f); 

        // Set hunger to be a random number between 0 and 50.
        hunger = Random.Range(0.0f, 50.0f);

        // Set energy to be a random number between 30 and 60.
        energy = Random.Range(30.0f, 60.0f);

        // Set age a random value between 0 and 20.
        age = Random.Range(0.0f, 60.0f);
    }

    private void Update()
    {
        if (stateMachine.currentState == idleState.Instance)
        {
            // If Transform.rotation.x is ever anything but -90, change it back to -90
            // I can't believe this worked, it's solved days of frustration, I'm over the moon
            // The ONLY downside to this is it ruins the amazing effect I had of a dead fish falling
            // hopelessly with no gravity and ending up on it's side...
            if (transform.eulerAngles.x > 270 || transform.eulerAngles.x < 270)
            {
                float x = 270.0f;
                float y = transform.eulerAngles.y;
                float z = transform.eulerAngles.z;

                Vector3 maintainHeading = new Vector3(x, y, z);

                transform.rotation = Quaternion.Euler(maintainHeading);
            }
        }
        if (hunger >= 100)
        {
            stateMachine.ChangeState(deadState.Instance);
        }

        if (hunger >= 70 && hunger < 100)
        {
            stateMachine.ChangeState(huntState.Instance);
        }

        if (hunger <= 69)
        {
            stateMachine.ChangeState(idleState.Instance);
        }
        if (age >= 120)
        {
            stateMachine.ChangeState(breedState.Instance);
        }
        
        stateMachine.Update();

        
    }
}
