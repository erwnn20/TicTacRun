using TMPro;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarDisplay : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text gear;
    [SerializeField] private TMP_Text rpm;

    private void Update()
    {
        speed.text = $"{(int)(car.rb.linearVelocity.magnitude * 3.6f)} km/h";
        gear.text =
            $"Gear : {car.gearState switch { GearState.Neutral => "N", GearState.RunningReverse => "R", _ => car.gearIndex + 1 }}";
        rpm.text = $"RPM : {Mathf.RoundToInt(car.rpm)}";
    }
}