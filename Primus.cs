using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

using ATAS.DataFeedsCore;
using ATAS.Indicators;
using ATAS.Indicators.Technical;
using ATAS.Strategies.Chart;
using ATAS.Indicators.Technical.Properties;
using Newtonsoft.Json.Linq;
using OFT.Rendering.Context;
using OFT.Rendering.Tools;
using Utils.Common.Logging;
using static ATAS.Indicators.Technical.SampleProperties;

using Color = System.Drawing.Color;
using MColor = System.Windows.Media.Color;
using MColors = System.Windows.Media.Colors;
using Pen = System.Drawing.Pen;
using String = System.String;
using OFT.Rendering.Control;

namespace Primus
{
    public class Primus : ChartStrategy
    {
        #region Private fields

        private Order _order = new Order();
        private Stopwatch clock = new Stopwatch();
        private Rectangle rc = new Rectangle() { X=50, Y=50, Height=200, Width=400 };
        private DateTime dtStart = DateTime.Now;
        private String sLastTrade = String.Empty;
        private int iPrevOrderBar = -1;
        private int iOrderDirection = -1;

        private const int INFO = 1;
        private const int WARN = 2;
        private const int ERROR = 3;
        private const int DEBUG = 0;
        private const int LONG = 1;
        private const int SHORT = 2;
        private const int ACTIVE = 1;
        private const int STOPPED = 2;

        private const String sVersion = "Beta 1.1";
        private List<string> lsH = new List<string>();
        private List<string> lsM = new List<string>();

        private decimal iJunk = 0;
        private int _lastBar = -1;
        private int _prevBar = -1;
        private int iMinDelta = 0;
        private int iMinDeltaPercent = 0;
        private int iMinADX = 0;
        private int iOffset = 9;
        private int iFontSize = 10;
        private int iWaddaSensitivity = 150;
        private int iBotStatus = ACTIVE;

        private bool _lastBarCounted;
        private bool bNewsProcessed = false;
        private bool bShowUp = true;
        private bool bShowDown = true;
        private bool bCloseOnStop = true;

        public decimal Volume = 1;

        #endregion

        #region GENERIC / DISPLAY OPTIONS

        [Display(Name = "Font Size", GroupName = "Drawing", Order = int.MaxValue)]
        [Range(1, 90)]
        public int TextFont { get => iFontSize; set { iFontSize = value; RecalculateValues(); } }

        [Display(Name = "Text Offset", GroupName = "Drawing", Order = int.MaxValue)]
        [Range(0, 900)]
        public int Offset { get => iOffset; set { iOffset = value; RecalculateValues(); } }

        [Display(ResourceType = typeof(Resources), GroupName = "Alerts", Name = "UseAlerts")]
        public bool UseAlerts { get; set; }

        [Display(ResourceType = typeof(Resources), GroupName = "Alerts", Name = "AlertFile")]
        public string AlertFile { get; set; } = "alert1";

        #endregion

        #region START TRADING OPTIONS

        private bool bEnterBuySell = true;
        private bool bEnterVolImb = true;
        private bool bEnterBBWick = true;
        private bool bEnter921Cross = false;
        private bool bEnterKamaWick = false;
        private bool bEnterNebula = false;
        private bool bEnterPinbar = false;

        [Display(GroupName = "Begin trading when", Name = "Standard buy/sell signal")]
        public bool EnterBuySell { get => bEnterBuySell; set { bEnterBuySell = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "Volume imbalance (candle gap)")]
        public bool EnterVolImb { get => bEnterVolImb; set { bEnterVolImb = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "Bollinger bands wick")]
        public bool EnterBBWick { get => bEnterBBWick; set { bEnterBBWick = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "9/21 cross")]
        public bool Enter921Cross { get => bEnter921Cross; set { bEnter921Cross = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "KAMA wick")]
        public bool EnterKamaWick { get => bEnterKamaWick; set { bEnterKamaWick = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "Standard Nebula rules")]
        public bool EnterNebula { get => bEnterNebula; set { bEnterNebula = value; RecalculateValues(); } }

