using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class EngineAudio : MonoBehaviour
{
    [SerializeField] private SoundSource runningAudio;
    [SerializeField] private SoundSource idleAudio;
    private CarController _carController;

    private void Start()
    {
        _carController = GetComponent<CarController>();
    }

    private void Update()
    {
        idleAudio.audioSource.volume = Mathf.Lerp(0.05f, idleAudio.maxVolume, Mathf.Clamp01(_carController.RpmRatio));
        runningAudio.audioSource.volume =
            Mathf.Lerp(0.1f, runningAudio.maxVolume, Mathf.Clamp01(_carController.RpmRatio));
        runningAudio.audioSource.pitch =
            Mathf.Lerp(0.3f, runningAudio.maxPitch, Mathf.Clamp01(_carController.RpmRatio));
    }
}

[Serializable]
public struct SoundSource
{
    public AudioSource audioSource;
    public float maxVolume;
    public float maxPitch;
}