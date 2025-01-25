using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CarValues", menuName = "Scriptable Objects/CarValues")]
public class CarValues : ScriptableObject
{
    [Header("Motor")] public float horsePower;

    public AnimationCurve horsePowerCurve = new(
        new Keyframe(0.0f, 0.0f, 1.25f, 1.25f),
        new Keyframe(0.625f, 0.9f, 0.97f, 0.97f),
        new Keyframe(0.7f, 0.965f, 0.6f, 0.6f),
        new Keyframe(0.8f, 1.0f, -0.065f, -0.065f),
        new Keyframe(0.85f, 0.85f, -6.5f, -6.5f),
        new Keyframe(0.9f, 0.0f, 0.0f, 0.0f)
    );

    public List<float> gearRatios = new() { 3.8f, 2.2f, 1.5f, 1.0f, 0.8f };
    public float differentialRatio = 4;
    [Range(4000, 15000)] public int maxRpm;
    public int MinRpm => (int)(maxRpm * 0.1f);
    public int IncreaseGearRpm => (int)(maxRpm * 0.875f) + Random.Range(-250, 50);
    public int DecreaseGearRpm => (int)(maxRpm * 0.25f) + Random.Range(-50, 250);
    public float engineBrakingFactor = 0.1f;
    public float changeGearTime = 0.5f;

    //

    [Header("Brake")] public float maxBrakePower = 1000f;

    //

    [Header("Steering")] public float maxTrunAxis = 20f;

    public AnimationCurve steeringCurve = new(
        new Keyframe(0, 1), // À 0 km/h, braquage maximal (100%)
        new Keyframe(20, 1), // À 20 km/h, toujours 100% du braquage
        new Keyframe(50, 0.7f, -0.01f, -0.01f), // À 50 km/h, réduction à 70%
        new Keyframe(100, 0.3f, -0.004f, -0.004f), // À 100 km/h, réduction à 30%
        new Keyframe(200, 0.1f) // À 200 km/h, réduction à 10%
    );
}