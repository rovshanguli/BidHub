using Timer = System.Timers;
namespace BidHub.Services;

public class TimeData : EventArgs
{
    public int Seconds { get; private set; }
    public TimeData() { }

    public TimeData(int seconds)
    {
        Seconds = seconds;
    }
}
public interface ITimerService
{
    public EventHandler<TimeData> TimerTickEvent { get; set; }
    public EventHandler TimerEndEvent { get; set; }
    void StartTimer();
    public void RestartTimer();
}
public class TimerService : ITimerService
{
    private readonly Timer.Timer _timer = new Timer.Timer();
    private readonly double INTERVAL = 1000;
    private TimeSpan ElapsTime = new(0, 0, 0, 10, 0);

    public EventHandler TimerEndEvent { get; set; }
    public EventHandler<TimeData> TimerTickEvent { get; set; }

    public TimerService()
    {
        InitTimer();
    }
    private void InitTimer()
    {
        _timer.Interval = INTERVAL;
        _timer.Elapsed += async (sender, e) => await TimerTick();

    }

    private async Task TimerTick()
    {
        if (ElapsTime.TotalSeconds > 0)
        {
            ElapsTime -= new TimeSpan(0, 0, 0, 1);
            TimerTickEvent.Invoke(this, new TimeData(ElapsTime.Seconds));
            return;
        }
        TimerEndEvent.Invoke(this, EventArgs.Empty);
        ClearTimer();
    }

    public void RestartTimer()
    {
        _timer.Stop();
        ElapsTime = new(0, 0, 0, 5, 0);
        _timer.Start();
    }

    public void StartTimer()
    {
        ElapsTime = new(0, 0, 0, 10, 0);
        _timer.Start();
    }

    private void ClearTimer()
    {
        _timer.Stop();
        _timer.Enabled = false;
        _timer.Close();
    }





}