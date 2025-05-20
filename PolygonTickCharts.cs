using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolygonTickChartConverter
{
    // Models for Polygon.io API
    public class PolygonTradeResponse
    {
        public string Status { get; set; }
        public string NextUrl { get; set; }
        public string RequestId { get; set; }
        public List<PolygonTrade> Results { get; set; }
    }

    public class PolygonTrade
    {
        public List<int> Conditions { get; set; }
        public int? Correction { get; set; }
        public int Exchange { get; set; }
        public string Id { get; set; }
        public long ParticipantTimestamp { get; set; }
        public decimal Price { get; set; }
        public long SequenceNumber { get; set; }
        public long SipTimestamp { get; set; }
        public decimal Size { get; set; }
        public int? Tape { get; set; }
        public int? TrfId { get; set; }
        public long? TrfTimestamp { get; set; }
    }

    // Tick chart data model
    public class TickData
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public long Timestamp { get; set; }
        public int Trades { get; set; }
        public int TickNumber { get; set; }
    }

    public class PolygonTickChartConverter
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public PolygonTickChartConverter(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Fetches trade data from Polygon.io API
        /// </summary>
        /// <param name="ticker">Stock ticker symbol</param>
        /// <param name="date">Date in YYYY-MM-DD format</param>
        /// <returns>List of trades</returns>
        public async Task<List<PolygonTrade>> FetchPolygonTradeDataAsync(string ticker, string date)
        {
            try
            {
                string url = $"https://api.polygon.io/v3/trades/{ticker}?timestamp={date}&limit=50000&order=asc&sort=timestamp&apiKey={_apiKey}";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var tradeResponse = JsonSerializer.Deserialize<PolygonTradeResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tradeResponse?.Status == "OK" && tradeResponse.Results?.Count > 0)
                {
                    Console.WriteLine($"Retrieved {tradeResponse.Results.Count} trades for {ticker} on {date}");
                    return tradeResponse.Results;
                }
                else
                {
                    Console.Error.WriteLine("No trade data found or API error");
                    return new List<PolygonTrade>();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching trade data: {ex.Message}");
                return new List<PolygonTrade>();
            }
        }

        /// <summary>
        /// Converts trades data to tick chart data
        /// </summary>
        /// <param name="trades">List of trades</param>
        /// <param name="tickSize">Minimum price change to consider a new tick</param>
        /// <returns>List of tick data</returns>
        public List<TickData> ConvertTradesToTickChart(List<PolygonTrade> trades, decimal tickSize = 0.01m)
        {
            if (trades == null || !trades.Any())
                return new List<TickData>();

            var tickDataList = new List<TickData>();
            TickData currentTickData = null;
            int tickCount = 0;

            // Sort trades by timestamp if not already sorted
            var sortedTrades = trades.OrderBy(t => t.SipTimestamp).ToList();

            foreach (var trade in sortedTrades)
            {
                // For the first trade
                if (currentTickData == null)
                {
                    currentTickData = new TickData
                    {
                        Open = trade.Price,
                        High = trade.Price,
                        Low = trade.Price,
                        Close = trade.Price,
                        Volume = trade.Size,
                        Timestamp = trade.SipTimestamp,
                        Trades = 1,
                        TickNumber = tickCount
                    };
                }
                else
                {
                    // Check if this trade represents a new tick (price change)
                    decimal priceDifference = Math.Abs(trade.Price - currentTickData.Close);
                    bool isNewTick = priceDifference >= tickSize;

                    if (isNewTick)
                    {
                        // Save the completed tick data
                        tickDataList.Add(currentTickData);

                        // Start a new tick
                        tickCount++;
                        currentTickData = new TickData
                        {
                            Open = trade.Price,
                            High = trade.Price,
                            Low = trade.Price,
                            Close = trade.Price,
                            Volume = trade.Size,
                            Timestamp = trade.SipTimestamp,
                            Trades = 1,
                            TickNumber = tickCount
                        };
                    }
                    else
                    {
                        // Update the current tick with this trade
                        currentTickData.High = Math.Max(currentTickData.High, trade.Price);
                        currentTickData.Low = Math.Min(currentTickData.Low, trade.Price);
                        currentTickData.Close = trade.Price;
                        currentTickData.Volume += trade.Size;
                        currentTickData.Trades += 1;
                    }
                }
            }

            // Add the last tick if it exists
            if (currentTickData != null)
            {
                tickDataList.Add(currentTickData);
            }

            Console.WriteLine($"Converted {sortedTrades.Count} trades into {tickDataList.Count} ticks");
            return tickDataList;
        }

        /// <summary>
        /// Main function to execute the process
        /// </summary>
        /// <param name="ticker">Stock ticker symbol</param>
        /// <param name="date">Date in YYYY-MM-DD format</param>
        /// <param name="tickSize">Minimum price change to consider a new tick</param>
        /// <returns>Dictionary with process results</returns>
        public async Task<Dictionary<string, object>> GenerateTickChartAsync(string ticker, string date, decimal tickSize = 0.01m)
        {
            try
            {
                // Step 1: Fetch trade data
                var tradeData = await FetchPolygonTradeDataAsync(ticker, date);

                if (!tradeData.Any())
                {
                    Console.Error.WriteLine("No trade data available to create tick chart");
                    return new Dictionary<string, object>();
                }

                // Step 2: Convert to tick data
                var tickData = ConvertTradesToTickChart(tradeData, tickSize);

                // Return processed data
                return new Dictionary<string, object>
                {
                    ["TradeCount"] = tradeData.Count,
                    ["TickCount"] = tickData.Count,
                    ["TickData"] = tickData
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error generating tick chart: {ex.Message}");
                return new Dictionary<string, object>();
            }
        }
    }

    // Example Program class
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configuration
            string apiKey = "YOUR_POLYGON_API_KEY"; // Replace with your Polygon.io API key
            string ticker = "AAPL";                 // Replace with your desired ticker
            string date = "2025-05-10";             // Format: YYYY-MM-DD
            decimal tickSize = 0.01m;               // Minimum price change to consider a new tick

            var converter = new PolygonTickChartConverter(apiKey);
            var result = await converter.GenerateTickChartAsync(ticker, date, tickSize);

            if (result.Count > 0)
            {
                Console.WriteLine($"Process completed successfully!");
                Console.WriteLine($"Converted {result["TradeCount"]} trades into {result["TickCount"]} ticks");
            }
            else
            {
                Console.WriteLine("Process failed. Check error messages above.");
            }
        }
    }
}