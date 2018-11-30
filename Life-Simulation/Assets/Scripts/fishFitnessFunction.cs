using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishFitnessFunction : MonoBehaviour {
    // Set an initial life span for the fish of x seconds.
    int lifeSpan = 10;
    int fitnessScore = 0;
    float speed = 0.10f;

    Rigidbody fishRigidBody;

    Vector3 targetRot = new Vector3(0, 0, 0);

    float time = 5;

    // When a collision is detected, we need to see which collision it was.
    // If it was food, eat the food and increase the fitness score.
    // If it was a shark, turn the other direction.
    void OnTriggerEnter(Collider col)
    {
        // If the fish collides with a pellet, eat it.
        if (col.gameObject.tag == "algae")
        {
            eatFood();
            //transform.position = Vector3.MoveTowards(transform.position, col.gameObject.position, speed);
            // Destroy the pellet.
            Destroy(col.gameObject);
            // Increase the fitness score.
            increaseFitnessScore(1);
        }
        // Also do some boundary checking here.
        // If the fish collider detects the sea wall.
        // Do this for each wall.
        if (col.gameObject.tag == "boundary")
        {
            // 180 the fish by using the global targetRot vector.
            targetRot = (transform.rotation * Quaternion.Euler(new Vector3(180, 180, 0))).eulerAngles;
        }
    }

    // This sensor function casts rays from several directions of a fish.
    // It takes an int x, which is the fitness function, and multiplayed the sensorStrength by the fitness
    // score. Thus as a fish eats more and gets stronger, the range it needs to seek food
    // decreases.
    void Sensor()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit detected;

        int sensorStrength = 30;
        // Does the ray hit algae?
        // Again, because the fish are rotated a strange direction we need the inverse of up.
        if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.up), out detected, sensorStrength, layerMask) && detected.transform.tag == "algae")
        {
            // Head towards the target to eat it.
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, detected.transform.position, step);


            // Swim up and resume idle swimming.
            Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.up) * detected.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.up) * sensorStrength, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    void eatFood()
    {
        // Double the lifespan.
        lifeSpan = lifeSpan + 10;
        // Increase the fitness score by 1.
        increaseFitnessScore(1);
    }

    void increaseFitnessScore(int x)
    {
        // Increase the fitness score by x.
        fitnessScore = fitnessScore + x;
        Debug.Log("My fitness score is: ");
        print(fitnessScore);
        // Grow the sensor collider by a magnitude of x.
    }

    void Die()
    {
        // What would really be fun would be to make the fish flip
        // 180 on the y axis and float to the top as phantom, dead game objects...
        Destroy(gameObject, lifeSpan);
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

    void idleSwim()
    {
        fishRigidBody = GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        fishRigidBody.MovePosition(transform.position - transform.up * speed);
    }

	// Update is called once per frame
	void Update () {

        Sensor();

        // Use this code if the fish collides with a boundary. This will flip the fish in an ever so
        // elegant 180 and send it back to the slaughter fields.
        if (targetRot != new Vector3(0, 0, 0)) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), 3 * Time.deltaTime);
        if (targetRot == transform.eulerAngles)
            targetRot = new Vector3(0, 0, 0);

        // Every 5 seconds rotate the fish.
        time += 1 * Time.deltaTime;
        if (time >= 5)
        {
            time = 0;
            rotateFish();
        }

    }

    void FixedUpdate()
    {
        idleSwim();

    }
}
