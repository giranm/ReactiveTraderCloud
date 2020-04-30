using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Common;
using Adaptive.ReactiveTrader.Contract;
using Adaptive.ReactiveTrader.EventStore.Domain;
using Adaptive.ReactiveTrader.Server.TradeExecution.Domain;
using Serilog;

namespace Adaptive.ReactiveTrader.Server.TradeExecution
{
    public class TradeExecutionEngine : IDisposable
    {
        private readonly IRepository _repository;
        private readonly TradeIdProvider _tradeIdProvider;
        private MockTradeAlertProvider mockTradeAlertProvider;

        public TradeExecutionEngine(IRepository repository, TradeIdProvider tradeIdProvider)
        {
            _repository = repository;
            _tradeIdProvider = tradeIdProvider;
            mockTradeAlertProvider = new MockTradeAlertProvider();
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

                // Fire off mock alerts using helper class.
                mockTradeAlertProvider.MockOutOfMemory();
                mockTradeAlertProvider.MockTradeEngineDown();
                mockTradeAlertProvider.MockTradeLatency();
                mockTradeAlertProvider.MockTradeRejection();
                mockTradeAlertProvider.MockLowTradingVolume();
                System.Environment.Exit(0);
            }
            else
            {
                trade.Complete();
            }
        }
    }
}
