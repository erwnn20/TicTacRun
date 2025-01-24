using UnityEngine;
using TMPro;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public float timeRemaining = 10f;
    public TextMeshProUGUI timerText;
    public AudioSource tickSound;
    public AudioSource timeAddedSound;
    public GameObject timeAddedVisual;
    public CanvasGroup timeAddedCanvasGroup;
    public float countdownDuration = 3f;

    private bool isGameOver = false;
    private bool isCountdownActive = false;
    private float previousTime;

    void Start()
    {
        previousTime = Mathf.Floor(timeRemaining);
        if (timeAddedVisual != null)
        {
            timeAddedVisual.SetActive(false);
        }
        if (timeAddedCanvasGroup != null)
        {
            timeAddedCanvasGroup.alpha = 0f;
        }

        StartCoroutine(Countdown());
    }

    void Update()
    {
        if (!isGameOver && !isCountdownActive)
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
        ShowTimeAddedVisual();
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

    private void ShowTimeAddedVisual()
    {
        if (timeAddedVisual != null && timeAddedCanvasGroup != null)
        {
            timeAddedVisual.SetActive(true);
            PlayTimeAddedSound();
            StartCoroutine(FadeInOut());
        }
    }

    private void PlayTimeAddedSound()
    {
        if (timeAddedSound != null)
        {
            timeAddedSound.Play();
        }
    }

    private IEnumerator FadeInOut()
    {
        float fadeDuration = 0.5f;
        float displayDuration = 0.5f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            timeAddedCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            timeAddedCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        timeAddedVisual.SetActive(false);
    }

    private IEnumerator Countdown()
    {
        isCountdownActive = true;
        InputBlocker.isCountdownActive = true;
        float countdownTime = countdownDuration;

        while (countdownTime > 0)
        {
            timerText.text = $"{Mathf.Ceil(countdownTime)}";
            yield return new WaitForSeconds(1f);
            countdownTime -= 1f;
        }

        timerText.text = "Go!";
        yield return new WaitForSeconds(1f);

        isCountdownActive = false;
        InputBlocker.isCountdownActive = false;
        timerText.text = $"{timeRemaining:F1}";
    }
}
