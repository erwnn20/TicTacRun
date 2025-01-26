using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarDisplay : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private Needle needle;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text gear;
    [SerializeField] private TMP_Text rpm;

    private void Update()
    {
        needle.transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(needle.minAngle, needle.maxAngle, car.RpmRatio));
        speed.text = $"{Mathf.RoundToInt(car.rb.linearVelocity.magnitude * 3.6f)}";
        gear.text =
            $"{car.gearState switch { GearState.Neutral => "N", GearState.RunningReverse => "R", _ => car.gearIndex + 1 }}";
        rpm.text = $"{Mathf.RoundToInt(car.rpm)}";
    }
}

[Serializable]
public struct Needle
{
    public Transform transform;
    public float minAngle;
    public float maxAngle;
}