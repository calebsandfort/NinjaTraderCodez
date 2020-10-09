#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class GuerillaTkpV1 : Strategy
    {
        private Brush upBuyBrush = Brushes.DeepSkyBlue;
        private Brush downBuyBrush = Brushes.RoyalBlue;
        private Brush upNeutralBrush = Brushes.GhostWhite;
        private Brush downNeutralBrush = Brushes.Gray;
        private Brush upSellBrush = Brushes.MediumOrchid;
        private Brush downSellBrush = Brushes.DeepPink;
        private Brush transparentBrush = Brushes.Transparent;

        private GuerillaTkp tkp;
        private EMA ema;

        private GuerillaStdDev upperStdDev;
        private GuerillaStdDev lowerStdDev;

        //private GuerillaTimeBetweenBars tbb;
        //private EMA tbbEma;
        //private double minTbb = double.MaxValue;
        //private double maxTbb = double.MinValue;

        private MarketPosition enterDirection = MarketPosition.Flat;
        private double enterPrice = 0;
        private double exitPrice = 0;
        private double dailyPnl = 0;
        private DateTime currentDate = DateTime.MinValue;
        private bool finalizeDailyPnl = false;
        private RSI rsi;

        private DateTime startDate;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "GuerillaTkpV1";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= false;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.High;
				OrderFillResolutionType						= BarsPeriodType.Minute;
				OrderFillResolutionValue					= 1;
				Slippage									= 1;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 10;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;

                //StdDevMultiplier = 2;
                //ExitOnStdDevCross = false;
                EmaPeriod = 40;
                EmaEnterFilter = true;
                EmaExitFilter = true;
                FlipOnBarChange = false;
                FlipOnBarChangeEmaFilter = false;
                StopLoss = 0;
                EnterConfirmationBars = 3;
                ExitConfirmationBars = 2;
                ProfitTarget = 0;
                StartDateString = DateTime.Now.ToShortDateString();
                StartMinutesOffset = 330;
                EndMinutesOffset = 780;
                DailyProfitTarget = 2750;
                DailyLossLimit = 0;
                TimeBetweenBarsPeriod = 3;
                MaxTbbThreshold = 0;
                ExitOnNeutral = true;
                ExitMidTrade = false;
                IsStrategyAnalyzer = true;
			}
			else if (State == State.Configure)
			{
			}
            else if (State == State.DataLoaded)
            {
                tkp = GuerillaTkp(Close, 25, 5, 14, 5, 50, 50);
                //AddChartIndicator(guerillaTkp);

                ema = EMA(this.EmaPeriod);
                AddChartIndicator(ema);
                ChartIndicators[0].Plots[0].Brush = Brushes.LimeGreen;
                ChartIndicators[0].Plots[0].Width = 2;

                //tbb = GuerillaTimeBetweenBars();
                //tbbEma = EMA(tbb, this.TimeBetweenBarsPeriod);
                //AddChartIndicator(tbbEma);
                //ChartIndicators[1].IsOverlay = false;
                //ChartIndicators[1].Panel = 1;
                //ChartIndicators[1].Plots[0].Brush = Brushes.DarkOrange;
                //ChartIndicators[1].Plots[0].Width = 2;

                //rsi = RSI(14, 7);
                //AddChartIndicator(rsi);
                //ChartIndicators[1].Panel = 2;

                //upperStdDev = GuerillaStdDev(ema, this.EmaPeriod, this.StdDevMultiplier);
                //AddChartIndicator(upperStdDev);

                //lowerStdDev = GuerillaStdDev(ema, this.EmaPeriod, -this.StdDevMultiplier);
                //AddChartIndicator(lowerStdDev);

                if (this.StopLoss > 0)
                {
                    SetStopLoss(CalculationMode.Currency, this.StopLoss);
                }

                if (this.ProfitTarget > 0)
                {
                    SetProfitTarget(CalculationMode.Currency, this.ProfitTarget);
                }

                startDate = DateTime.Parse(this.StartDateString);
            }
		}

		protected override void OnBarUpdate()
		{
            #region Color Bar
            bool isUpBar = Close[0] > Open[0];

            if (tkp.Buy[0])
            {
                BarBrushes[0] = isUpBar ? upBuyBrush : downBuyBrush;
                //CandleOutlineBrushes[0] = upBuyBrush;
            }
            else if (tkp.Sell[0])
            {
                BarBrushes[0] = !isUpBar ? upSellBrush : downSellBrush;
                //CandleOutlineBrushes[0] = upSellBrush;
            }
            else
            {
                BarBrushes[0] = isUpBar ? upNeutralBrush : downNeutralBrush;
                //CandleOutlineBrushes[0] = upNeutralBrush;
            } 
            #endregion


            if (Time[0].Date != currentDate.Date)
            {
                currentDate = Time[0].Date;
                //Print(currentDate.ToShortDateString());
                finalizeDailyPnl = true;
            }

            if (finalizeDailyPnl)
            {
                //Print(String.Format("Date: {0}, PnL: {1:C}", currentDate.AddDays(-1).ToShortDateString(), dailyPnl));
                dailyPnl = 0;
                finalizeDailyPnl = false;
            }

            if ((this.IsStrategyAnalyzer && Time[0].Date >= startDate) || (!this.IsStrategyAnalyzer && State == NinjaTrader.NinjaScript.State.Realtime))
            {
                //bool enterLong = false;
                //bool enterShort = false;
                //bool exitLong = false;
                //bool exitShort = false;

                //Print(tbbEma[0]);

                DateTime now = Time[0];
                DateTime startTime = now.Date.AddMinutes(this.StartMinutesOffset);
                DateTime endTime = now.Date.AddMinutes(this.EndMinutesOffset);

                //if(now >= endTime && 

                bool validEnterTime = now.DayOfWeek != DayOfWeek.Sunday
                    && now.DayOfWeek != DayOfWeek.Saturday
                    && (this.StartMinutesOffset == 0 || (this.StartMinutesOffset != 0 && now >= startTime))
                    && (this.EndMinutesOffset == 0 || (this.EndMinutesOffset != 0 && now <= endTime));

                //Print(String.Format("{0}, {1}, {2}", startTime.ToShortTimeString(), now.ToShortTimeString(), endTime.ToShortTimeString()));

                //if (validEnterTime)
                //{
                //    minTbb = Math.Min(minTbb, tbbEma[0]);
                //    maxTbb = Math.Max(maxTbb, tbbEma[0]);

                //    //Print(String.Format("{0}, {1}", minTbb, maxTbb));
                //}

                bool underDailyProfitTarget = DailyProfitTarget == 0 ? true : dailyPnl < DailyProfitTarget;
                bool underDailyLossLimit = DailyLossLimit == 0 ? true : dailyPnl > DailyLossLimit;
                //bool tbbFilter = MaxTbbThreshold == 0 ? true : tbbEma[0] < MaxTbbThreshold;
                bool tbbFilter = true;

                if (this.ExitMidTrade && Position.MarketPosition == MarketPosition.Long && DailyLossLimit != 0
                    && (dailyPnl + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency)) < DailyLossLimit)
                {
                    ExitLong();
                }
                else if (this.ExitMidTrade && Position.MarketPosition == MarketPosition.Short && DailyLossLimit != 0
                    && (dailyPnl + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency)) < DailyLossLimit)
                {
                    ExitShort();
                }
                else if (tbbFilter && underDailyProfitTarget && underDailyLossLimit && validEnterTime && Position.MarketPosition == MarketPosition.Flat
                    && CountIf(() => tkp.Buy[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] > ema[0])))
                {
                    EnterLong();
                }
                else if (tbbFilter && underDailyProfitTarget && underDailyLossLimit && validEnterTime && Position.MarketPosition == MarketPosition.Flat
                    && CountIf(() => tkp.Sell[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] < ema[0])))
                {
                    EnterShort();
                }
                else if (validEnterTime && FlipOnBarChangeEmaFilter && Position.MarketPosition == MarketPosition.Short && tkp.Buy[0] && (!FlipOnBarChangeEmaFilter || (FlipOnBarChangeEmaFilter && Close[0] > ema[0])))
                {
                    EnterLong();
                }
                else if (validEnterTime && FlipOnBarChange && Position.MarketPosition == MarketPosition.Long && tkp.Sell[0] && (!FlipOnBarChangeEmaFilter || (FlipOnBarChangeEmaFilter && Close[0] < ema[0])))
                {
                    EnterShort();
                }
                //else if (ExitOnStdDevCross && Position.MarketPosition == MarketPosition.Long && CrossBelow(Close, upperStdDev, 1))
                //{
                //    ExitLong();
                //}
                //else if (ExitOnStdDevCross && Position.MarketPosition == MarketPosition.Short && CrossAbove(Close, lowerStdDev, 1))
                //{
                //    ExitShort();
                //}
                else if (ExitOnNeutral && Position.MarketPosition == MarketPosition.Short
                    && CountIf(() => !tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort();
                }
                else if (ExitOnNeutral && Position.MarketPosition == MarketPosition.Long
                    && CountIf(() => !tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong();
                }
                else if (!ExitOnNeutral && Position.MarketPosition == MarketPosition.Short
                    && CountIf(() => tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort();
                }
                else if (!ExitOnNeutral && Position.MarketPosition == MarketPosition.Long
                    && CountIf(() => tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong();
                }

                //Print(String.Format("Date: {0}, Buy: {1}, Sell: {2}, Neutral: {3}, EMA: {4}", Time[0].Date, tkp.Buy[0], tkp.Sell[0], tkp.Neutral[0], Close[0] > ema[0]));
            }
            
		}

        protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
        {
            //Print(String.Format("price: {0:C}, quantity: {1}, marketPosition: {2}, Position.MarketPosition: {3}, ", price, quantity, marketPosition, Position.MarketPosition));

            if (Position.MarketPosition == MarketPosition.Flat)
            {
                exitPrice = price;

                double pnl = 0;

                if (enterDirection == MarketPosition.Long)
                {
                    pnl = (exitPrice - enterPrice) * Bars.Instrument.MasterInstrument.PointValue;
                }
                else
                {
                    pnl = (enterPrice - exitPrice) * Bars.Instrument.MasterInstrument.PointValue;
                }

                dailyPnl += pnl;

                //Print(String.Format("time: {0}, pnl: {1:C}", time, pnl));

                enterDirection = MarketPosition.Flat;
                enterPrice = 0;
                exitPrice = 0;
            }
            else
            {
                enterDirection = marketPosition;
                enterPrice = price;
            }
        }

        #region Properties
        //[NinjaScriptProperty]
        //[Range(double.MinValue, double.MaxValue)]
        //[Display(Name = "StdDevMultiplier", Order = 1, GroupName = "Parameters")]
        //public double StdDevMultiplier
        //{ get; set; }

        //[NinjaScriptProperty]
        //[Display(Name = "ExitOnStdDevCross", Order = 2, GroupName = "Parameters")]
        //public bool ExitOnStdDevCross
        //{ get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "EmaPeriod", Order = 1, GroupName = "Parameters")]
        public int EmaPeriod
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "EmaEnterFilter", Order = 2, GroupName = "Parameters")]
        public bool EmaEnterFilter
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "EmaExitFilter", Order = 3, GroupName = "Parameters")]
        public bool EmaExitFilter
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "FlipOnBarChange", Order = 4, GroupName = "Parameters")]
        public bool FlipOnBarChange
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "FlipOnBarChangeEmaFilter", Order = 6, GroupName = "Parameters")]
        public bool FlipOnBarChangeEmaFilter
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "StopLoss", Order = 7, GroupName = "Parameters")]
        public double StopLoss
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "EnterConfirmationBars", Order = 8, GroupName = "Parameters")]
        public int EnterConfirmationBars
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "ExitConfirmationBars", Order = 9, GroupName = "Parameters")]
        public int ExitConfirmationBars
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "ProfitTarget", Order = 10, GroupName = "Parameters")]
        public double ProfitTarget
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "StartDateString", Order = 11, GroupName = "Parameters")]
        public String StartDateString
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, int.MaxValue)]
        [Display(Name = "StartMinutesOffset", Order = 12, GroupName = "Parameters")]
        public int StartMinutesOffset
        { get; set; }
            
        [NinjaScriptProperty]
        [Range(0, int.MaxValue)]
        [Display(Name = "EndMinutesOffset", Order = 13, GroupName = "Parameters")]
        public int EndMinutesOffset
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "DailyProfitTarget", Order = 14, GroupName = "Parameters")]
        public double DailyProfitTarget
        { get; set; }

        [NinjaScriptProperty]
        [Range(double.MinValue, 0)]
        [Display(Name = "DailyLossLimit", Order = 15, GroupName = "Parameters")]
        public double DailyLossLimit
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, int.MaxValue)]
        [Display(Name = "TimeBetweenBarsPeriod", Order = 16, GroupName = "Parameters")]
        public int TimeBetweenBarsPeriod
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "MaxTbbThreshold", Order = 17, GroupName = "Parameters")]
        public double MaxTbbThreshold
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ExitOnNeutral", Order = 18, GroupName = "Parameters")]
        public bool ExitOnNeutral
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ExitMidTrade", Order = 19, GroupName = "Parameters")]
        public bool ExitMidTrade
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "IsStrategyAnalyzer", Order = 20, GroupName = "Parameters")]
        public bool IsStrategyAnalyzer
        { get; set; }
        #endregion
	}
}
