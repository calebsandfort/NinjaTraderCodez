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
        private Brush buyBrush = Brushes.DeepSkyBlue;
        private Brush neutralBrush = Brushes.GhostWhite;
        private Brush sellBrush = Brushes.DeepPink;
        private Brush transparentBrush = Brushes.Transparent;

        private GuerillaTkp tkp;
        private EMA ema;

        private GuerillaStdDev upperStdDev;
        private GuerillaStdDev lowerStdDev;
        //private RSI rsi;

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
                EmaPeriod = 20;
                EmaEnterFilter = true;
                EmaExitFilter = true;
                FlipOnBarChange = false;
                FlipOnBarChangeEmaFilter = false;
                StopLoss = 0;
                EnterConfirmationBars = 1;
                ExitConfirmationBars = 1;
                //ProfitTarget = 0;
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
                //rsi = RSI(14, 7);
                //AddChartIndicator(rsi);
                //ChartIndicators[1].Panel = 1;

                //upperStdDev = GuerillaStdDev(ema, this.EmaPeriod, this.StdDevMultiplier);
                //AddChartIndicator(upperStdDev);

                //lowerStdDev = GuerillaStdDev(ema, this.EmaPeriod, -this.StdDevMultiplier);
                //AddChartIndicator(lowerStdDev);

                if (this.StopLoss > 0)
                {
                    SetStopLoss(CalculationMode.Currency, this.StopLoss);
                }

                //if (this.ProfitTarget > 0)
                //{
                //    SetProfitTarget(CalculationMode.Currency, this.ProfitTarget);
                //}
            }
		}

		protected override void OnBarUpdate()
		{
            #region Color Bar
            bool isUpBar = Close[0] > Open[0];

            if (tkp.Buy[0])
            {
                BarBrushes[0] = isUpBar ? transparentBrush : buyBrush;
                CandleOutlineBrushes[0] = buyBrush;
            }
            else if (tkp.Sell[0])
            {
                BarBrushes[0] = isUpBar ? transparentBrush : sellBrush;
                CandleOutlineBrushes[0] = sellBrush;
            }
            else
            {
                BarBrushes[0] = isUpBar ? transparentBrush : neutralBrush;
                CandleOutlineBrushes[0] = neutralBrush;
            } 
            #endregion


            if (Time[0].Date >= new DateTime(2019, 9, 1))
            {
                //bool enterLong = false;
                //bool enterShort = false;
                //bool exitLong = false;
                //bool exitShort = false;



                if (Position.MarketPosition == MarketPosition.Flat
                    && CountIf(() => tkp.Buy[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] > ema[0])))
                {
                    EnterLong();
                }
                else if (Position.MarketPosition == MarketPosition.Flat
                    && CountIf(() => tkp.Sell[0], EnterConfirmationBars) == EnterConfirmationBars
                    && (!EmaEnterFilter || (EmaEnterFilter && Close[0] < ema[0])))
                {
                    EnterShort();
                }
                else if (FlipOnBarChangeEmaFilter && Position.MarketPosition == MarketPosition.Short && tkp.Buy[0] && (!FlipOnBarChangeEmaFilter || (FlipOnBarChangeEmaFilter && Close[0] > ema[0])))
                {
                    EnterLong();
                }
                else if (FlipOnBarChange && Position.MarketPosition == MarketPosition.Long && tkp.Sell[0] && (!FlipOnBarChangeEmaFilter || (FlipOnBarChangeEmaFilter && Close[0] < ema[0])))
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
                else if (Position.MarketPosition == MarketPosition.Short
                    && CountIf(() => !tkp.Sell[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] > ema[0])))
                {
                    ExitShort();
                }
                else if (Position.MarketPosition == MarketPosition.Long
                    && CountIf(() => !tkp.Buy[0], ExitConfirmationBars) == ExitConfirmationBars
                    && (!EmaExitFilter || (EmaExitFilter && Close[0] < ema[0])))
                {
                    ExitLong();
                }

                //Print(String.Format("Date: {0}, Buy: {1}, Sell: {2}, Neutral: {3}, EMA: {4}", Time[0].Date, tkp.Buy[0], tkp.Sell[0], tkp.Neutral[0], Close[0] > ema[0]));
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

        //[NinjaScriptProperty]
        //[Range(0, double.MaxValue)]
        //[Display(Name = "ProfitTarget", Order = 6, GroupName = "Parameters")]
        //public double ProfitTarget
        //{ get; set; }
        #endregion
	}
}
