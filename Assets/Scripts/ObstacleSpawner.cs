using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform player;

    public float laneDistance = 2.5f;
    public float spawnDistanceAhead = 35f;
    public float spawnInterval = 1.5f;

    private float timer;
    private int lastLane = 99;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        int lane;

        do
        {
            lane = Random.Range(-1, 2);    
        } while (lane == lastLane);

        lastLane = lane;

        float xPos = lane * laneDistance;
        float zPos = player.position.z + spawnDistanceAhead;

        Vector3 spawnPos = new Vector3(xPos, 1f, zPos);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        DestroyBehindPlayer cleaner = obstacle.GetComponent<DestroyBehindPlayer>();
        if (cleaner != null)
            cleaner.player = player;
    }
}
