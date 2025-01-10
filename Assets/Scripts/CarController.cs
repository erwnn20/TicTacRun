using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> throttleWheels;
    [SerializeField] private List<WheelCollider> brakeWheels;
    [SerializeField] private List<WheelCollider> steeringWheels;

    [SerializeField] private float maxMotorTorque = 1250f;
    [SerializeField] private float maxBrakePower = 1500f;

    [SerializeField] private float maxDecelerationTorque = 1000f;
    [SerializeField] private float speedThreshold = 15f;

    [SerializeField] private float maxTrunAxis = 20f;

    private Rigidbody _rb;
    private CarInputManager _input;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<CarInputManager>();
    }

    private void FixedUpdate()
    {
        switch (_input.throttle)
        {
            case > 0 when IsMovingForward() || !IsMoving():
            case < 0 when IsMovingBackward() || !IsMoving():
                Throttle(_input.throttle * maxMotorTorque);
                Brake(0);
                break;
            case > 0:
            case < 0:
                Throttle(0);
                Brake(Mathf.Abs(_input.throttle) * maxBrakePower);
                break;
            case 0 when IsMoving():
                Deceleration();
                break;
            default:
                Throttle(0);
                Brake(maxBrakePower * 0.001f);
                break;
        }

        if (_input.handBrake) Brake(maxBrakePower);

        steeringWheels.ForEach(wheel => wheel.steerAngle = maxTrunAxis * _input.steer);
    }

    private void Throttle(float motorTorque) => throttleWheels.ForEach(wheel => wheel.motorTorque = motorTorque);
    private void Brake(float brakePower) => brakeWheels.ForEach(wheel => wheel.brakeTorque = brakePower);

    private void Deceleration()
    {
        var speed = _rb.linearVelocity.magnitude;
        var speedDirection = IsMovingForward() ? 1 : IsMovingBackward() ? -1 : 0;

        var deceleration = -speedDirection * DecelerationCalculation(speed);
        Throttle(deceleration);
        Brake(0);
    }

    private float DecelerationCalculation(float speed) =>
        maxDecelerationTorque * Mathf.Clamp01(-0.9f + Mathf.Exp(speed / speedThreshold));

    private bool IsMovingForward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z < -0.1f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
}