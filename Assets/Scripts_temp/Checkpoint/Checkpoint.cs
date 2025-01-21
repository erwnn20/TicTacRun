using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    public float timeBonus = 10f;

    private CheckpointManager checkpointManager;

    private void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointManager != null)
        {
            if (checkpointManager.ValidateCheckpoint(checkpointIndex))
            {
                FindObjectOfType<TimerManager>().AddTime(timeBonus);
            }
        }
    }
}
