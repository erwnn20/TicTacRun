using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private List<Wheel> wheels;

    private List<Wheel.Object> _wheels;
    private List<Wheel.Object> _throttleWheels;
    private List<Wheel.Object> _brakeWheels;
    private List<Wheel.Object> _steeringWheels;

    [SerializeField] private CarValues data;

    private Rigidbody _rb;
    private CarInputManager _input;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<CarInputManager>();

        _wheels = GetWheels();
        _throttleWheels = GetThrottleWheels();
        _brakeWheels = GetBrakeWheels();
        _steeringWheels = GetSteeringWheels();
    }

    private void FixedUpdate()
    {
        switch (_input.throttle)
        {
            case > 0 when IsMovingForward() || !IsMoving():
            case < 0 when IsMovingBackward() || !IsMoving():
                Throttle(_input.throttle);
                Brake(0);
                break;
            case > 0:
            case < 0:
                Throttle(0);
                Brake(Mathf.Abs(_input.throttle));
                break;
            case 0 when IsMoving():
                Deceleration();
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

    private void Throttle(float throttlePercentage)
    {
        var currentSpeed = _rb.linearVelocity.magnitude * 3.6f;
        var speedFactor = Mathf.Clamp01(currentSpeed / data.maxSpeed);
        var powerFactor = data.accelerationCurve.Evaluate(speedFactor);
        var motorTorque = data.maxMotorTorque * powerFactor * throttlePercentage;

        _throttleWheels.ForEach(wheel => wheel.Collider.motorTorque = motorTorque);
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
        var speed = _rb.linearVelocity.magnitude;

        var deceleration = -SpeedDirection() * DecelerationCalculation(speed);
        Throttle(deceleration);

        var dragForce = -_rb.linearVelocity.normalized * (data.dragCoefficient * speed * speed);
        _rb.AddForce(dragForce, ForceMode.Force);

        Brake(0);
    }

    private float DecelerationCalculation(float speed)
    {
        var normalizedSpeed = Mathf.Clamp01(speed / data.maxSpeed);
        return data.decelerationCurve.Evaluate(normalizedSpeed);
    }

    private void WheelsRotation()
    {
        var speed = _rb.linearVelocity.magnitude;
        _wheels.ForEach(wheel =>
        {
            wheel.Model.Rotate(Vector3.right *
                               (SpeedDirection() * speed / (2 * Mathf.PI * wheel.Collider.radius)));
        });
    }

    private bool IsMovingForward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z < -0.1f;
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