        [Display(GroupName = "Begin trading when", Name = "Enter on pin bar")]
        public bool EnterPinbar { get => bEnterPinbar; set { bEnterPinbar = value; RecalculateValues(); } }

        #endregion

        #region ADVANCED OPTIONS

        private bool bHoldTradeOnContraryOrder = false;
        private int iAdvMaxContracts = 6;

        [Display(GroupName = "Advanced Options", Name = "Hold current trade on contrary order")]
        public bool HoldTradeOnContraryOrder { get => bHoldTradeOnContraryOrder; set { bHoldTradeOnContraryOrder = value; RecalculateValues(); } }

        [Display(Name = "Max simultaneous contracts", GroupName = "Advanced Options", Order = int.MaxValue)]
        [Range(1, 90)]
        public int AdvMaxContracts { get => iAdvMaxContracts; set { iAdvMaxContracts = value; RecalculateValues(); } }

        #endregion

        #region INDICATORS

        private readonly SMA _LindaShort = new SMA() { Period = 3 };
        private readonly SMA _LindaLong = new SMA() { Period = 10 };
        private readonly SMA _LindaSignal = new SMA() { Period = 16 };

        private readonly RSI _rsi = new() { Period = 14 };
        private readonly ATR _atr = new() { Period = 14 };
        private readonly AwesomeOscillator _ao = new AwesomeOscillator();
        private readonly ParabolicSAR _psar = new ParabolicSAR();
        private readonly ADX _adx = new ADX() { Period = 10 };
        private readonly EMA _9 = new EMA() { Period = 9 };
        private readonly EMA _21 = new EMA() { Period = 21 };
        private readonly EMA fastEma = new EMA() { Period = 20 };
        private readonly EMA slowEma = new EMA() { Period = 40 };
        private readonly FisherTransform _ft = new FisherTransform() { Period = 10 };
        private readonly SuperTrend _st = new SuperTrend() { Period = 10, Multiplier = 1m };
        private readonly BollingerBands _bb = new BollingerBands() { Period = 20, Shift = 0, Width = 2 };
        private readonly KAMA _kama9 = new KAMA() { ShortPeriod = 2, LongPeriod = 109, EfficiencyRatioPeriod = 9 };
        private readonly MACD _macd = new MACD() { ShortPeriod = 12, LongPeriod = 26, SignalPeriod = 9 };
        private readonly T3 _t3 = new T3() { Period = 10, Multiplier = 1 };

        #endregion

        #region BUY SELL FILTERS

        // Default TRUE
        private bool bUseFisher = true;          // USE
        private bool bUseWaddah = true;
        private bool bUseT3 = true;
        private bool bUseSuperTrend = true;
        private bool bUseAO = true;
        private bool bUsePSAR = true;
        private bool bVolumeImbalances = true;

        // Default FALSE
        private bool bUseMACD = false;
        private bool bUseLinda = false;
        private bool bUseKAMA = false;

        [Display(GroupName = "Buy/Sell Filters", Name = "Waddah Explosion", Description = "The Waddah Explosion must be the correct color, and have a value")]
        public bool Use_Waddah_Explosion { get => bUseWaddah; set { bUseWaddah = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "Awesome Oscillator", Description = "AO is positive or negative")]
        public bool Use_Awesome { get => bUseAO; set { bUseAO = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "Parabolic SAR", Description = "The PSAR must be signaling a buy/sell signal same as the arrow")]
        public bool Use_PSAR { get => bUsePSAR; set { bUsePSAR = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "Squeeze Momentum", Description = "The squeeze must be the correct color")]
        public bool Use_Squeeze_Momentum { get => bUseSqueeze; set { bUseSqueeze = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "MACD", Description = "Standard 12/26/9 MACD crossing in the correct direction")]
        public bool Use_MACD { get => bUseMACD; set { bUseMACD = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "Linda MACD", Description = "")]
        public bool UseLinda { get => bUseLinda; set { bUseLinda = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "SuperTrend", Description = "Price must align to the current SuperTrend trend")]
        public bool Use_SuperTrend { get => bUseSuperTrend; set { bUseSuperTrend = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "T3", Description = "Price must cross the T3")]
        public bool Use_T3 { get => bUseT3; set { bUseT3 = value; RecalculateValues(); } }

