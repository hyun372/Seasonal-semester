using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject cubePrefab;
    private GameObject[] cubes;

    private float timer;

    private void Update()
    {
        if(DoSpawn())
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Instantiate(cubePrefab, spawnPoints[randomIndex].position, Quaternion.identity);
                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
    }

    private bool DoSpawn()
    {
        cubes = GameObject.FindGameObjectsWithTag("Cube");
        if (cubes.Length <= 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
