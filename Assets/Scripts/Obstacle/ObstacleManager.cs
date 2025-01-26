using UnityEngine;

/// <summary>
/// Manages the spawning of obstacles at random positions in the game.
/// Handles spawning obstacles and ensuring that the obstacles are placed correctly.
/// </summary>
public class ObstacleManager : MonoBehaviour
{
    /// <summary>
    /// An array of positions where obstacles can be spawned.
    /// </summary>
    public Transform[] obstaclePositions;

    /// <summary>
    /// The obstacle prefab to be spawned.
    /// </summary>
    public GameObject obstaclePrefab;

    /// <summary>
    /// An array to hold the currently spawned obstacles.
    /// </summary>
    private GameObject[] currentObstacles;

    /// <summary>
    /// Called when the script is initialized. It spawns random obstacles at the start.
    /// </summary>
    void Start()
    {
        SpawnRandomObstacles();
    }

    /// <summary>
    /// Spawns two obstacles at random positions from the available obstacle positions.
    /// If obstacles already exist, they are destroyed before spawning new ones.
    /// </summary>
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
