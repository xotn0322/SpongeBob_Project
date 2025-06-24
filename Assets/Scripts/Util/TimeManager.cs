using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour, IEngineComponent
{
    public static TimeManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static TimeManager _instance;

    //private
    private List<Timer> _realTimeTimerList =new ();
    private List<Timer> _gameTimeTimerList = new ();

    private List<Timer> _pendingAddRealTimeTimerList = new ();
    private List<Timer> _pendingAddGameTimeTimerList = new ();

    private bool _timerPaused = false;

    //public

    //function

    public IEngineComponent Init()
    {
        return this;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("TimeManager: Another instance of TimeManager found, destroying this one.");
            Destroy(gameObject); // 중복 인스턴스 파괴
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void Destroy()
    {
        // 인스턴스가 파괴될 때 참조를 제거합니다.
        if (_instance == this)
        {
            _instance = null;
        }
        Destroy(gameObject); // GameObject 자체를 파괴합니다.
    }

    public void FixedUpdate()
    {
        if (_timerPaused)
        {
            return;
        }

        // 실제 FixedUpdate 간격을 밀리초로 계산
        long realTimeTickMs = (long)(Time.fixedDeltaTime * 1000);
        // Time.timeScale의 영향을 받는 게임 시간 틱 계산
        long gameTimeTickMs = (long)(Time.fixedDeltaTime * Time.timeScale * 1000);

        AddPendingAddGameTimeTimer();
        AddPendingAddRealTimeTimer();

        foreach (var timer in _realTimeTimerList)
        {
            // RealTime 타이머는 Time.timeScale에 영향을 받지 않도록 ignoreTimeScale = true
            timer.UpdateTimer(realTimeTickMs, true); 
        }

        foreach (var timer in _gameTimeTimerList)
        {
            // GameTime 타이머는 Time.timeScale에 영향을 받도록 ignoreTimeScale = false (기본값)
            timer.UpdateTimer(gameTimeTickMs); 
        }

        // 만료된 타이머를 효율적으로 제거
        _realTimeTimerList.RemoveAll(timer => timer.IsExpired());
        _gameTimeTimerList.RemoveAll(timer => timer.IsExpired());
    }

    public void ResisterTimer(Timer timer)
    {
        if (timer.TimerType == ETimerType.GameTime)
        {
            _pendingAddGameTimeTimerList.Add(timer);
        }
        else if (timer.TimerType == ETimerType.RealTime)
        {
            _pendingAddRealTimeTimerList.Add(timer);
        }
        else if (timer.TimerType == ETimerType.None)
        {
            Debug.LogError("TimeManager : Error in RegisterTimer. Timer Type is None.");
            return;
        }
    }

    private void AddPendingAddGameTimeTimer()
    {
        foreach (var timer in _pendingAddGameTimeTimerList)
        {
            _gameTimeTimerList.Add(timer);
        }
        _pendingAddGameTimeTimerList.Clear();
    }

    private void AddPendingAddRealTimeTimer()
    {
        foreach (var timer in _pendingAddRealTimeTimerList)
        {
            _realTimeTimerList.Add(timer);
        }
        _pendingAddRealTimeTimerList.Clear();
    }
}