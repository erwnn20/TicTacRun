using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private List<Wheel> wheels;
    public CarValues data;

    [HideInInspector] public int gearIndex;
    [HideInInspector] public float rpm;
    [HideInInspector] public GearState gearState;
    [HideInInspector] public Rigidbody rb;
    private CarInputManager _input;

    private float _rpmVelocity;
    private bool _isRevLimiterActive;
    private float _revLimiterCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _input = GetComponent<CarInputManager>();
    }

    private void FixedUpdate()
    {
        CalculateRpm(_input.throttle);
        StartCoroutine(GearsCheck(_input.throttle));

        Throttle(0);
        Brake(0);
        switch (_input.throttle)
        {
            case > 0 when gearState != GearState.RunningReverse:
            case < 0 when gearState == GearState.RunningReverse:
                Throttle(_input.throttle);
                break;
            case > 0 when gearState == GearState.RunningReverse:
            case < 0 when gearState != GearState.RunningReverse:
                Brake(Mathf.Abs(_input.throttle));
                break;
            case 0 when IsMoving():
                EngineBrake();
                break;
        }

        if (_input.handBrake) Brake(1);

        Steer(_input.steer);

        WheelsRotation();
    }

    private IEnumerator GearsCheck(float throttlePercentage)
    {
        switch (gearState)
        {
            case GearState.Changing:
                _input.clutch = 0;
                break;
            case GearState.Neutral:
                _input.clutch = 0;
                if ((throttlePercentage > 0 && !IsMovingBackward()) || IsMovingForward())
                    gearState = GearState.Running;
                if ((throttlePercentage < 0 && !IsMovingForward()) || IsMovingBackward())
                    gearState = GearState.RunningReverse;
                break;
            case GearState.Running when _input.clutch > 0.1f:
                if (rpm > data.IncreaseGearRpm) StartCoroutine(ChangeGear(1));
                if (rpm < data.DecreaseGearRpm)
                {
                    if (gearIndex == 0 && IsMoving()) break;
                    StartCoroutine(ChangeGear(-1));
                }

                break;
            case GearState.RunningReverse:
                yield return new WaitForSeconds(data.changeGearTime);
                if (!IsMovingBackward()) gearState = GearState.Neutral;
                break;
            case GearState.CheckingChange:
            default:
                break;
        }

        if (!IsMovingForward() && gearIndex > 0) StartCoroutine(ChangeGear(-1));
    }

    private void Throttle(float throttlePercentage) => wheels.Throttle().ForEach(
        wheel =>
            wheel.collider.motorTorque = CalculateTorque() * throttlePercentage
    );

    private void CalculateRpm(float throttlePercentage)
    {
        float targetRpm, smoothTime;
        var revLimiterThreshold = data.maxRpm * 0.975f;
        var revLimiterResetThreshold = data.maxRpm * 0.95f;

        if (_input.clutch < 0.1f)
        {
            targetRpm = Mathf.Max(data.MinRpm, data.maxRpm * Mathf.Abs(throttlePercentage)) + Random.Range(-100f, 100f);
            smoothTime = 0.25f;
        }
        else
        {
            var wheelRpm = wheels.Throttle().Average(wheel => wheel.collider.rpm) * data.gearRatios[gearIndex] * data.differentialRatio;
            targetRpm = Mathf.Max(data.MinRpm, Mathf.Abs(wheelRpm));
            if (Mathf.Abs(throttlePercentage) < 0.1f) targetRpm *= 0.98f;
            smoothTime = 0.15f;
        }

        if (rpm >= revLimiterThreshold && !_isRevLimiterActive)
        {
            _isRevLimiterActive = true;
            _revLimiterCooldown = 0.2f;
        }

        if (_isRevLimiterActive)
        {
            _revLimiterCooldown -= Time.fixedDeltaTime;
            targetRpm = data.maxRpm * 0.9f + Random.Range(-50f, 50f);
            if (_revLimiterCooldown <= 0 && rpm < revLimiterResetThreshold) _isRevLimiterActive = false;
        }

        rpm = Mathf.SmoothDamp(rpm, targetRpm, ref _rpmVelocity, smoothTime);
    }

    private float CalculateTorque() => _input.clutch > 0.1f
        ? data.horsePower * data.horsePowerCurve.Evaluate(rpm / data.maxRpm) / rpm * data.gearRatios[gearIndex] *
          data.differentialRatio * 5252 * _input.clutch
        : 0;

    private void EngineBrake()
    {
        var engineResistance = data.engineBrakingFactor * rpm / data.maxRpm;
        var brakingTorque = -SpeedDirection() * engineResistance * data.gearRatios[gearIndex] * data.differentialRatio;

        wheels.Throttle().ForEach(wheel => wheel.collider.motorTorque = brakingTorque);
    }

    private IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if (gearIndex + gearChange >= 0)
        {
            switch (gearChange)
            {
                case > 0:
                {
                    yield return new WaitForSeconds(0.7f);
                    if (rpm < data.IncreaseGearRpm || gearIndex >= data.gearRatios.Count - 1)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }

                    break;
                }
                case < 0:
                {
                    yield return new WaitForSeconds(0.1f);
                    if (rpm > data.DecreaseGearRpm || gearIndex <= 0)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }

                    break;
                }
            }

            gearState = GearState.Changing;
            yield return new WaitForSeconds(data.changeGearTime);
            if (0 <= gearIndex + gearChange && gearIndex + gearChange < data.gearRatios.Count) gearIndex += gearChange;
        }

        gearState = GearState.Neutral;
    }

    private void Brake(float brakePercentage) =>
        wheels.Brake().ForEach(wheel => wheel.collider.brakeTorque = data.maxBrakePower * brakePercentage);

    private void Steer(float steerPercentage) => wheels.Steering().ForEach(
        wheel =>
            wheel.collider.steerAngle =
                data.maxTrunAxis * data.steeringCurve.Evaluate(rb.linearVelocity.magnitude) * steerPercentage
    );

    private void WheelsRotation()
    {
        wheels.ForEach(wheel =>
        {
            wheel.collider.GetWorldPose(out var position, out var rotation);
            wheel.meshRenderer.transform.position = position;
            wheel.meshRenderer.transform.rotation = rotation;
        });
    }

    private bool IsMovingForward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z < -0.1f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
    private int SpeedDirection() => IsMovingForward() ? 1 : IsMovingBackward() ? -1 : 0;

    public float RpmRatio => rpm / data.maxRpm;
}

public enum GearState
{
    Neutral,
    Running,
    RunningReverse,
    CheckingChange,
    Changing,
}

[Serializable]
public struct Wheel
{
    public WheelCollider collider;
    public MeshRenderer meshRenderer;
    public bool drive;
    public bool brake;
    public bool steering;
}

public static class WheelLists
{
    public static List<Wheel> Throttle(this List<Wheel> wheels) => wheels.Where(w => w.drive).ToList();
    public static List<Wheel> Brake(this List<Wheel> wheels) => wheels.Where(w => w.brake).ToList();
    public static List<Wheel> Steering(this List<Wheel> wheels) => wheels.Where(w => w.steering).ToList();
}