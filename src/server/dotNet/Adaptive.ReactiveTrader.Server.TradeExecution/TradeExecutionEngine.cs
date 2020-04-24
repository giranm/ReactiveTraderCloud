using System;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Common;
using Adaptive.ReactiveTrader.Contract;
using Adaptive.ReactiveTrader.EventStore.Domain;
using Adaptive.ReactiveTrader.Server.TradeExecution.Domain;
using Serilog;
using RestSharp;

namespace Adaptive.ReactiveTrader.Server.TradeExecution
{
    public class TradeExecutionEngine : IDisposable
    {
        private readonly IRepository _repository;
        private readonly TradeIdProvider _tradeIdProvider;

        public TradeExecutionEngine(IRepository repository, TradeIdProvider tradeIdProvider)
        {
            _repository = repository;
            _tradeIdProvider = tradeIdProvider;
        }

        public void Dispose()
        {
            Log.Warning("Not disposed.");
        }

        public async Task<ExecuteTradeResponseDto> ExecuteAsync(ExecuteTradeRequestDto request, string user)
        {
            var id = await _tradeIdProvider.GetNextId();
            var tradeDate = DateTimeOffset.UtcNow.Date;

            DateTimeOffset valueDate;
            if (!DateTimeOffset.TryParse(request.ValueDate, out valueDate))
            {
                valueDate = DateTimeOffset.UtcNow.AddWeekDays(2);
            }


            var trade = new Trade(id,
                                  user,
                                  request.CurrencyPair,
                                  request.SpotRate,
                                  tradeDate,
                                  valueDate,
                                  request.Direction,
                                  request.Notional,
                                  request.DealtCurrency);
            await _repository.SaveAsync(trade);

            await ExecuteImpl(trade);

            // We do the saving in two phases here as this gives us the created event emitted when the first save happens, then
            // the completed/rejected event emitted after the actual execution happens, which will be after a slight delay.
            // This gives us a sequence of events that is more like a real world application
            // rather than both events being received on the client almost simultaneously
            await _repository.SaveAsync(trade);

            return new ExecuteTradeResponseDto
            {
                Trade = new TradeDto
                {
                    CurrencyPair = trade.CurrencyPair,
                    Direction = trade.Direction,
                    Notional = trade.Notional,
                    SpotRate = trade.SpotRate,
                    Status = trade.State,
                    TradeDate = tradeDate,
                    ValueDate = valueDate,
                    TradeId = id,
                    TraderName = user,
                    DealtCurrency = trade.DealtCurrency
                }
            };
        }

        private async Task ExecuteImpl(Trade trade)
        {
            switch (trade.CurrencyPair)
            {
                case "EURJPY":
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    break;
                case "GBPUSD":
                    await Task.Delay(TimeSpan.FromSeconds(1.5));
                    break;
                default:
                    await Task.Delay(TimeSpan.FromSeconds(.5));
                    break;
            }

            if (trade.CurrencyPair == "GBPJPY")
            {
                trade.Reject("Execution engine rejected trade");

                // Setup RESTful client for PD API.
                // var client = new RestClient("https://events.pagerduty.com/v2/enqueue");
                // client.Timeout = -1;
                // var request = new RestRequest(Method.POST);
                // request.AddHeader("Content-Type", "application/json");
                // request.AddParameter("application/json", "{\n  \"payload\": {\n    \"summary\": \"Execution Engine Down - Critical\",\n    \"timestamp\": \"2020-04-23T08:42:58.315+0000\",\n    \"source\": \"TradeExecutionEngine\",\n    \"severity\": \"critical\",\n    \"component\": \"payment\",\n    \"group\": \"payment\",\n    \"class\": \"deploy\",\n    \"custom_details\": {\n      \"ping time\": \"1500ms\",\n      \"load avg\": 0.75\n    }\n  },\n  \"routing_key\": \"9018b1dbc0c94794b7546ea222ac6b40\",\n  \"dedup_key\": \"\",\n  \"images\": [\n    {\n      \"src\": \"https://www.pagerduty.com/wp-content/uploads/2016/05/pagerduty-logo-green.png\",\n      \"href\": \"https://example.com/\",\n      \"alt\": \"Example text\"\n    }\n  ],\n  \"links\": [\n    {\n      \"href\": \"https://example.com/\",\n      \"text\": \"Link text\"\n    }\n  ],\n  \"event_action\": \"trigger\",\n  \"client\": \"Sample Monitoring Service\",\n  \"client_url\": \"https://monitoring.example.com\"\n}",  ParameterType.RequestBody);
                // IRestResponse response = client.Execute(request);
                // Console.WriteLine(response.Content);
                // System.Environment.Exit(0);
            }
            else
            {
                trade.Complete();
            }
        }
    }
}
