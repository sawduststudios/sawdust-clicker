using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnsManager : MonoBehaviour
{
    public static RandomSpawnsManager Instance;

    public GameObject SpawnPrefab;
    public List<SORandomBoost> Boosts;
    public List<GameObject> SpawnSpots;

    public float Interval = 5f;
    public float SpawnChance = 0.02f;
    public float ShownFor = 4f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnBoost();
        }
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Interval);
            if (Random.value <= SpawnChance)
            {
                SpawnBoost();
            }
        }
    }

    private void SpawnBoost()
    {
        var spawnSpot = SpawnSpots[Random.Range(0, SpawnSpots.Count)].transform;
        var spawn = Instantiate(SpawnPrefab, spawnSpot.position, Quaternion.identity, spawnSpot);
        var randomBoost = Boosts[Random.Range(0, Boosts.Count)];
        var spawnScript = spawn.GetComponent<RandomBoost>();
        spawnScript.Boost = randomBoost;
        spawnScript.InitDissapear(ShownFor);
        spawnScript.Init();
    }


}
