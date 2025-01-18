using UnityEngine;
using System.Linq;

public class CheckpointManager : MonoBehaviour
{
    private int nextCheckpointIndex = 0;
    private int totalCheckpoints;

    private Checkpoint[] checkpoints;

    void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>()
            .OrderBy(checkpoint => checkpoint.checkpointIndex)
            .ToArray();

        totalCheckpoints = checkpoints.Length;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            SetGlowActive(checkpoints[i], i == nextCheckpointIndex);
        }
    }

    public bool ValidateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == nextCheckpointIndex)
        {
            SetGlowActive(checkpoints[nextCheckpointIndex], false);

            nextCheckpointIndex++;

            if (nextCheckpointIndex >= totalCheckpoints)
            {
                nextCheckpointIndex = 0;
            }

            SetGlowActive(checkpoints[nextCheckpointIndex], true);

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
}
