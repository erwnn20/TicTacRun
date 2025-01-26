using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the countdown timer for the game, tracks the remaining time,
/// handles time bonuses, and triggers game over when the time runs out.
/// </summary>
public class TimerManager : MonoBehaviour
{
    /// <summary>
    /// The remaining time on the timer.
    /// </summary>
    public float timeRemaining = 10f;

    /// <summary>
    /// The UI text element to display the current time.
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// The audio source for the ticking sound when time is running out.
    /// </summary>
    public AudioSource tickSound;

    /// <summary>
    /// The audio source for the sound when time is added.
    /// </summary>
    public AudioSource timeAddedSound;

    /// <summary>
    /// A visual effect that plays when time is added.
    /// </summary>
    public GameObject timeAddedVisual;

    /// <summary>
    /// The canvas group that controls the fade-in/out of the time-added visual effect.
    /// </summary>
    public CanvasGroup timeAddedCanvasGroup;

    /// <summary>
    /// The duration of the countdown before the game begins.
    /// </summary>
    public float countdownDuration = 3f;

    private bool isGameOver = false;
    private bool isCountdownActive = false;
    private float previousTime;

    /// <summary>
    /// Initializes the timer, sets up visual elements, and starts the countdown.
    /// </summary>
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

    /// <summary>
    /// Updates the remaining time each frame, checks if time is up,
    /// and handles the tick sound and time display updates.
    /// </summary>
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

    /// <summary>
    /// Adds extra time to the timer.
    /// </summary>
    /// <param name="timeToAdd">The amount of time to add to the timer.</param>
    public void AddTime(float timeToAdd)
    {
        timeRemaining += timeToAdd;
        previousTime = Mathf.Floor(timeRemaining);
        ShowTimeAddedVisual();
    }

    /// <summary>
    /// Updates the displayed time on the UI based on the remaining time.
    /// </summary>
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

    /// <summary>
    /// Triggers the game over condition and displays the game over message.
    /// </summary>
    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
    }

    /// <summary>
    /// Handles the ticking sound when the remaining time is less than 4 seconds.
    /// </summary>
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

    /// <summary>
    /// Stops the ticking sound when the time is above 4 seconds or when the game ends.
    /// </summary>
    private void StopTickSound()
    {
        if (tickSound != null && tickSound.isPlaying)
        {
            tickSound.Stop();
        }
    }

    /// <summary>
    /// Shows the visual effect and plays the sound when time is added.
    /// </summary>
    private void ShowTimeAddedVisual()
    {
        if (timeAddedVisual != null && timeAddedCanvasGroup != null)
        {
            timeAddedVisual.SetActive(true);
            PlayTimeAddedSound();
            StartCoroutine(FadeInOut());
        }
    }

    /// <summary>
    /// Plays the sound when time is added to the timer.
    /// </summary>
    private void PlayTimeAddedSound()
    {
        if (timeAddedSound != null)
        {
            timeAddedSound.Play();
        }
    }

    /// <summary>
    /// Fades in and out the time-added visual effect over a short duration.
    /// </summary>
    /// <returns>An enumerator for the fade in and fade out effect.</returns>
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

    /// <summary>
    /// Starts the countdown before the game begins, displaying the countdown timer.
    /// </summary>
    /// <returns>An enumerator for the countdown process.</returns>
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