        [Display(GroupName = "Buy/Sell Filters", Name = "Fisher Transform", Description = "Fisher Transform must cross to the correct direction")]
        public bool Use_Fisher_Transform { get => bUseFisher; set { bUseFisher = value; RecalculateValues(); } }

        [Display(GroupName = "Value Filters", Name = "Minimum Delta", Description = "The minimum candle delta value to show buy/sell")]
        [Range(0, 9000)]
        public int Min_Delta { get => iMinDelta; set { if (value < 0) return; iMinDelta = value; RecalculateValues(); } }

        [Display(GroupName = "Value Filters", Name = "Minimum Delta Percent", Description = "Minimum diff between max delta and delta to show buy/sell")]
        [Range(0, 100)]
        public int Min_Delta_Percent { get => iMinDeltaPercent; set { if (value < 0) return; iMinDeltaPercent = value; RecalculateValues(); } }

        [Display(GroupName = "Value Filters", Name = "Minimum ADX", Description = "Minimum ADX value before showing buy/sell")]
        [Range(0, 100)]
        public int Min_ADX { get => iMinADX; set { if (value < 0) return; iMinADX = value; RecalculateValues(); } }

        #endregion

        #region CONSTRUCTOR

        public Primus()
        {
            bCloseOnStop = true;
            EnableCustomDrawing = true;

            Add(_ao);
            Add(_ft);
            Add(_psar);
            Add(_st);
            Add(_kama9);
            Add(_adx);
        }

        #endregion

        #region RENDER CONTEXT

        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            var font = new RenderFont("Calibri", iFontSize);
            var fontB = new RenderFont("Calibri", iFontSize, FontStyle.Bold);
            int upY = 50;
            int upX = 50;
            var txt = String.Empty;

            // LINE 1 - BOT STATUS + ACCOUNT + START TIME
            switch (iBotStatus)
            {
                case ACTIVE:
                    TimeSpan t = TimeSpan.FromMilliseconds(clock.ElapsedMilliseconds);
                    String an = String.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                    txt = $"PrimusBot ACTIVE on {TradingManager.Portfolio.AccountID} since " + dtStart.ToString() + " (" + an + ")";
                    context.DrawString(txt, fontB, Color.Lime, upX, upY);
                    if (!clock.IsRunning)
                        clock.Start();
                    break;
                case STOPPED:
                    txt = $"BOT STOPPED on {TradingManager.Portfolio.AccountID}";
                    context.DrawString(txt, fontB, Color.Orange, upX, upY);
                    if (clock.IsRunning)
                        clock.Stop();
                    break;
            }
            var tsize = context.MeasureString(txt, fontB);
            upY += tsize.Height + 6;

            // LINE 2 - TOTAL TRADES + PNL
            if (TradingManager.Portfolio != null && TradingManager.Position != null)
            {
                txt = $"{TradingManager.MyTrades.Count()} trades, with PNL: {TradingManager.Position.RealizedPnL}";
                if (iBotStatus == STOPPED) { txt = String.Empty; sLastTrade = String.Empty; }
                context.DrawString(txt, font, Color.White, upX, upY);
                upY += tsize.Height + 6;
                txt = sLastTrade;
                context.DrawString(txt, font, Color.White, upX, upY);
            }

            if (sLastLog != String.Empty && iBotStatus == ACTIVE)
            {
                upY += tsize.Height + 6;
                txt = $"Last Log: " + sLastLog;
                context.DrawString(txt, font, Color.Yellow, upX, upY);
            }

        }

        #endregion

        #region MAIN LOGIC

