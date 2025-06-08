using System.Collections.Generic;
using UnityEngine;

public class SpawningSystem : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int minSpawnCount = 3;
    public int maxSpawnCount = 10;
    public Vector2 spawnAreaMin = new Vector2(-10, -10);
    public Vector2 spawnAreaMax = new Vector2(10, 10);
    public float minDistanceBetweenSpawns = 2f; // Minimum distance between spawned objects
     MonsterAI2D templateAI; // Reference to the MonsterAI2D component

    void Start()
    {
        SpawnRandomObjects();
        templateAI = FindFirstObjectByType<MonsterAI2D>().gameObject.GetComponent<MonsterAI2D>();
    }

    public void SpawnRandomObjects()
    {
        // Load preferences from the attached GameObject (objectToSpawn)
        //  templateAI = objectToSpawn.GetComponent<MonsterAI2D>();
        float templateHealth = 0f;
        float templateMoveSpeed = 0f;
        float templateAttackDamage = 0f;

        if (templateAI != null && templateAI.monsterStats != null)
        {
            templateHealth = templateAI.monsterStats.Health;
            templateMoveSpeed = templateAI.monsterStats.MoveSpeed;
            templateAttackDamage = templateAI.monsterStats.AttackDamage;
        }

        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPosition;
            int attempts = 0;
            bool positionValid = false;

            do
            {
                randomPosition = new Vector2(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                positionValid = true;
                foreach (Vector2 pos in spawnPositions)
                {
                    if (Vector2.Distance(randomPosition, pos) < minDistanceBetweenSpawns)
                    {
                        positionValid = false;
                        break;
                    }
                }
                attempts++;
            }
            while (!positionValid && attempts < 50);

            if (positionValid)
            {
                spawnPositions.Add(randomPosition);
                GameObject spawned = Instantiate(objectToSpawn, randomPosition, Quaternion.identity, transform);

                // Inherit attributes from loaded preferences
                MonsterAI2D spawnedAI = spawned.GetComponent<MonsterAI2D>();
                if (spawnedAI != null && spawnedAI.monsterStats != null)
                {
                    spawnedAI.monsterStats.Health = templateHealth;
                    spawnedAI.monsterStats.MoveSpeed = templateMoveSpeed;
                    spawnedAI.monsterStats.AttackDamage = templateAttackDamage;
                    // Add more attributes as needed
                }
            }
        }
    }
}
