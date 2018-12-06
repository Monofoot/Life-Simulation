using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachineInternals;

public class sharkAI : MonoBehaviour {

    // Public variables which hold the shark genes.
    public float energy;
    public float hunger;
    public float speed;

    public Rigidbody sharkRigidBody;

    public stateMachine<sharkAI> stateMachine { get; set; }

    // Set a new empty vector 3 for the rotations. This is referred to often
    // and globally in many functions.
    public Vector3 targetRot = new Vector3(0, 0, 0);

    // Function to move the fish to a new position.
    // This is mainly used after the fish dives to eat - it's beneficial
    // to have a function forcing the fish to swim higher, otherwise it'll stay
    // there for the duration of the game.
    void newPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-40.0f, 150.0f), 90.0f, Random.Range(-40.0f, 150.0f));
        transform.Rotate(0.0f, Random.Range(0.0f, 180.0f), 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, randomPos, speed);
    }

    // Handle triggers on the monobehaviour script.
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "boundary")
        {
            // 180 the fish by using the global targetRot vector.
            targetRot = (transform.rotation * Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))).eulerAngles;
        }
        if (col.gameObject.tag == "fish" && stateMachine.currentState == sharkHuntState.Instance)
        {
            sharkHuntState.Instance.Eat(this);
            newPosition();
            // Destroy the game object.
            Destroy(col.gameObject);
        }
    }

        private void Start()
    {
        // Create a new instance of state machine with this.
        stateMachine = new stateMachine<sharkAI>(this);
        // Change the state to the idle state - this is the initial form.
        stateMachine.ChangeState(sharkIdleState.Instance);

        // Begin with a random speed between 0.1 and 0.2
        speed = Random.Range(0.1f, 0.2f);

        // Set hunger to be a random number between 0 and 50.
        hunger = Random.Range(0.0f, 50.0f);

        // Set energy to be a random number between 30 and 60.
        energy = Random.Range(30.0f, 60.0f);
    }

    // Update is called once per frame
    private void Update () {
        if (stateMachine.currentState == sharkIdleState.Instance)
        {
            // If Transform.rotation.x is ever anything but -90, change it back to -90
            // I can't believe this worked, it's solved days of frustration, I'm over the moon
            // The ONLY downside to this is it ruins the amazing effect I had of a dead fish falling
            // hopelessly with no gravity and ending up on it's side...
            if (transform.eulerAngles.z > 0 || transform.eulerAngles.z < 0)
            {
                float x = transform.eulerAngles.x;
                float y = transform.eulerAngles.y;
                float z = 0.0f;

                Vector3 maintainHeading = new Vector3(x, y, z);

                transform.rotation = Quaternion.Euler(maintainHeading);
            }
        }

        if (hunger <= 69)
        {
            stateMachine.ChangeState(sharkIdleState.Instance);
        }

        if (hunger >= 100)
        {
            stateMachine.ChangeState(sharkHuntState.Instance);
        }

        if (hunger >= 150)
        {
            stateMachine.ChangeState(sharkDeadState.Instance);
        }

        stateMachine.Update();

    }
}
