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

        private MarketPosition enterDirection = MarketPosition.Flat;
        private double enterPrice = 0;
        private double exitPrice = 0;
        private double dailyPnl = 0;
        private DateTime currentDate = DateTime.MinValue;
        private bool finalizeDailyPnl = false;

        private DateTime startDate;
        private GuerillaPositionTracker positionTracker;

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

                EmaPeriod = 20;
                EmaEnterFilter = false;
                EmaExitFilter = true;
                EnterConfirmationBars = 1;
                ExitConfirmationBars = 1;
                StartDateString = DateTime.Now.ToShortDateString();
                StartMinutesOffset = 330;
                EndMinutesOffset = 780;
                DailyProfitTarget = 5500;
                DailyLossLimit = -4750;
                ExitOnNeutral = true;
                StopLoss = 0;
                ProfitTarget = 0;
                IsStrategyAnalyzer = true;

                TsiLongLength = 25;
                TsiShortLength = 5;
                TsiSignalLength = 14;

                RsiLength = 5;
			}
			else if (State == State.Configure)
			{
			}
            else if (State == State.DataLoaded)
            {
                tkp = GuerillaTkp(Close, TsiLongLength, TsiShortLength, TsiSignalLength, RsiLength, 50, 50);

                ema = EMA(this.EmaPeriod);
                AddChartIndicator(ema);
                ChartIndicators[0].Plots[0].Brush = Brushes.LimeGreen;
                ChartIndicators[0].Plots[0].Width = 2;

                startDate = DateTime.Parse(this.StartDateString);

                if (this.StopLoss > 0)
                {
                    SetStopLoss(CalculationMode.Currency, this.StopLoss);
                }

                if (this.ProfitTarget > 0)
                {
                    SetProfitTarget(CalculationMode.Currency, this.ProfitTarget);
                }

                positionTracker = new GuerillaPositionTracker();
                positionTracker.Positions.Add("position1", new GuerillaPosition(Bars.Instrument.MasterInstrument.PointValue));
                positionTracker.Positions.Add("position2", new GuerillaPosition(Bars.Instrument.MasterInstrument.PointValue));
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
                BarBrushes[0] = isUpBar ? upSellBrush : downSellBrush;
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
                finalizeDailyPnl = true;
            }

            if (finalizeDailyPnl)
            {
                dailyPnl = 0;
                finalizeDailyPnl = false;

                foreach (String k in positionTracker.Positions.Keys)
                {
                    positionTracker.Positions[k].Reset();
                }
            }

            if ((this.IsStrategyAnalyzer && Time[0].Date >= startDate) || (!this.IsStrategyAnalyzer && State == NinjaTrader.NinjaScript.State.Realtime))
            {
                #region Entry
                DateTime now = Time[0];
                DateTime startTime = now.Date.AddMinutes(this.StartMinutesOffset);
                DateTime endTime = now.Date.AddMinutes(this.EndMinutesOffset);

                bool validEnterTime = now.DayOfWeek != DayOfWeek.Sunday
                    && now.DayOfWeek != DayOfWeek.Saturday
                    && (this.StartMinutesOffset == 0 || (this.StartMinutesOffset != 0 && now >= startTime))
                    && (this.EndMinutesOffset == 0 || (this.EndMinutesOffset != 0 && now <= endTime));

                bool underDailyProfitTarget = DailyProfitTarget == 0 ? true : dailyPnl < DailyProfitTarget;
                bool underDailyLossLimit = DailyLossLimit == 0 ? true : dailyPnl > DailyLossLimit;

                if (underDailyProfitTarget && underDailyLossLimit && validEnterTime && positionTracker.IsFlat
                    && CountIf(() => tkp.Buy[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] > ema[0])))
                {
                    EnterLong(1, "position1");
                    EnterLong(1, "position2");
                    return;
                }
                else if (underDailyProfitTarget && underDailyLossLimit && validEnterTime && positionTracker.IsFlat
                    && CountIf(() => tkp.Sell[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] < ema[0])))
                {
                    EnterShort(1, "position1");
                    EnterShort(1, "position2");
                    return;
                } 
                #endregion

                #region Exit
                String positionKey = "";

                #region Position1
                positionKey = "position1";

                if (ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Short
                    && CountIf(() => !tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort(positionKey);
                }
                else if (ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Long
                    && CountIf(() => !tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong(positionKey);
                }
                else if (!ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Short
                    && CountIf(() => tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort(positionKey);
                }
                else if (!ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Long
                    && CountIf(() => tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong(positionKey);
                }
                #endregion

                #region Position2
                positionKey = "position2";

                if (ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Short
                    && CountIf(() => !tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort(positionKey);
                }
                else if (ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Long
                    && CountIf(() => !tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong(positionKey);
                }
                else if (!ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Short
                    && CountIf(() => tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort(positionKey);
                }
                else if (!ExitOnNeutral && positionTracker.Positions[positionKey].EnterDirection == MarketPosition.Long
                    && CountIf(() => tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong(positionKey);
                }
                #endregion 
                #endregion

                
            }
		}

        protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
        {
            String key = "";

            //Flat
            if (execution.Order.Name == "Exit on session close")
            {
                key = String.Empty;

                foreach (String k in positionTracker.Positions.Keys)
                {
                    if (positionTracker.Positions[k].EnterDirection != MarketPosition.Flat)
                    {
                        key = k;
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(key))
                {
                    positionTracker.Positions[key].ExitPrice = price;

                    dailyPnl += positionTracker.Positions[key].Pnl;

                    positionTracker.Positions[key].Reset();
                }
            }
            else if (positionTracker.Positions.ContainsKey(execution.Order.FromEntrySignal))
            {
                key = execution.Order.FromEntrySignal;

                positionTracker.Positions[key].ExitPrice = price;

                dailyPnl += positionTracker.Positions[key].Pnl;

                positionTracker.Positions[key].Reset();
            }
            //Entry
            else if (positionTracker.Positions.ContainsKey(execution.Order.Name))
            {
                key = execution.Order.Name;

                positionTracker.Positions[key].EnterDirection = marketPosition;
                positionTracker.Positions[key].EnterPrice = price;
            }            
        }

        #region Properties
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
        [Display(Name = "ExitOnNeutral", Order = 18, GroupName = "Parameters")]
        public bool ExitOnNeutral
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "StopLoss", Order = 20, GroupName = "Parameters")]
        public double StopLoss
        { get; set; }

        [NinjaScriptProperty]
        [Range(0, double.MaxValue)]
        [Display(Name = "ProfitTarget", Order = 21, GroupName = "Parameters")]
        public double ProfitTarget
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "IsStrategyAnalyzer", Order = 23, GroupName = "Parameters")]
        public bool IsStrategyAnalyzer
        { get; set; }

        #region TSI Parameters
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiLongLength", Order = 24, GroupName = "Parameters")]
        public int TsiLongLength
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiShortLength", Order = 25, GroupName = "Parameters")]
        public int TsiShortLength
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiSignalLength", Order = 26, GroupName = "Parameters")]
        public int TsiSignalLength
        { get; set; }
        #endregion

        #region RSI Parameters
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "RsiLength", Order = 27, GroupName = "Parameters")]
        public int RsiLength
        { get; set; }
        #endregion
        #endregion

        #region GuerillaPositionTracker
        private class GuerillaPositionTracker
        {
            public Dictionary<string, GuerillaPosition> Positions { get; set; }

            public List<GuerillaPosition> List
            {
                get
                {
                    return Positions.Values.ToList();
                }
            }

            public bool IsFlat
            {
                get
                {
                    return this.List.All(x => x.EnterDirection == MarketPosition.Flat);
                }
            }

            public GuerillaPositionTracker()
            {
                this.Positions = new Dictionary<string, GuerillaPosition>();
            }
        }
        #endregion

        #region GuerillaPosition
        private class GuerillaPosition
        {
            public MarketPosition EnterDirection { get; set; }
            public Double EnterPrice { get; set; }
            public Double ExitPrice { get; set; }
            public Double PointValue { get; set; }

            public Double Pnl
            {
                get
                {
                    if (EnterDirection == MarketPosition.Long)
                    {
                        return (ExitPrice - EnterPrice) * PointValue;
                    }
                    else
                    {
                        return (EnterPrice - ExitPrice) * PointValue;
                    }
                }
            }

            public GuerillaPosition()
            {
                this.EnterDirection = MarketPosition.Flat;
                this.EnterPrice = 0;
                this.ExitPrice = 0;
            }

            public GuerillaPosition(Double pointValue)
            {
                this.PointValue = pointValue;
                this.EnterDirection = MarketPosition.Flat;
                this.EnterPrice = 0;
                this.ExitPrice = 0;
            }

            public void Reset()
            {
                this.EnterDirection = MarketPosition.Flat;
                this.EnterPrice = 0;
                this.ExitPrice = 0;
            }
        } 
        #endregion
	}
}
