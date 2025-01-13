using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private List<WheelCollider> throttleWheels;
    [SerializeField] private List<WheelCollider> brakeWheels;
    [SerializeField] private List<WheelCollider> steeringWheels;

    [SerializeField] private CarValues data;

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

        steeringWheels.ForEach(wheel => wheel.steerAngle = data.maxTrunAxis * _input.steer);
    }

    private void Throttle(float throttlePercentage)
    {
        var currentSpeed = _rb.linearVelocity.magnitude * 3.6f;
        var speedFactor = Mathf.Clamp01(currentSpeed / data.maxSpeed);
        var powerFactor = data.accelerationCurve.Evaluate(speedFactor);
        var motorTorque = data.maxMotorTorque * powerFactor * throttlePercentage;

        throttleWheels.ForEach(wheel => wheel.motorTorque = motorTorque);
    }

    private void Brake(float brakePercentage) =>
        brakeWheels.ForEach(wheel => wheel.brakeTorque = data.maxBrakePower * brakePercentage);

    private void Deceleration()
    {
        var speed = _rb.linearVelocity.magnitude;
        var speedDirection = IsMovingForward() ? 1 : IsMovingBackward() ? -1 : 0;

        var deceleration = -speedDirection * DecelerationCalculation(speed);
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

    private bool IsMovingForward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => _rb.transform.InverseTransformDirection(_rb.linearVelocity).z < -0.1f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
}