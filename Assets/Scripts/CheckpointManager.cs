using UnityEngine;
using System.Linq;

public class CheckpointManager : MonoBehaviour
{
    private int nextCheckpointIndex = 0;
    private int totalCheckpoints;
    private int currentTour = 0;

    private Checkpoint[] checkpoints;
    private BoostManager boostManager;
    private ObstacleManager obstacleManager;

    void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>()
            .OrderBy(checkpoint => checkpoint.checkpointIndex)
            .ToArray();

        totalCheckpoints = checkpoints.Length;

        boostManager = FindObjectOfType<BoostManager>();
        obstacleManager = FindObjectOfType<ObstacleManager>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            SetGlowActive(checkpoints[i], i == nextCheckpointIndex);
            SetObjectActive(checkpoints[i], i == nextCheckpointIndex);
        }

        boostManager.SpawnRandomBoost();
        obstacleManager.SpawnRandomObstacles();
    }

    public bool ValidateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == nextCheckpointIndex)
        {
            SetGlowActive(checkpoints[nextCheckpointIndex], false);
            SetObjectActive(checkpoints[nextCheckpointIndex], false);

            nextCheckpointIndex++;

            if (nextCheckpointIndex >= totalCheckpoints)
            {
                nextCheckpointIndex = 0;
                currentTour++;
                boostManager.SpawnRandomBoost();
                obstacleManager.SpawnRandomObstacles();
            }

            SetGlowActive(checkpoints[nextCheckpointIndex], true);
            SetObjectActive(checkpoints[nextCheckpointIndex], true);

            return true;
        }

        return false;
    }

    private void SetGlowActive(Checkpoint checkpoint, bool isActive)
    {
        Transform glow = checkpoint.transform.Find("Glow");
        if (glow != null)
        {
            glow.gameObject.SetActive(isActive);
        }
    }

    private void SetObjectActive(Checkpoint checkpoint, bool isActive)
    {
        Transform obj = checkpoint.transform.Find("Boost");
        if (obj != null)
        {
            obj.gameObject.SetActive(isActive);
        }
    }
}
