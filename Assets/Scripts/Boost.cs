using UnityEngine;

public class Boost : MonoBehaviour
{
    private BoostManager boostManager;

    private void Start()
    {
        boostManager = FindObjectOfType<BoostManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boostManager.OnBoostCollected();
            FindObjectOfType<TimerManager>().AddTime(10f);
        }
    }
}