        protected override void OnCalculate(int bar, decimal value)
        {
            if (iBotStatus == STOPPED || bar < (CurrentBar - 5))
                return;

            var pbar = bar; // - 1;
            var prevBar = _prevBar;
            _prevBar = bar;

            if (prevBar == bar)
                return;

            #region CANDLE CALCULATIONS

            var candle = GetCandle(pbar);
            value = candle.Close;
            var chT = ChartInfo.ChartType;
            var red = candle.Close < candle.Open;
            var green = candle.Close > candle.Open;

            bShowDown = true;
            bShowUp = true;

            decimal _tick = ChartInfo.PriceChartContainer.Step;
            var p1C = GetCandle(pbar - 1);
            var p2C = GetCandle(pbar - 2);
            var p3C = GetCandle(pbar - 3);
            var p4C = GetCandle(pbar - 4);

            var c0G = candle.Open < candle.Close;
            var c0R = candle.Open > candle.Close;
            var c1G = p1C.Open < p1C.Close;
            var c1R = p1C.Open > p1C.Close;
            var c2G = p2C.Open < p2C.Close;
            var c2R = p2C.Open > p2C.Close;
            var c3G = p3C.Open < p3C.Close;
            var c3R = p3C.Open > p3C.Close;
            var c4G = p4C.Open < p4C.Close;
            var c4R = p4C.Open > p4C.Close;

            var c0Body = Math.Abs(candle.Close - candle.Open);
            var c1Body = Math.Abs(p1C.Close - p1C.Open);
            var c2Body = Math.Abs(p2C.Close - p2C.Open);
            var c3Body = Math.Abs(p3C.Close - p3C.Open);
            var c4Body = Math.Abs(p4C.Close - p4C.Open);

            var upWickLarger = c0R && Math.Abs(candle.High - candle.Open) > Math.Abs(candle.Low - candle.Close);
            var downWickLarger = c0G && Math.Abs(candle.Low - candle.Open) > Math.Abs(candle.Close - candle.High);
            var upPinbar = c0R && Math.Abs(candle.High - candle.Open) > (Math.Abs(candle.Low - candle.Close) * 3);
            var downPinbar = c0G && Math.Abs(candle.Low - candle.Open) > (Math.Abs(candle.Close - candle.High) * 3);

            decimal deltaPer = 0;
            if (candle.Delta > 0 && candle.MaxDelta > 0)
            {
                if (candle.MinDelta > 0)
                    deltaPer = (candle.Delta / candle.MaxDelta) * 100;
                else
                    deltaPer = (candle.Delta / candle.MinDelta) * 100;
            }

            #endregion

            #region INDICATOR CALCULATIONS

            _t3.Calculate(pbar, value);
            fastEma.Calculate(pbar, value);
            slowEma.Calculate(pbar, value);
            _21.Calculate(pbar, value);
            _macd.Calculate(pbar, value);
            _bb.Calculate(pbar, value);
            _rsi.Calculate(pbar, value);

            var ao = ((ValueDataSeries)_ao.DataSeries[0])[pbar];
            var kama9 = ((ValueDataSeries)_kama9.DataSeries[0])[pbar];
            var kama21 = ((ValueDataSeries)_kama9.DataSeries[0])[pbar];
            var m1 = ((ValueDataSeries)_macd.DataSeries[0])[pbar];
            var m2 = ((ValueDataSeries)_macd.DataSeries[1])[pbar];
            var m3 = ((ValueDataSeries)_macd.DataSeries[2])[pbar];
            var t3 = ((ValueDataSeries)_t3.DataSeries[0])[pbar];
            var fast = ((ValueDataSeries)fastEma.DataSeries[0])[pbar];
            var fastM = ((ValueDataSeries)fastEma.DataSeries[0])[pbar - 1];
            var slow = ((ValueDataSeries)slowEma.DataSeries[0])[pbar];
            var slowM = ((ValueDataSeries)slowEma.DataSeries[0])[pbar - 1];
            var f1 = ((ValueDataSeries)_ft.DataSeries[0])[pbar];
            var f2 = ((ValueDataSeries)_ft.DataSeries[1])[pbar];
            var st = ((ValueDataSeries)_st.DataSeries[0])[pbar];
            var x = ((ValueDataSeries)_adx.DataSeries[0])[pbar];
            var nn = ((ValueDataSeries)_9.DataSeries[0])[pbar];
            var prev_nn = ((ValueDataSeries)_9.DataSeries[0])[pbar - 1];
            var twone = ((ValueDataSeries)_21.DataSeries[0])[pbar];
            var prev_twone = ((ValueDataSeries)_21.DataSeries[0])[pbar - 1];
            var psar = ((ValueDataSeries)_psar.DataSeries[0])[pbar];
            var bb_mid = ((ValueDataSeries)_bb.DataSeries[0])[pbar]; // mid
            var bb_top = ((ValueDataSeries)_bb.DataSeries[1])[pbar]; // top
            var bb_bottom = ((ValueDataSeries)_bb.DataSeries[2])[pbar]; // bottom
            var rsi = ((ValueDataSeries)_rsi.DataSeries[0])[pbar];
            var rsi1 = ((ValueDataSeries)_rsi.DataSeries[0])[pbar - 1];
            var rsi2 = ((ValueDataSeries)_rsi.DataSeries[0])[pbar - 2];

            var t1 = ((fast - slow) - (fastM - slowM)) * 150; // iWaddaSensitivity;

            var fisherUp = (f1 < f2);
            var fisherDown = (f2 < f1);
            var macdUp = (m1 > m2);
            var macdDown = (m1 < m2);
            var psarBuy = (psar < candle.Close);
            var psarSell = (psar > candle.Close);

            var atr = _atr[bar];
            var median = (candle.Low + candle.High) / 2;
            var dUpperLevel = median + atr * 1.7m;
            var dLowerLevel = median - atr * 1.7m;

            #endregion

            #region BAR PATTERN CALCULATIONS

            // Linda MACD
            var lmacd = _LindaShort.Calculate(pbar, value) - _LindaLong.Calculate(pbar, value);
            var signal = _LindaSignal.Calculate(pbar, lmacd);
            var Linda = macd - signal;

            var NebulaLong = (t1 > 0 && (c0G && c1G && candle.Close > p1C.Close && p1C.Close > p2C.Close));
            var NebulaShort = (t1 < 0 && (c0R && c1R && candle.Close < p1C.Close && p1C.Close < p2C.Close));

            if (deltaPer < iMinDeltaPercent)
            {
                bShowUp = false;
                bShowDown = false;
            }

            // Standard BUY / SELL
            if ((Linda < 0 && bUseLinda) || (candle.Delta < iMinDelta) || (!macdUp && bUseMACD) || (psarSell && bUsePSAR) || (!fisherUp && bUseFisher) || (value < t3 && bUseT3) || (value < kama9 && bUseKAMA) || (t1 < 0 && bUseWaddah) || (ao < 0 && bUseAO) || (st == 0 && bUseSuperTrend) || (x < iMinADX))
                bShowUp = false;

            if ((Linda > 0 && bUseLinda) || (candle.Delta > (iMinDelta * -1)) || (psarBuy && bUsePSAR) || (!macdDown && bUseMACD) || (!fisherDown && bUseFisher) || (value > kama9 && bUseKAMA) || (value > t3 && bUseT3) || (t1 >= 0 && bUseWaddah) || (ao > 0 && bUseAO) || (st == 0 && bUseSuperTrend) || (x < iMinADX))
                bShowDown = false;

            #endregion

            var bbWickLong = false;
            var bbWickShort = false;
            // Bollinger band bounce
            if (candle.Low < bb_bottom && candle.Open > bb_bottom && c0G && candle.Close > p1C.Close && downWickLarger)
                bbWickLong = true;
            if (candle.High > bb_top && candle.Open < bb_top && c0R && candle.Close < p1C.Close && upWickLarger)
                bbWickShort = true;

            #region ENTRANCE STRATEGIES

            if (bShowDown)
                OpenPosition("Standard Sell Signal", candle, bar, SHORT);
            if (bShowUp)
                OpenPosition("Standard Buy Signal", candle, bar, LONG);

            if (green && c1G && candle.Open > p1C.Close && bEnterVolImb)
                OpenPosition("Volume Imbalance", candle, bar, LONG);
            if (red && c1R && candle.Open < p1C.Close)
                OpenPosition("Volume Imbalance", candle, bar, SHORT);

            if (downPinbar && bEnterPinbar)
                OpenPosition("Pinbar Buy", candle, bar, LONG);
            if (upPinbar && bEnterPinbar)
                OpenPosition("Pinbar Sell", candle, bar, SHORT);

            if (NebulaLong && bEnterNebula) //  && !sLastTrade.Contains("NEBULA"))
                OpenPosition("NEBULA Buy Signal", candle, bar, LONG);
            if (NebulaShort && bEnterNebula) // && !sLastTrade.Contains("NEBULA"))
                OpenPosition("NEBULA Sell Signal", candle, bar, SHORT);

            if (eqHigh && bEnterHighLow)
                OpenPosition("Equal High", candle, bar, SHORT);
            if (eqLow && bEnterHighLow)
                OpenPosition("Equal Low", candle, bar, LONG);

            if (nn > twone && prev_nn <= prev_twone && bEnter921Cross)
                OpenPosition("9/21 cross", candle, bar, LONG);
            if (nn < twone && prev_nn >= prev_twone && bEnter921Cross)
                OpenPosition("9/21 cross", candle, bar, SHORT);

            if (bEnterBBWick && bbWickLong && downWickLarger && !sLastTrade.Contains("wick EXIT"))
                OpenPosition("Bollinger Band Wick", candle, bar, LONG);
            if (bEnterBBWick && bbWickShort && upWickLarger && !sLastTrade.Contains("wick EXIT"))
                OpenPosition("Bollinger Band Wick", candle, bar, SHORT);

            if (green && candle.Low < kama9 && candle.Close > kama9 && bEnterKamaWick)
                OpenPosition("9 KAMA Wick", candle, bar, LONG);
            if (red && candle.High > kama9 && candle.Close < kama9 && bEnterKamaWick)
                OpenPosition("9 KAMA Wick", candle, bar, SHORT);

            #endregion

        }

