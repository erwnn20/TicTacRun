using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private CarInputManager input;
    [SerializeField] private List<WheelCollider> throttleWheels;
    [SerializeField] private List<WheelCollider> brakeWheels;
    [SerializeField] private List<WheelCollider> steeringWheels;

    [SerializeField] private float maxMotorTorque = 1250f;
    [SerializeField] private float maxBrakePower = 1500f;

    [SerializeField] private float maxDecelerationTorque = 1000f;
    [SerializeField] private float speedThreshold = 15f;

    [SerializeField] private float maxTrunAxis = 20f;

    private Rigidbody _rb;

    private void Start()
    {
        input = GetComponent<CarInputManager>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // acc se bloque en tournant
        switch (input.throttle)
        {
            case > 0 when IsMovingForward() || !IsMoving():
            case < 0 when IsMovingBackward() || !IsMoving():
                Throttle(input.throttle * maxMotorTorque);
                Brake(0);
                break;
            case > 0:
            case < 0:
                Throttle(0);
                Brake(Mathf.Abs(input.throttle) * maxBrakePower);
                break;
            case 0 when IsMoving():
                Deceleration();
                break;
            default:
                Throttle(0);
                Brake(maxBrakePower * 0.001f);
                break;
        }


        // pas de problems en tournant
        /*throttleWheels.ForEach(
            wheel => wheel.motorTorque = input.throttle > 0
                ? input.throttle * maxMotorTorque
                : 0
        );
        brakeWheels.ForEach(
            wheel => wheel.brakeTorque = input.throttle < 0
                ? Mathf.Abs(input.throttle) * maxBrakeTorque
                : input.throttle == 0 && IsMoving()
                    ? Deceleration()
                    : 0
        );*/

        if (input.handBrake) Brake(maxBrakePower);

        steeringWheels.ForEach(wheel => wheel.steerAngle = maxTrunAxis * input.steer);
    }

    private void Throttle(float motorTorque) => throttleWheels.ForEach(wheel => wheel.motorTorque = motorTorque);
    private void Brake(float brakePower) => brakeWheels.ForEach(wheel => wheel.brakeTorque = brakePower);

    private void Deceleration()
    {
        var speed = _rb.linearVelocity.magnitude;
        var speedDirection = Math.Sign(_rb.linearVelocity.z);

        var deceleration = -speedDirection * DecelerationCalculation(speed);
        Throttle(deceleration);
        Brake(0);
    }

    private float DecelerationCalculation(float speed) =>
        maxDecelerationTorque * Mathf.Clamp01(-0.9f + Mathf.Exp(speed / speedThreshold));

    private bool IsMovingForward() => _rb.linearVelocity.z > 0.15f;
    private bool IsMovingBackward() => _rb.linearVelocity.z < -0.15f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
}