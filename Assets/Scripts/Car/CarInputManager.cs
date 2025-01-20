using UnityEngine;

public class CarInputManager : MonoBehaviour
{
    public float throttle;
    public float steer;
    public bool handBrake;
    public float clutch;

    private void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
        handBrake = Input.GetKey(KeyCode.Space);
    }
}