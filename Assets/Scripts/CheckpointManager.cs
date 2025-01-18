using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private int nextCheckpointIndex = 0;
    private int totalCheckpoints;

    void Start()
    {
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        totalCheckpoints = checkpoints.Length;
    }

    public bool ValidateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == nextCheckpointIndex)
        {
            nextCheckpointIndex++;

            if (nextCheckpointIndex >= totalCheckpoints)
            {
                nextCheckpointIndex = 0;
            }

            return true;
        }

        return false;
    }
}
