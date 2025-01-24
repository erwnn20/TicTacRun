using UnityEngine;

public class InputBlocker : MonoBehaviour
{
    public static bool isCountdownActive = false;

    void Update()
    {
        if (isCountdownActive)
        {
            if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.C))
            {
            }
            else
            {
                Input.ResetInputAxes();
            }
        }
    }
}
