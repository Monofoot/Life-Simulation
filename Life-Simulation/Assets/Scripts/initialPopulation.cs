using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// initialPopulation is a script which decides how many of which species the scene should place.
// It's important to get the correct balance here: too many fish, not enough plankton -
// too many predators, not enough fish.
public class initialPopulation : MonoBehaviour {

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

    // Hold the prefab for the fish pellet.
    public GameObject algae;

    // Hold the prefab for the scary sharks.
    public GameObject sharks;

    void instantiateFish(int x)
    {
        // Fish population size.
        int fishPopulationSize = x;

        // Add the fish prefabs to the fish list.
        fishList.Add(fishPrefab);
        fishList.Add(fishPrefab1);
        fishList.Add(fishPrefab2);
        fishList.Add(fishPrefab3);
        fishList.Add(fishPrefab4);
        fishList.Add(fishPrefab5);

        for (int i = 0; i < fishPopulationSize; i++)
        {
            int fishPrefabIndex = UnityEngine.Random.Range(0, 6);
            // Set the position of the new fish to a random range between x, y and z.
            Vector3 fishPos = new Vector3(Random.Range(-40.0f, 150.0f), Random.Range(20.0f, 60.0f), Random.Range(-40.0f, 150.0f));
            GameObject fish = Instantiate(fishList[fishPrefabIndex], fishPos, Quaternion.identity);
            // The fish also need random degrees of rotation along the y and z axis.
            // The prefab for the fish requires the x axis to be rotated by -90 degress at
            // instantiation.
            fish.transform.Rotate(-90.0f, Random.Range(0.0f, 180.0f), Random.Range(0.0f, 180.0f));
        }
    }

    void instantiateAlgae(int x)
    {
        int algaePopulationSize = x;

        for (int i = 0; i < algaePopulationSize; i++)
        {
            // Keep the y axis of fish pellets as 0. The idea is that they fall from the top of the scene to the bottom.
            Vector3 algaePos = new Vector3(Random.Range(-40.0f, 150.0f), 0.0f, Random.Range(-40.0f, 150.0f));
            GameObject Algae = Instantiate(algae, algaePos, Quaternion.identity);

            // Give the pellets some random rotation, just to make them seem alive. This will be updated in the update function as they
            // swivel to the bottom of the tank.
            Algae.transform.Rotate(0.0f, Random.Range(0.0f, 150.0f), 0.0f);
        }
    }

    void instantiateSharks(int x)
    {
        int sharkPopulationSize = x;

        for (int i = 0; i < sharkPopulationSize; i++)
        {
            // Shark behaviour is such that they should begin at the top of the tank so they can catch fish
            // swimming to the top of the tank to eat bait.
            Vector3 sharkPos = new Vector3(Random.Range(-40.0f, 150.0f), 90.0f, Random.Range(-40.0f, 150.0f));
            GameObject shark = Instantiate(sharks, sharkPos, Quaternion.identity);

            shark.transform.Rotate(0.0f, Random.Range(0.0f, 180.0f), 0.0f);
        }
    }

	// Use this for initialization
	void Start () {

        instantiateFish(20);
        instantiateSharks(2);
        instantiateAlgae(200);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
