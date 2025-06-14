using Azure;
using Azure.AI.TextAnalytics;

namespace RealTimeChat.Services
{
    public class SentimentService : ISentimentService
    {
        private readonly TextAnalyticsClient _client;

        public SentimentService(string endpoint, string key)
        {
            _client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        public string AnalyzeSentiment(string text)
        {
            try
            {
                DocumentSentiment result = _client.AnalyzeSentiment(text);
                return result.Sentiment.ToString();
            }
            catch (Exception ex)
            {
                // Fallback in case of errors
                Console.WriteLine($"Sentiment analysis error: {ex.Message}");
                return "Neutral";
            }
        }
    }
}