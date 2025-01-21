using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CarValues", menuName = "Scriptable Objects/CarValues")]
public class CarValues : ScriptableObject
{
    [Header("Motor")] public float horsePower;

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
    [Range(4000, 15000)] public int maxRpm;
    public int MinRpm => (int)(maxRpm * 0.1f);
    public int IncreaseGearRpm => (int)(maxRpm * 0.9f) + Random.Range(-250, 50);
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

    //

    [Header("Wheels"), HideInInspector] public Wheels Wheels;
}

public struct Wheels
{
    public readonly List<Wheel.Object> List;
    public readonly List<Wheel.Object> Throttle;
    public readonly List<Wheel.Object> Brake;
    public readonly List<Wheel.Object> Steering;

    public Wheels(List<Wheel> wheels)
    {
        List = wheels.Select(wheel => wheel.GetObject()).ToList();
        Throttle = wheels.Where(wheel => wheel.drive).Select(wheel => wheel.GetObject()).ToList();
        Brake = wheels.Where(wheel => wheel.brake).Select(wheel => wheel.GetObject()).ToList();
        Steering = wheels.Where(wheel => wheel.steering).Select(wheel => wheel.GetObject()).ToList();
    }
}

[Serializable]
public struct Wheel
{
    public GameObject obj;
    public bool drive;
    public bool brake;
    public bool steering;

    public Object GetObject() => new()
    {
        Model = obj.transform,
        Collider = obj.GetComponent<WheelCollider>(),
        Renderer = obj.GetComponent<MeshRenderer>(),
    };

    public struct Object
    {
        public Transform Model;
        public WheelCollider Collider;
        public MeshRenderer Renderer;
    }
}