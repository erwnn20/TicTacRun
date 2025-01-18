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

    private List<Wheel.Object> _wheels;
    private List<Wheel.Object> _throttleWheels;
    private List<Wheel.Object> _brakeWheels;
    private List<Wheel.Object> _steeringWheels;

    [SerializeField] private CarValues data;
    [HideInInspector] public int gearIndex;
    [HideInInspector] public float rpm;
    [HideInInspector] public GearState gearState;

    [HideInInspector] public Rigidbody rb;
    private CarInputManager _input;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _input = GetComponent<CarInputManager>();

        _wheels = GetWheels();
        _throttleWheels = GetThrottleWheels();
        _brakeWheels = GetBrakeWheels();
        _steeringWheels = GetSteeringWheels();
    }

    private void FixedUpdate()
    {
        CalculateRpm(_input.throttle);
        StartCoroutine(GearsCheck(_input.throttle));

        switch (_input.throttle)
        {
            case > 0 when gearState != GearState.RunningReverse:
            case < 0 when gearState == GearState.RunningReverse:
                Throttle(_input.throttle);
                Brake(0);
                break;
            case > 0 when gearState == GearState.RunningReverse:
            case < 0 when gearState != GearState.RunningReverse:
                Throttle(0);
                Brake(Mathf.Abs(_input.throttle));
                break;
            case 0 when IsMoving():
                // Deceleration();
                Throttle(0);
                Brake(0);
                break;
            default:
                Throttle(0);
                Brake(0.001f);
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
                if (throttlePercentage > 0 && !IsMovingBackward() || IsMovingForward())
                    gearState = GearState.Running;
                if (throttlePercentage < 0 && !IsMovingForward() || IsMovingBackward())
                    gearState = GearState.RunningReverse;
                break;
            case GearState.Running when _input.clutch > 0.1f:
                if (rpm > data.increaseGearRPM) StartCoroutine(ChangeGear(1));
                if (rpm < data.decreaseGearRPM) StartCoroutine(ChangeGear(-1));
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

    private void Throttle(float throttlePercentage) => _throttleWheels.ForEach(wheel =>
        wheel.Collider.motorTorque = CalculateTorque() * throttlePercentage);

    private void CalculateRpm(float throttlePercentage)
    {
        if (_input.clutch < 0.1f)
            rpm = Mathf.Lerp(rpm,
                Mathf.Max(data.minRPM, data.maxRPM * Mathf.Abs(throttlePercentage)) + Random.Range(-100f, 100f),
                Time.fixedDeltaTime);
        else
        {
            var wheelRpm = _throttleWheels.Sum(wheel => wheel.Collider.rpm) / _throttleWheels.Count *
                           data.gearRatios[gearIndex] * data.differentialRatio;
            rpm = Mathf.Lerp(rpm, Mathf.Max(data.minRPM, Mathf.Abs(wheelRpm)), Time.fixedDeltaTime * 3);
        }
    }

    private float CalculateTorque() => _input.clutch > 0.1f
        ? data.horsePower * data.horsePowerCurve.Evaluate(rpm / data.maxRPM) / rpm * data.gearRatios[gearIndex] *
          data.differentialRatio * 5252 * _input.clutch
        : 0;
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
                    if (rpm < data.increaseGearRPM || gearIndex >= data.gearRatios.Count - 1)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }

                    break;
                }
                case < 0:
                {
                    yield return new WaitForSeconds(0.1f);
                    if (rpm > data.decreaseGearRPM || gearIndex <= 0)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }

                    break;
                }
            }

            gearState = GearState.Changing;
            yield return new WaitForSeconds(data.changeGearTime);
            if (gearIndex + gearChange >= 0) gearIndex += gearChange;
        }

        gearState = GearState.Neutral;
    }

    private void Brake(float brakePercentage) =>
        _brakeWheels.ForEach(wheel =>
            wheel.Collider.brakeTorque = data.maxBrakePower * brakePercentage);

    private void Steer(float steerPercentage)
    {
        var steerAngle = data.maxTrunAxis * steerPercentage;
        _steeringWheels.ForEach(wheel =>
        {
            wheel.Collider.steerAngle = steerAngle;
            wheel.Model.localRotation = Quaternion.AngleAxis(steerAngle, Vector3.up);
        });
    }

    private void Deceleration()
    {
        var speed = rb.linearVelocity.magnitude;

        var deceleration = -SpeedDirection() * DecelerationCalculation(speed);
        Throttle(deceleration);

        var dragForce = -rb.linearVelocity.normalized * (data.dragCoefficient * speed * speed);
        rb.AddForce(dragForce, ForceMode.Force);

        Brake(0);
    }

    private float DecelerationCalculation(float speed)
    {
        var normalizedSpeed = Mathf.Clamp01(speed / data.maxSpeed);
        return data.decelerationCurve.Evaluate(normalizedSpeed);
    }

    private void WheelsRotation()
    {
        var speed = rb.linearVelocity.magnitude;
        _wheels.ForEach(wheel =>
        {
            wheel.Model.Rotate(Vector3.right *
                               (SpeedDirection() * speed / (2 * Mathf.PI * wheel.Collider.radius)));
        });
    }

    private bool IsMovingForward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z < -0.1f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
    private int SpeedDirection() => IsMovingForward() ? 1 : IsMovingBackward() ? -1 : 0;

    private List<Wheel.Object> GetWheels() => wheels
        .Select(wheel => wheel.GetObject()).ToList();

    private List<Wheel.Object> GetThrottleWheels() => wheels
        .Where(wheel => wheel.drive)
        .Select(wheel => wheel.GetObject()).ToList();

    private List<Wheel.Object> GetBrakeWheels() => wheels
        .Where(wheel => wheel.brake)
        .Select(wheel => wheel.GetObject()).ToList();

    private List<Wheel.Object> GetSteeringWheels() => wheels
        .Where(wheel => wheel.steering)
        .Select(wheel => wheel.GetObject()).ToList();
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
    };

    public struct Object
    {
        public Transform Model;
        public WheelCollider Collider;
    }
}

public enum GearState
{
    Neutral,
    Running,
    RunningReverse,
    CheckingChange,
    Changing,
}