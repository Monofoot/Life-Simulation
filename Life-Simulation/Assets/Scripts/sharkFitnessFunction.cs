using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sharkFitnessFunction : MonoBehaviour {

    // Set an initial life span for the fish of x seconds.
    int lifeSpan = 10;
    int fitnessScore = 0;
    float speed = 0.1f;

    Rigidbody sharkRigidBody;

    Vector3 targetRot = new Vector3(0, 0, 0);

    float time = 10;


    void rotateShark()
    {
        sharkRigidBody = GetComponent<Rigidbody>();
        // Use the one vector 3 variable so we have a random range of x, y and z which
        // both correlate to position and rotation.
        // Store 3 floats of random ranges, X, Y and Z.
        float randomRangesY = Random.Range(0.0f, 180.0f);

        // Store these 3 floats in a new vector 3 for both our rotation and position transformations.
        Vector3 newHeading = new Vector3(0.0f, randomRangesY, 0.0f);

        targetRot = (transform.rotation * Quaternion.Euler(newHeading)).eulerAngles;
        if (targetRot == transform.eulerAngles) targetRot = new Vector3(0, 0, 0);
    }

    void idleSwim()
    {
        sharkRigidBody = GetComponent<Rigidbody>();
        // Unfortunately the prefab is set in such a way that we need to 
        // use the inverse of up to make the fish swim forward. Strange, I know.
        sharkRigidBody.MovePosition(transform.position + transform.forward * speed);
    }

    void Update()
    {

        // Use this code if the fish collides with a boundary. This will flip the fish in an ever so
        // elegant 180 and send it back to the slaughter fields.
        if (targetRot != new Vector3(0, 0, 0)) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), 3 * Time.deltaTime);
        if (targetRot == transform.eulerAngles) targetRot = new Vector3(0, 0, 0);

        time += 1 * Time.deltaTime;
        if (time >= 5)
        {
            time = 0;
            rotateShark();
        }

    }

    void FixedUpdate()
    {
        idleSwim();
    }
}
