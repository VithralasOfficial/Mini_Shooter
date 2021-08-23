using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWeapons : MonoBehaviour
{
    public Transform[] spawns;
    public GameObject[] objects;
    public int spawnCount = 12;

    int[] whereSpawn;


    // Start is called before the first frame update
    void Start()
    {

        whereSpawn = new int[spawns.Length];
        Spawn();
    }

    private void Spawn()
    {
        int counter = spawnCount;
        while (counter > 0)
        {
            int rand;
            do
            {
                rand = Random.Range(0, spawns.Length);
            } while (whereSpawn[rand] == 1);
            if (counter > spawnCount / 2)
            {
                Instantiate(objects[0], spawns[rand].position, spawns[rand].rotation);
                whereSpawn[rand] = 1;
            }
            else
            {
                Instantiate(objects[1], spawns[rand].position, spawns[rand].rotation);
                whereSpawn[rand] = 1;
            }
            counter--;
        }
    }    
}
