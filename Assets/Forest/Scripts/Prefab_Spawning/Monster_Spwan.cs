using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Spwan : MonoBehaviour {
    public GameObject monster;                // The enemy prefab to be spawned.
    public float spawnTime = 3f;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    public int monsterNumbers;
    int spawnPointIndex = 0;

	// Use this for initialization
    void Start()
    {
        
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        // If the player has no health left...
        if (spawnPointIndex == monsterNumbers)
        {
            // ... exit the function.
            return;
        }

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(monster, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        // Find a random index between zero and one less than the number of spawn points.
        spawnPointIndex++;

    }
}
