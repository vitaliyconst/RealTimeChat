using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Data;
using RealTimeChat.Services;

public class ChatHub : Hub
{
    private readonly ISentimentService _sentimentService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ISentimentService sentimentService, AppDbContext dbContext, ILogger<ChatHub> logger)
    {
        _sentimentService = sentimentService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SendMessage(string user, string message)
    {
        try
        {
            _logger.LogInformation("Received message from {User}: {Message}", user, message);

            // ---Аналіз тональності---
            var sentiment = _sentimentService.AnalyzeSentiment(message);

            // ---Збереження в БД---
            var chatMessage = new Message
            {
                User = user,
                Content = message,
                Sentiment = sentiment,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.Messages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();

            // ---Надсилання клієнтам---
            await Clients.All.SendAsync("ReceiveMessage", user, message, sentiment);

            _logger.LogInformation("Sent message to clients from {User} with sentiment {Sentiment}", user, sentiment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message from {User}", user);
            throw;
        }
    }
}
