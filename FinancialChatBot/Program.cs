﻿using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TinyCsvParser;

namespace FinancialChatBot
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static HubConnection hubConnection;

        static async Task Main(string[] args)
        {
            string uri;
            Uri validatedUri;

            do
            {
                Console.Write("Enter API Url: ");
                uri = Console.ReadLine();
            } while (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out validatedUri));

            var loginBot = await LoginBot(validatedUri);

            hubConnection = CreateConnection(validatedUri, loginBot.Token);

            await ReadMessages();

            Console.ReadLine();
        }        

        private static HubConnection CreateConnection(Uri uri, string token)
        {
            return new HubConnectionBuilder().WithUrl(uri + "/MessageHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            }).Build();
        }

        private static async Task<LoginResult> LoginBot(Uri uri)
        {
            var body = new
            {
                Email = "friendly@financialbot.com",
                Password = "Friendly123!"
            };

            var todoItemJson = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(uri + "/Accounts/Login", todoItemJson);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var loginResult = JsonSerializer.Deserialize<LoginResult>(result, options);

            Console.WriteLine("Token: " + loginResult.Token);
            Console.WriteLine("UserName: " + loginResult.UserName);

            return loginResult;
        }

        private static async Task ReadMessages()
        {
            hubConnection.On<Message>("MessageBot", async (message) =>
            {
                var response = await GetResponseToCommand(message);
                await hubConnection.InvokeAsync("Message", response, cancellationToken: default);
            });

            await hubConnection.StartAsync();
        }

        private static async Task<string> GetResponseToCommand(Message message)
        {
            var messageContent = message.Content.Split(' ');

            switch (messageContent[0])
            {
                case "/stock":
                    return await CallApi(messageContent[1]);
                case "/help":
                default:
                    return "Type /stock <stock_code>";
            }
        }

        private static async Task<string> CallApi(string filename)
        {
            var url = string.Format("https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv", filename);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/csv"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            await using var ms = await response.Content.ReadAsStreamAsync();

            var csvParserOptions = new CsvParserOptions(true, ',');
            var csvMapper = new CsvStockMapping();
            var csvParser = new CsvParser<Stock>(csvParserOptions, csvMapper);

            var details = csvParser.ReadFromStream(ms, Encoding.ASCII)
                            .Select(r => r.Result)
                            .First();

            if (details.Close != "N/D")
            {
                return $"{details.Symbol.ToUpper()} quote is ${details.Close} per share";
            }

            return "Stock code is invalid";                    
        }
    }
}
