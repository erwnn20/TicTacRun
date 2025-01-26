using UnityEngine;

/// <summary>
/// Manages the spawning and handling of boosts in the game.
/// </summary>
public class BoostManager : MonoBehaviour
{
    /// <summary>
    /// Array of potential positions where boosts can spawn.
    /// </summary>
    public Transform[] boostPositions;

    /// <summary>
    /// Prefab of the boost to instantiate.
    /// </summary>
    public GameObject boostPrefab;

    /// <summary>
    /// Reference to the currently active boost in the scene.
    /// </summary>
    private GameObject currentBoost;

    /// <summary>
    /// Called when the script starts. Spawns a boost at a random position.
    /// </summary>
    void Start()
    {
        SpawnRandomBoost();
    }

    /// <summary>
    /// Spawns a boost at a random position from the available positions.
    /// Destroys the previous boost if one exists.
    /// </summary>
    public void SpawnRandomBoost()
    {
        if (currentBoost != null)
        {
            Destroy(currentBoost);
        }

        if (boostPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, boostPositions.Length);
            Vector3 randomPosition = boostPositions[randomIndex].position;
            currentBoost = Instantiate(boostPrefab, randomPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// Called when a boost is collected by the player.
    /// Destroys the active boost and clears its reference.
    /// </summary>
    public void OnBoostCollected()
    {
        if (currentBoost != null)
        {
            Destroy(currentBoost);
            currentBoost = null;
        }
    }
}
