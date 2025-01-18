using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarValues", menuName = "Scriptable Objects/CarValues")]
public class CarValues : ScriptableObject
{
    public float maxSpeed = 250f;
    public float maxMotorTorque = 1250f;
    public float maxBrakePower = 1000f;
    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;
    public float dragCoefficient = 0.05f;
    public float maxTrunAxis = 20f;

    public float horsePower;

    public AnimationCurve horsePowerCurve = new(
        new Keyframe(0.0f, 0.0f, 1.25f, 1.25f),
        new Keyframe(0.725f, 0.9f, 0.97f, 0.97f),
        new Keyframe(0.8f, 0.965f, 0.6f, 0.6f),
        new Keyframe(0.9f, 1.0f, -0.065f, -0.065f),
        new Keyframe(0.95f, 0.85f, -3.555f, -3.555f),
        new Keyframe(1.0f, 0.0f, 0.0f, 0.0f)
    );

    public List<float> gearRatios = new() { 3.8f, 2.2f, 1.5f, 1.0f, 0.8f };
    public float differentialRatio = 4;
    [Range(4000, 15000)] public int maxRPM;
    public int minRPM => (int)(maxRPM * 0.1f);
    public int increaseGearRPM => (int)(maxRPM * 0.9f) + Random.Range(-250, 50);
    public int decreaseGearRPM => (int)(maxRPM * 0.25f) + Random.Range(-50, 250);
    public float changeGearTime = 0.5f;
}