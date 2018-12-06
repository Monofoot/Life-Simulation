using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class releaseFish : MonoBehaviour {

    public float timeToRelease = 9.0f;
    public float time = 0.0f;

    // Hold the prefab variables publically so we can change it on the fly.
    // Let's actually make this a list of game objects - we can use
    // more than one fish model then. That might be fun.
    // We'll have to completely ignore it when it comes to them mating, but hey ho.
    // It would be interesting if we could make them only mate their own prefab....
    List<GameObject> fishList = new List<GameObject>();
    public GameObject fishPrefab;
    public GameObject fishPrefab1;
    public GameObject fishPrefab2;
    public GameObject fishPrefab3;
    public GameObject fishPrefab4;
    public GameObject fishPrefab5;

    void instantiateFish()
    {
        // Add the fish prefabs to the fish list.
        fishList.Add(fishPrefab);
        fishList.Add(fishPrefab1);
        fishList.Add(fishPrefab2);
        fishList.Add(fishPrefab3);
        fishList.Add(fishPrefab4);
        fishList.Add(fishPrefab5);

        int fishPrefabIndex = UnityEngine.Random.Range(0, 6);
        // Set the position of the new fish to a random range between x, y and z.
        GameObject fish = Instantiate(fishList[fishPrefabIndex], this.transform.position, Quaternion.identity);
        // The fish also need random degrees of rotation along the y and z axis.
        // The prefab for the fish requires the x axis to be rotated by -90 degress at
        // instantiation.
        // fish.transform.Rotate(-90.0f, Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));

    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        // In 9 seconds, create the fish:
        time += Time.deltaTime;
        if (time >= timeToRelease)
        {
            // Reset time to 0.
            time = 0.0f;
            instantiateFish();
        }

    }
}
