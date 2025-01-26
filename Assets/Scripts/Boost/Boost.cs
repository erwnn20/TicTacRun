using UnityEngine;

/// <summary>
/// Represents a boost object in the game that the player can interact with.
/// </summary>
public class Boost : MonoBehaviour
{
    /// <summary>
    /// Reference to the BoostManager that manages boost spawning and collection.
    /// </summary>
    private BoostManager boostManager;

    /// <summary>
    /// Called when the script starts. Finds the BoostManager in the scene.
    /// </summary>
    private void Start()
    {
        boostManager = FindObjectOfType<BoostManager>();
    }

    /// <summary>
    /// Triggered when another collider enters this object's trigger zone.
    /// If the collider belongs to the player, handles boost collection and adds time.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boostManager.OnBoostCollected();
            FindObjectOfType<TimerManager>().AddTime(10f);
        }
    }
}
