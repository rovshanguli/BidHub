using BidHub.Services;
using Microsoft.AspNetCore.SignalR;

namespace BidHub.Hubs;

public class BidHub : Hub
{
    private readonly ILogger<BidHub> _logger;
    private readonly IHubContext<BidHub> _hubContext;
    public static List<BidModel> Bids { get; set; } = new();
    int StartPrice = 500;
    int CurrentPrice = 500;
    string LastBidUser = "";
    private readonly ITimerService _timerService;
    static int UserCount = 0;
    CancellationTokenSource _tokenSource = null;

    public BidHub(ILogger<BidHub> logger, IHubContext<BidHub> hubContext, ITimerService timerService)
    {
        _timerService = timerService;
        _logger = logger;
        _hubContext = hubContext;
        _timerService = timerService;
        _timerService.TimerEndEvent += async (sender, e) => await NotifyAllMembersEndLot();
        _timerService.TimerTickEvent += async (sender, e) => await NotifyAllMembersTime(e);

    }


    public async Task AddUserToGroup()
    {
        UserCount++;


        if (UserCount > 0)
        {
            _timerService.StartTimer();
        }

        int minute = 40;
        var prevDate = new DateTime(2021, 7, 15);

        DateTime dateTime = DateTime.UtcNow;
        var diffOfDates = dateTime - prevDate;

        Console.WriteLine("Difference in Miniutes: {0}", diffOfDates.Minutes);


        foreach (var item in Bids)
        {
            CurrentPrice += item.BidAmount;
        }

        JoinBidModel joinBidModel = new JoinBidModel()
        {
            Bids = Bids,
            Second = 360,
            ConnectionId = Context.ConnectionId,
            CurrentPrice = CurrentPrice,
            StartPrice = StartPrice
        };

        await Clients.All.SendAsync("Connect", joinBidModel);
    }



    public async Task ChangeBid(string user, int bid)
    {
        LastBidUser = user;
        _timerService.RestartTimer();

        foreach (var item in Bids)
        {
            CurrentPrice += item.BidAmount;
        }

        BidModel bidModel = new BidModel()
        {
            User = user,
            BidAmount = bid,
            CurrentPrice = CurrentPrice + bid
        };

        Bids.Add(bidModel);


        CurrentPrice = CurrentPrice + bid;

        await Clients.All.SendAsync("ReciveChange", user, CurrentPrice, Bids, 120);

    }


    private async Task NotifyAllMembersTime(TimeData data)
    {
        _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;
        _logger.LogInformation("timera: " + data.Seconds);
        await _hubContext.Clients.All.SendAsync("countSecodns", "countSecodns", data.Seconds);
    }


    private async Task NotifyAllMembersEndLot()
    {
        _logger.LogInformation("ended");
        await _hubContext.Clients.All.SendAsync("EndOfBid", "EndOfBid", LastBidUser);
    }



}

public class BidModel
{
    public string? User { get; set; }
    public int BidAmount { get; set; }
    public int CurrentPrice { get; set; }
}



public class JoinBidModel
{
    public List<BidModel>? Bids { get; set; }
    public int Second { get; set; }

    public string? ConnectionId { get; set; }
    public int CurrentPrice { get; set; }
    public int StartPrice { get; set; }
}