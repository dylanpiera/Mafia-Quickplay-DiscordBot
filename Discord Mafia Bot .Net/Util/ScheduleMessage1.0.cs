using System;
using System.Threading; 
using System.Threading.Tasks;
using Discord.Commands;

public class TimerService
{
    private readonly Timer _timer;
    // This example only concerns a single timer.
    // If you would like to have multiple independant timers,
    // you could use a collection such as List<Timer>,
    // or even a Dictionary<string, Timer> to quickly get
    // a specific Timer instance by name.

    public TimerService()
    {
        _timer = new Timer(_ =>
        {
            // 3) Any code you want to periodically run goes here
        },
        null,
        TimeSpan.FromMinutes(10),  // 4) Time that message should fire after bot has started
        TimeSpan.FromMinutes(30)); // 5) Time after which message should repeat (`Timeout.Infinite` for no repeat)
    }

    public void Stop() // 6) Example to make the timer stop running
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Restart() // 7) Example to restart the timer
    {
        _timer.Change(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(30));
    }
}

public class TimerModule : ModuleBase
{
    private readonly TimerService _service;

    public TimerModule(TimerService service) // Make sure to configure your DI with your TimerService instance
    {
        _service = service;
    }

    // Example commands
    [Command("stoptimer")]
    public async Task StopCmd()
    {
        _service.Stop();
        await ReplyAsync("Timer stopped.");
    }

    [Command("starttimer")]
    public async Task RestartCmd()
    {
        _service.Restart();
        await ReplyAsync("Timer (re)started.");
    }
}