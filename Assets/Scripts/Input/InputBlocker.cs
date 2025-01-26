using UnityEngine;

/// <summary>
/// A component that blocks or resets player input during a countdown.
/// It prevents player input unless the "C" key is pressed when the countdown is active.
/// </summary>
public class InputBlocker : MonoBehaviour
{
    /// <summary>
    /// A static boolean indicating if the countdown is currently active.
    /// When active, player input is blocked or reset.
    /// </summary>
    public static bool isCountdownActive = false;

    /// <summary>
    /// Called once per frame to check and manage input during the countdown.
    /// If the countdown is active, it resets input unless the "C" key is pressed.
    /// </summary>
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