        #endregion

        #region POSITION METHODS

        private void OpenPosition(String sReason, IndicatorCandle c, int bar, int iDirection = -1)
        {
            String sD = String.Empty;
            if (iBotStatus == STOPPED)
            {
                AddLog("Attempted to open position, but bot was stopped");
                return;
            }

            if (iDirection == LONG)
            {
                sLastTrade = "Bar " + bar + " - " + sReason + " LONG at " + c.Close;
                sD = sReason + " LONG (" + bar + ")";
            }
            else
            {
                sLastTrade = "Bar " + bar + " - " + sReason + " SHORT at " + c.Close;
                sD = sReason + " SHORT (" + bar + ")";
            }

            // Limit 1 order per bar
            if (iPrevOrderBar == bar)
                return;
            else
                iPrevOrderBar = bar;

            if (iOrderDirection == SHORT && CurrentPosition > 0)
                CloseCurrentPosition("Opposite direction order - cancelling current", bar);
            if (iOrderDirection == LONG && CurrentPosition < 0)
                CloseCurrentPosition("Opposite direction order - cancelling current", bar);

            if (CurrentPosition > 0 && iOrderDirection == SHORT)
            {
                AddLog("Cannot reverse order already in progress");
                return;
            }
            if (CurrentPosition < 0 && iOrderDirection == LONG)
            {
                AddLog("Cannot reverse order already in progress");
                return;
            }

            OrderDirections d = OrderDirections.Buy;
            if (c.Open > c.Close || iDirection == SHORT)
                d = OrderDirections.Sell;
            if (c.Open < c.Close || iDirection == LONG)
                d = OrderDirections.Buy;

            _order = new Order
            {
                Portfolio = Portfolio,
                Security = Security,
                Direction = d,
                Type = OrderTypes.Market,
                QuantityToFill = 1, // GetOrderVolume(),
                Comment = sD
            };
            OpenOrder(_order);
            AddLog(sLastTrade);
            iOrderDirection = iDirection;
            AddAlert(AlertFile, "POSITION OPENED");
        }

