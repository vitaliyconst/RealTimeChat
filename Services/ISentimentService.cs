namespace RealTimeChat.Services
{
    public interface ISentimentService
    {
        string AnalyzeSentiment(string text);
    }
}