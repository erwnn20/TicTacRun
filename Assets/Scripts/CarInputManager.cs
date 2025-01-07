using UnityEngine;
using UnityEngine.Serialization;

public class CarInputManager : MonoBehaviour
{
    public float throttle;
    public float steer;
    public bool handBrake;

    private void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        handBrake = Input.GetKey(KeyCode.Space);
    }
}