        private void CloseCurrentPosition(String s, int bar)
        {
            // Limit 1 order per bar
            if (iPrevOrderBar == bar)
                return;
            else
                iPrevOrderBar = bar;

            if (s.Contains("Opposite") && bHoldTradeOnContraryOrder)
            {
                AddLog("Current position held based upon config");
                return;
            }

            if (CurrentPosition != 0)
            {
                _order = new Order
                {
                    Portfolio = Portfolio,
                    Security = Security,
                    Direction = CurrentPosition > 0 ? OrderDirections.Sell : OrderDirections.Buy,
                    Type = OrderTypes.Market,
                    QuantityToFill = Math.Abs(CurrentPosition),
                    Comment = s
                };
                OpenOrder(_order);
                sLastTrade = s;
            }
        }

        protected override void OnOrderRegisterFailed(Order order, string message)
        {
            if (order == _order)
                AddLog("ORDER FAILED: " + message);
        }

        protected override void OnOrderChanged(Order order)
        {
            if (order == _order)
            {
                switch (order.Status())
                {
                    case OrderStatus.None:
                        // The order has an undefined status (you need to wait for the next method calls).
                        break;
                    case OrderStatus.Placed:
                        // the order is placed.
                        break;
                    case OrderStatus.Filled:
                        // the order is filled.
                        break;
                    case OrderStatus.PartlyFilled:
                        // the order is partially filled.
                        {
                            var unfilled = order.Unfilled; // this is a unfilled volume.

                            break;
                        }
                    case OrderStatus.Canceled:
                        // the order is canceled.
                        break;
                }
            }
        }

