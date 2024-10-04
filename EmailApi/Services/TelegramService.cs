using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace TGS.EmailApi.Services
{
    public interface ITelegramService
    {
        Task SendMessageAsync(string message);
    }

    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly string _telegramApiUrl;

        public TelegramService(HttpClient httpClient, string botToken, string chatId)
        {
            _httpClient = httpClient;
            _botToken = botToken ?? throw new ArgumentNullException(nameof(botToken));
            _chatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
            _telegramApiUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";
        }

        public async Task SendMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            var payload = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("chat_id", _chatId),
            new KeyValuePair<string, string>("text", message)
        });

            HttpResponseMessage response = await _httpClient.PostAsync(_telegramApiUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send message.");
            }
        }
    }
}
