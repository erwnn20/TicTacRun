using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CarInputManager))]
public class CarController : MonoBehaviour
{
    [SerializeField] private List<Wheel> wheels;
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

        data.Wheels = new Wheels(wheels);
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

    private void Throttle(float throttlePercentage) => data.Wheels.Throttle.ForEach(wheel =>
        wheel.Collider.motorTorque = CalculateTorque() * throttlePercentage);

    private void CalculateRpm(float throttlePercentage)
    {
        if (_input.clutch < 0.1f)
            rpm = Mathf.Lerp(rpm,
                Mathf.Max(data.MinRpm, data.maxRpm * Mathf.Abs(throttlePercentage)) + Random.Range(-100f, 100f),
                Time.fixedDeltaTime);
        else
        {
            var wheelRpm = data.Wheels.Throttle.Average(wheel => wheel.Collider.rpm) *
                           data.gearRatios[gearIndex] * data.differentialRatio;
            rpm = Mathf.Lerp(rpm, Mathf.Max(data.MinRpm, Mathf.Abs(wheelRpm)), Time.fixedDeltaTime * 3);
        }
    }

    private float CalculateTorque() => _input.clutch > 0.1f
        ? data.horsePower * data.horsePowerCurve.Evaluate(rpm / data.maxRpm) / rpm * data.gearRatios[gearIndex] *
          data.differentialRatio * 5252 * _input.clutch
        : 0;

    private void EngineBrake()
    {
        var engineResistance = data.engineBrakingFactor * rpm / data.maxRpm;
        var brakingTorque = SpeedDirection() * engineResistance * data.gearRatios[gearIndex] * data.differentialRatio;

        data.Wheels.Throttle.ForEach(wheel => wheel.Collider.motorTorque = brakingTorque);
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
        data.Wheels.Brake.ForEach(wheel => wheel.Collider.brakeTorque = data.maxBrakePower * brakePercentage);

    private void Steer(float steerPercentage) => data.Wheels.Steering.ForEach(wheel =>
        wheel.Collider.steerAngle = data.maxTrunAxis * data.steeringCurve.Evaluate(rb.linearVelocity.magnitude) *
                                    steerPercentage);

    private void WheelsRotation()
    {
        data.Wheels.List.ForEach(wheel =>
        {
            wheel.Collider.GetWorldPose(out var position, out var rotation);
            wheel.Renderer.transform.rotation = rotation;
        });
    }

    private bool IsMovingForward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z > 0.1f;
    private bool IsMovingBackward() => rb.transform.InverseTransformDirection(rb.linearVelocity).z < -0.1f;
    private bool IsMoving() => IsMovingForward() || IsMovingBackward();
    private int SpeedDirection() => IsMovingForward() ? 1 : IsMovingBackward() ? -1 : 0;
}

public enum GearState
{
    Neutral,
    Running,
    RunningReverse,
    CheckingChange,
    Changing,
}