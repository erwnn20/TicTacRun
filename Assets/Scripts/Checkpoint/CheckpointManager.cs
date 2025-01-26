using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// Manages the checkpoints in the game, ensuring the player progresses through them in order.
/// Handles checkpoint validation, spawning boosts and obstacles, and updating the checkpoint counter.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    /// <summary>
    /// The index of the next checkpoint that the player needs to reach.
    /// </summary>
    private int nextCheckpointIndex = 0;

    /// <summary>
    /// The total number of checkpoints in the game.
    /// </summary>
    private int totalCheckpoints;

    /// <summary>
    /// The current tour the player is on.
    /// </summary>
    private int currentTour = 0;

    /// <summary>
    /// The number of checkpoints the player has passed.
    /// </summary>
    private int checkpointsPassed = 0;

    /// <summary>
    /// Array containing all the checkpoints in the scene, ordered by checkpoint index.
    /// </summary>
    private Checkpoint[] checkpoints;

    /// <summary>
    /// Reference to the BoostManager, which handles the spawning of boosts.
    /// </summary>
    private BoostManager boostManager;

    /// <summary>
    /// Reference to the ObstacleManager, which handles the spawning of obstacles.
    /// </summary>
    private ObstacleManager obstacleManager;

    /// <summary>
    /// Text UI element that displays the current checkpoint count to the player.
    /// </summary>
    public TextMeshProUGUI checkpointCounterText;

    /// <summary>
    /// Called when the script starts. Initializes checkpoints, spawns boosts and obstacles, and updates the checkpoint counter.
    /// </summary>
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

        UpdateCheckpointCounter();
    }

    /// <summary>
    /// Validates that the player has reached the correct checkpoint.
    /// Handles progressing to the next checkpoint, spawning new boosts and obstacles, and updating the checkpoint counter.
    /// </summary>
    /// <param name="checkpointIndex">The index of the checkpoint the player has reached.</param>
    /// <returns>Returns true if the checkpoint was validated, false otherwise.</returns>
    public bool ValidateCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == nextCheckpointIndex)
        {
            SetGlowActive(checkpoints[nextCheckpointIndex], false);
            SetObjectActive(checkpoints[nextCheckpointIndex], false);

            nextCheckpointIndex++;
            checkpointsPassed++;

            if (nextCheckpointIndex >= totalCheckpoints)
            {
                nextCheckpointIndex = 0;
                currentTour++;
                boostManager.SpawnRandomBoost();
                obstacleManager.SpawnRandomObstacles();
            }

            SetGlowActive(checkpoints[nextCheckpointIndex], true);
            SetObjectActive(checkpoints[nextCheckpointIndex], true);

            UpdateCheckpointCounter();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the "Glow3D" child object of a checkpoint to be active or inactive.
    /// </summary>
    /// <param name="checkpoint">The checkpoint whose glow should be toggled.</param>
    /// <param name="isActive">Whether the glow should be active or inactive.</param>
    private void SetGlowActive(Checkpoint checkpoint, bool isActive)
    {
        Transform glow = checkpoint.transform.Find("Glow3D");
        if (glow != null)
        {
            glow.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Sets the "Boost" child object of a checkpoint to be active or inactive.
    /// </summary>
    /// <param name="checkpoint">The checkpoint whose boost should be toggled.</param>
    /// <param name="isActive">Whether the boost should be active or inactive.</param>
    private void SetObjectActive(Checkpoint checkpoint, bool isActive)
    {
        Transform obj = checkpoint.transform.Find("Boost");
        if (obj != null)
        {
            obj.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// Updates the checkpoint counter text displayed on the UI.
    /// </summary>
    private void UpdateCheckpointCounter()
    {
        if (checkpointCounterText != null)
        {
            checkpointCounterText.text = "Goal(s): " + checkpointsPassed;
        }
    }
}
