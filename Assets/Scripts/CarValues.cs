using UnityEngine;

[CreateAssetMenu(fileName = "CarValues", menuName = "Scriptable Objects/CarValues")]
public class CarValues : ScriptableObject
{
    public float maxSpeed = 250f;
    public float maxMotorTorque = 1250f;
    public float maxBrakePower = 1500f;
    public AnimationCurve accelerationCurve;
    public float maxTrunAxis = 20f;
}