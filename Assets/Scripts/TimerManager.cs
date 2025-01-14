using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public float timeRemaining = 10f;
    public TextMeshProUGUI timerText;

    public AudioSource tickSound;
    private bool isGameOver = false;

    private float previousTime;

    void Start()
    {
        previousTime = Mathf.Floor(timeRemaining);
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                StopTickSound();
                GameOver();
            }

            if (Mathf.Floor(timeRemaining) < previousTime)
            {
                HandleTickSound();
                previousTime = Mathf.Floor(timeRemaining);
            }

            UpdateTimerDisplay();
        }
    }

    public void AddTime(float timeToAdd)
    {
        timeRemaining += timeToAdd;
        previousTime = Mathf.Floor(timeRemaining);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            
            if (timeRemaining > 10)
            {
                timerText.text = $"{timeRemaining:F0}";
            }
            else
            {
                timerText.text = $"{timeRemaining:F1}";
            }
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
    }

    private void HandleTickSound()
    {
        if (tickSound != null)
        {
            if (timeRemaining <= 4 && !tickSound.isPlaying)
            {
                float dynamicPitch = 1 + (4 - timeRemaining) * 0.25f;
                tickSound.pitch = dynamicPitch;

                tickSound.Play();
            }
            else if (timeRemaining > 4 && tickSound.isPlaying)
            {
                StopTickSound();
            }
        }
    }

    private void StopTickSound()
    {
        if (tickSound != null && tickSound.isPlaying)
        {
            tickSound.Stop();
        }
    }
}
