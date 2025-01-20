using UnityEngine;

public class BoostManager : MonoBehaviour
{
    public Transform[] boostPositions;
    public GameObject boostPrefab;
    public float boostDuration = 10f;

    private GameObject currentBoost;

    void Start()
    {
        SpawnRandomBoost();
    }

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
            Destroy(currentBoost, boostDuration);
        }
    }

    public void OnBoostCollected()
    {
        if (currentBoost != null)
        {
            Destroy(currentBoost);
            currentBoost = null;
        }
    }
}
