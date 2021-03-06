﻿namespace StockSharp.Anywhere
{
    using System;
    using System.IO;
    using System.Text;

    using BusinessEntities;
    using Messages;

    /// <summary>
    ///     The write functions to text files
    /// </summary>
    public static class WriteHelpers
    {
        private static readonly string _outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OUTPUT"); // Data output folder path

        private static readonly string _tradesFilePath = Path.Combine(_outputFolder, "trades.txt");
        private static readonly string _ordersFilePath = Path.Combine(_outputFolder, "orders.txt");
        private static readonly string _myTradesFilePath = Path.Combine(_outputFolder, "mytrades.txt");
        private static readonly string _level1FilePath = Path.Combine(_outputFolder, "level1.txt");
        private static readonly string _positionsFilePath = Path.Combine(_outputFolder, "positions.txt");

        static WriteHelpers()
        {
            if (!Directory.Exists(_outputFolder))
                Directory.CreateDirectory(_outputFolder);

            if (!File.Exists(_tradesFilePath))
                File.Create(_outputFolder);

            if (!File.Exists(_ordersFilePath))
                File.Create(_ordersFilePath);

            if (!File.Exists(_myTradesFilePath))
                File.Create(_myTradesFilePath);

            if (!File.Exists(_level1FilePath))
                File.Create(_level1FilePath);

            if (!File.Exists(_positionsFilePath))
                File.Create(_positionsFilePath);
        }

        public static void WriteTrade(this ExecutionMessage tick)
        {
            MemoryToFile(TradeToString(tick), _tradesFilePath);
        }

        public static void WriteMyTrade(this ExecutionMessage trade)
        {
            MemoryToFile(MyTradeToString(trade), _myTradesFilePath);
        }

        public static void WriteMarketDepth(this QuoteChangeMessage quotes)
        {
            MemoryToFile(QuotesToString(quotes), Path.Combine(_outputFolder, string.Format("{0}_depth.txt", quotes.SecurityId.SecurityCode)));
        }

        public static void WriteLevel1(this Level1ChangeMessage level)
        {
            MemoryToFile(Level1ToString(level), _level1FilePath);
        }

        public static void WriteOrder(this ExecutionMessage order)
        {
            MemoryToFile(OrderToString(order), _ordersFilePath);
        }

        public static void WritePosition(this PositionChangeMessage position)
        {
            MemoryToFile(PositionToString(position), _positionsFilePath);
        }

        private static string TradeToString(ExecutionMessage tick)
        {
            //securityId;tradeId;time;price;volume;orderdirection 
            return string.Format("{0};{1};{2};{3};{4};{5}",
                tick.SecurityId.SecurityCode,
                tick.TradeId,
                tick.ServerTime,
                tick.TradePrice,
                tick.Volume,
                tick.Side);
        }

        private static string MyTradeToString(ExecutionMessage trade)
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6}",
                trade.SecurityId.SecurityCode,
                trade.TradeId,
                trade.ServerTime,
                trade.TradePrice,
                trade.Volume,
                trade.Side,
                trade.OrderId);
        }

        private static string OrderToString(ExecutionMessage order)
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12}",
                order.OrderId,
                order.OriginalTransactionId,
                order.ServerTime,
                order.SecurityId.SecurityCode,
                order.PortfolioName,
                order.Volume,
                order.Balance,
                order.Price,
                order.OriginSide,
                order.OrderType,
                order.OrderState,
                order.IsCancelled,
                order.LocalTime);
        }

        private static string PositionToString(PositionChangeMessage position)
        {
            return string.Format("{0};{1};{2};{3};{4}",
                position.SecurityId.SecurityCode,
                position.PortfolioName,
                position.Changes[PositionChangeTypes.BeginValue],
                position.Changes[PositionChangeTypes.CurrentValue],
                position.Changes[PositionChangeTypes.AveragePrice]);
        }

        private static string QuotesToString(QuoteChangeMessage quotes)
        {
            var sb = new StringBuilder();
            foreach (var quote in quotes.Bids)
                sb.Append(string.Format("Bid:{0}:{1};", quote.Price, quote.Volume));
            foreach (var quote in quotes.Asks)
                sb.Append(string.Format("Ask:{0}:{1};", quote.Price, quote.Volume));

            return string.Format("{0};{1};{2}",
                quotes.ServerTime,
                quotes.SecurityId.SecurityCode,
                sb
                );
        }

        private static string Level1ToString(Level1ChangeMessage level)
        {
            var sb = new StringBuilder();
            foreach (var kvp in level.Changes)
                sb.Append(string.Format("{0}:{1};", kvp.Key, kvp.Value));

            return string.Format("{0};{1};{2}",
                level.ServerTime,
                level.SecurityId.SecurityCode,
                sb
                );
        }

        private static void MemoryToFile(string line, string filePath)
        {
            using (var mem = new MemoryStream(line.Length + 2))
            {
                var bytes = Encoding.UTF8.GetBytes(line);
                mem.Write(bytes, 0, bytes.Length);
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                    mem.WriteTo(file);
            }
        }
    }
}