        protected override void OnStopping()
        {
            if (CurrentPosition != 0 && bCloseOnStop)
            {
                RaiseShowNotification($"Closing position {CurrentPosition} on stopping.");
                CloseCurrentPosition($"Closing position {CurrentPosition} on stopping.", 0);
            }

            base.OnStopping();
        }

        #endregion

        #region MISC METHODS

        private bool IsPointInsideRectangle(Rectangle rectangle, Point point)
        {
            return point.X >= rectangle.X && point.X <= rectangle.X + rectangle.Width && point.Y >= rectangle.Y && point.Y <= rectangle.Y + rectangle.Height;
        }

        public override bool ProcessMouseClick(RenderControlMouseEventArgs e)
        {
            if (e.Button == RenderControlMouseButtons.Left && IsPointInsideRectangle(rc, e.Location))
            {
                if (iBotStatus == ACTIVE)
                    CloseCurrentPosition("Taking full profit", CurrentBar);
            }
            if (e.Button == RenderControlMouseButtons.Right && IsPointInsideRectangle(rc, e.Location))
            {
                if (iBotStatus == ACTIVE)
                {
                    CloseCurrentPosition("Bot stopped by user command", CurrentBar);
                    iBotStatus = STOPPED;
                }
            }
            return false;
        }

        private void AddLog(String s, int iSev = INFO)
        {
            switch (iSev)
            {
                case INFO: this.LogInfo(s); break;
                case ERROR: this.LogError(s); break;
                case WARN: this.LogWarn(s); break;
                default: this.LogDebug(s); break;
            }
        }

        private decimal VolSec(IndicatorCandle c) { return c.Volume / Convert.ToDecimal((c.LastTime - c.Time).TotalSeconds); }

            #endregion

        }
    }
