using UnityEngine;

/// <summary>
/// Represents a checkpoint in the game that the player can interact with.
/// Manages checkpoint validation and provides a time bonus when the player passes through.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// The unique index of this checkpoint.
    /// Used for validation in the CheckpointManager.
    /// </summary>
    public int checkpointIndex;

    /// <summary>
    /// The amount of time (in seconds) to add to the timer when this checkpoint is activated.
    /// </summary>
    public float timeBonus = 10f;

    /// <summary>
    /// Reference to the CheckpointManager, which handles checkpoint validation and progression.
    /// </summary>
    private CheckpointManager checkpointManager;

    /// <summary>
    /// Called when the script starts. Finds the CheckpointManager in the scene.
    /// </summary>
    private void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    /// <summary>
    /// Triggered when another collider enters this checkpoint's trigger zone.
    /// If the collider is the player and the checkpoint is valid, a time bonus is added.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
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
