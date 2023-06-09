using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// x: [-18.34, 18.34]
// y: [-10, 9.98]
// z: -10

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject heartBlockPrefab;
    public float heartSpawnChance = 0.1f;

    [Header("Rates")] public float secondsBetweenSpawn = 1, secondsBetweenAccelerate = 10, secondsBetweenBoss = 10;
    public int blockHealth = 2, reduceDropSpeed = 5;
    [Header("Accelerate")] public float speedIncrease = 0.1f;
    public float secondsBetweenSpawnMultiplier = 0.95f;
    public float blockHealthMultiplier = 1.5f;
    [Header("Block Speed")] public float minSpeed = 10;
    public float maxSpeed = 15;
    [Header("Block Size")] public float minSize = 1f;
    public float maxSize = 4f, bossSize = 6f;

    // [Header("ScreenDimensions")] public float lowX, highX, lowY, highY;

    private float timerSpawn = 0, timerSpeed = 0;
    private PlayerInput player;
    private Movement playerMovement;
    private bool Boss = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerInput>();
        playerMovement = player.GetComponent<Movement>();

        StartCoroutine(Spawn());
        StartCoroutine(BossSpawn());
        StartCoroutine(IncreaseSpawnRate());
    }

    KeyValuePair<Vector2, Vector3> GetRandomPosition(Vector3 centre)
    {
        Vector3 v = Camera.main.ViewportToWorldPoint(Vector3.zero);
        var lowX = v.x;
        var lowY = v.y;
        v = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        var highX = v.x;
        var highY = v.y;

        Vector2 p = Vector2.zero;
        Vector3 dir = Vector3.zero;
        do
        {
            int id = Random.Range(0, 3);
            if (id == 0)
            {
                dir = Vector3.right;
                p = new Vector2(lowX, Random.Range(lowY, highY));
            }

            if (id == 1)
            {
                dir = Vector3.left;
                p = new Vector2(highX, Random.Range(lowY, highY));
            }

            if (id == 2)
            {
                dir = Vector3.down;
                p = new Vector2(Random.Range(lowX, highX), highY);
            }
        } while (Vector2.Distance(p, centre) < 3);

        return new KeyValuePair<Vector2, Vector3>(p, dir);
    }

    // Update is called once per frame
    void Update()
    {
        // if (timerSpeed < secondsBetweenAccelerate)
        //     timerSpeed += Time.deltaTime;
        // else
        // {
        // }
        //
        // if (timerSpawn < secondsBetweenSpawn)
        //     timerSpawn += Time.deltaTime;
        // else
        // {
        // Vector3 v = Camera.main.ViewportToWorldPoint(Vector3.zero);
        // lowX = v.x;
        // lowY = v.y;
        // v = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        // highX = v.x;
        // highY = v.y;
        // Z = v.z;
        //
        //     CreateBlock();
        //
        //     timerSpawn = 0;
        // }
    }

    IEnumerator IncreaseSpawnRate()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenAccelerate);
            minSpeed += speedIncrease;
            maxSpeed += speedIncrease;
            secondsBetweenSpawn *= secondsBetweenSpawnMultiplier;
            blockHealth = Mathf.RoundToInt((blockHealth + 1) * blockHealthMultiplier);
        }
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenSpawn);
            //if (Boss)
            //    continue;
            if (Random.value < heartSpawnChance)
            {
                CreateHeartBlock();
            }
            else
            {
                CreateBlock();
            }
        }
    }

    IEnumerator BossSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenBoss);
            //Boss = true;
            CreateBossBlock();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CreateHeartBlock()
    {
        var (position, dir) = GetRandomPosition(player.transform.position);
        var blockGO = Instantiate(heartBlockPrefab, position, Quaternion.identity);
        blockGO.GetComponent<Health>().SetMaxHealth(blockHealth);
        var blockMover = blockGO.GetComponent<BlockMover>();
        var size = Random.Range(minSize, maxSize);
        blockMover.SetMove(dir, minSpeed - reduceDropSpeed, maxSpeed - reduceDropSpeed, size, size);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CreateBlock()
    {
        var (position, dir) = GetRandomPosition(player.transform.position);
        var blockGO = Instantiate(blockPrefab, position, Quaternion.identity);
        blockGO.GetComponent<Health>().SetMaxHealth(blockHealth);
        var blockMover = blockGO.GetComponent<BlockMover>();
        if (dir == Vector3.down)
            blockMover.SetMove(dir, minSpeed - reduceDropSpeed, maxSpeed - reduceDropSpeed, minSize, maxSize);
        else blockMover.SetMove(dir, minSpeed, maxSpeed, minSize, maxSize);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CreateBossBlock()
    {
        var (position, dir) = GetRandomPosition(player.transform.position);
        var blockGO = Instantiate(blockPrefab, position, Quaternion.identity);
        blockGO.GetComponent<Health>().SetMaxHealth(Mathf.Max(blockHealth * 2, 10));
        var blockMover = blockGO.GetComponent<BlockMover>();
        if (dir == Vector3.down)
            blockMover.SetMove(dir, minSpeed - reduceDropSpeed, maxSpeed - reduceDropSpeed, bossSize, bossSize);
        else blockMover.SetMove(dir, minSpeed, maxSpeed, bossSize, bossSize);
    }


    private void OnDrawGizmos()
    {
        //Camera.main.ViewportToWorldPoint(Vector3.zero);
        Gizmos.DrawWireSphere(Camera.main.ViewportToWorldPoint(Vector3.zero), 3);
        Gizmos.DrawWireSphere(Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)), 3);
        Gizmos.DrawWireSphere(Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)), 3);
        Gizmos.DrawWireSphere(Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)), 3);
    }
}