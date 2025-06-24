using System;

public enum ETimerType
{
    None = 0,

    GameTime = 1,
    RealTime = 2,
}

public class Timer
{
    public int ID;
    public ETimerType TimerType;
    public bool IsAccumulative;
    public bool IsPaused;
    public long OriginalTimeMs;
    public long CurrentTimeMs;
    public long Multiplier;
    public Action<Timer> ActionOnExpire = null;

    private bool _isMultiplied = false;

    public static implicit operator long(Timer t) => (long)(t.CurrentTimeMs / Constant.Time.MS_DIVIDER_LONG);
    public static implicit operator float(Timer t) => t.CurrentTimeMs / Constant.Time.MS_DIVIDER;
    public static implicit operator double(Timer t) => t.CurrentTimeMs / Constant.Time.MS_DIVIDER;

    private static int IGUIDGenerator = 0;

    public Timer()
    {
        ResetTimer();
    }

    private int ResetTimer()
    {
        ID = ++IGUIDGenerator;

        TimerType = ETimerType.None;
        IsAccumulative = false;
        IsPaused = false;
        OriginalTimeMs = 0;
        CurrentTimeMs = 0;
        Multiplier = Constant.FloatingPoint.FLOATING_POINT_MULTIPLIER;
        ActionOnExpire = null;
        _isMultiplied = false;

        return ID;
    }

    public void SetTimer(ETimerType timerType, bool isAccumulative, bool isStartWithPaused = false, long originalTimeMs = 0, long multiplier = Constant.FloatingPoint.FLOATING_POINT_MULTIPLIER, Action<Timer> actionOnExpire = null)
    {
        TimerType = timerType;
        IsAccumulative = isAccumulative;
        IsPaused = isStartWithPaused;
        OriginalTimeMs = originalTimeMs;
        Multiplier = multiplier;
        ActionOnExpire = actionOnExpire;
        _isMultiplied = (multiplier != 1);

        if (isAccumulative == false)
        {
            CurrentTimeMs = originalTimeMs;
        }
    }

    public void DisableTimer()
    {
        IsPaused = false;
        OriginalTimeMs = 0;
        CurrentTimeMs = 0;
        ActionOnExpire = null;
    }

    public void UpdateTimer(long tickTimeMs, bool ignoreTimeScale = false)
    {
        if (IsPaused)
        {
            return;
        }

        // Floating Point Calculation Optimization
        if (_isMultiplied && ignoreTimeScale == false)
        {
            if (IsAccumulative)
            {
                CurrentTimeMs += tickTimeMs * Multiplier / Constant.FloatingPoint.FLOATING_POINT_MULTIPLIER;
            }
            else
            {
                CurrentTimeMs -= tickTimeMs * Multiplier / Constant.FloatingPoint.FLOATING_POINT_MULTIPLIER;

                if (IsExpired())
                {
                    ActionOnExpire.RunExt(this);
                }
            }
        }
        else
        {
            if (IsAccumulative)
            {
                CurrentTimeMs += tickTimeMs;
            }
            else
            {
                CurrentTimeMs -= tickTimeMs;

                if (IsExpired())
                {
                    ActionOnExpire.RunExt(this);
                }
            }
        }
    }

    public void SetMultiplier(long multiplier)
    {
        Multiplier = multiplier;
        _isMultiplied = (multiplier != Constant.FloatingPoint.FLOATING_POINT_MULTIPLIER);
    }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }

    public void ChangeCurrentTime(long newCurrentTimeMs)
    {
        CurrentTimeMs = newCurrentTimeMs;
    }

    public void AddCurrentTime(long currentTimeAddingAmount)
    {
        CurrentTimeMs += currentTimeAddingAmount;
    }

    public void ResetCurrentTime()
    {
        ChangeCurrentTime(0);
    }

    public bool IsExpired()
    {
        if (IsPaused)
        {
            return false;
        }

        if (IsAccumulative)
        {
            return false;
        }

        return CurrentTimeMs <= 0;
    }
}