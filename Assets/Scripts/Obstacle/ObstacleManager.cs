using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public Transform[] obstaclePositions;
    public GameObject obstaclePrefab;

    private GameObject[] currentObstacles;

    void Start()
    {
        SpawnRandomObstacles();
    }

    public void SpawnRandomObstacles()
    {
        if (currentObstacles != null)
        {
            foreach (var obstacle in currentObstacles)
            {
                Destroy(obstacle);
            }
        }

        currentObstacles = new GameObject[2];

        if (obstaclePositions.Length >= 2)
        {
            int randomIndex1 = Random.Range(0, obstaclePositions.Length);
            int randomIndex2;

            do
            {
                randomIndex2 = Random.Range(0, obstaclePositions.Length);
            } while (randomIndex2 == randomIndex1);

            Vector3 randomPosition1 = obstaclePositions[randomIndex1].position;
            Vector3 randomPosition2 = obstaclePositions[randomIndex2].position;

            currentObstacles[0] = Instantiate(obstaclePrefab, randomPosition1, Quaternion.identity);
            currentObstacles[1] = Instantiate(obstaclePrefab, randomPosition2, Quaternion.identity);
        }
    }
}
