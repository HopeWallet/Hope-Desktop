using System;

/// <summary>
/// Class which represents a countdown timer.
/// </summary>
public class CountdownTimer
{

    private readonly Action<float> onTimeChanged;
    private readonly Action onTimerFinished;
    private readonly float countdownInterval;

    private float countdownTime;
    private bool started;

    /// <summary>
    /// Initializes the countdown timer by assigning all required values.
    /// </summary>
    /// <param name="onTimeChanged"> Action to call each time the time is changed. </param>
    /// <param name="onTimerFinished"> Action to call when the timer has counted down to 0. </param>
    /// <param name="countdownTime"> The amount of time to spend counting down. </param>
    /// <param name="countdownInterval"> The interval to count down by each iteration. </param>
    public CountdownTimer(Action<float> onTimeChanged, Action onTimerFinished, float countdownTime, float countdownInterval)
    {
        this.countdownTime = countdownTime;
        this.countdownInterval = countdownInterval;
        this.onTimeChanged = onTimeChanged;
        this.onTimerFinished = onTimerFinished;
    }

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    public void StartCountdown()
    {
        if (started)
            return;

        started = true;
        Countdown();
    }

    /// <summary>
    /// Counts down the time and executes the required actions when needed.
    /// </summary>
    private void Countdown()
    {
        try { onTimeChanged?.Invoke(countdownTime--); }
        catch { return; }

        CoroutineUtils.ExecuteAfterWait(countdownInterval, () =>
        {
            if (countdownTime <= 0)
            {
                try { onTimerFinished?.Invoke(); }
                catch { }
            }
            else
            {
                Countdown();
            }
        });
    }

}