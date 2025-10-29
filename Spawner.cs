using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs to spawn")]
    public GameObject zombiePrefab;    
    public GameObject spiritPrefab;      //dont know if needed but dragging from hierarchy made it simpler and it seems to spawn them correctly


    [Header("Spawn settings")]
    public float startChance = 0.5f;     //time for player to get ready before spawns
    public float spawnTime = 1.5f;  // every X second another will spawn (going below 2 gets lowkey crazy and ur pc will have to handle that)(mind you each sprite is 500x500 pixel ish)
    public float spiritChance = 0.4f;   // chance to spawn spirit i like to keep it higher on left side bc ur character blocks and player has to predict where eyes will be
    public float offset = 0.4f;    // prevents overlap :)

    void Start()
    {
        StartCoroutine(SpawnLoop()); //plus dont forget the delay we choose
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startChance);

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    void SpawnEnemy()
    {
        
        GameObject prefab = (Random.value < spiritChance) ? spiritPrefab : zombiePrefab; //lowkey used some help for this one but it was real interesting how random was used 
        // I know from previous coursed R
        if (!prefab) return;

        Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * offset; //the offset we chose helps sell it a litl more thoughh

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}
