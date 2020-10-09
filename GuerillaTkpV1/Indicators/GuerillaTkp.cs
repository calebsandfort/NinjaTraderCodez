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
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class GuerillaTkp : Indicator
	{
        private GuerillaPcDif pc;
        private GuerillaTsi tsi;
        private EMA tsiSignalLine;

        private GuerillaMax maxPc;
        private GuerillaMin minPc;
        private GuerillaAbsVal absMinPc;
        private GuerillaRma upRma;
        private GuerillaRma downRma;

        private Series<double> rsi;
        private Series<bool> buy;
        private Series<bool> sell;
        private Series<bool> neutral;

        private Brush upBuyBrush = Brushes.DeepSkyBlue;
        private Brush downBuyBrush = Brushes.RoyalBlue;
        private Brush upNeutralBrush = Brushes.GhostWhite;
        private Brush downNeutralBrush = Brushes.Gray;
        private Brush upSellBrush = Brushes.MediumOrchid;
        private Brush downSellBrush = Brushes.DeepPink;
        private Brush transparentBrush = Brushes.Transparent;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaTkp";
				Calculate									= Calculate.OnBarClose;
                //IsOverlay									= false;
                //DisplayInDataBox							= true;
                //DrawOnPricePanel							= true;
                //DrawHorizontalGridLines						= true;
                //DrawVerticalGridLines						= true;
                //PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				TsiLongLength					= 25;
				TsiShortLength					= 5;
				TsiSignalLength					= 14;

                RsiLength = 5;
                RsiBuyFilterLevel = 50;
                RsiSellFilterLevel = 50;

                AddPlot(Brushes.Goldenrod, "GuerillaTkpPlot");
			}
			else if (State == State.Configure)
			{
			}
            else if (State == State.DataLoaded)
            {
                pc = GuerillaPcDif(Close);

                #region TSI
                tsi = GuerillaTsi(this.TsiLongLength, this.TsiShortLength, this.TsiSignalLength);
                tsiSignalLine = EMA(tsi, this.TsiSignalLength); 
                #endregion

                #region RSI
                maxPc = GuerillaMax(pc, 0);
                minPc = GuerillaMin(pc, 0);
                absMinPc = GuerillaAbsVal(minPc);

                upRma = GuerillaRma(maxPc, this.RsiLength);
                downRma = GuerillaRma(absMinPc, this.RsiLength);

                rsi = new Series<double>(this);
                buy = new Series<bool>(this);
                sell = new Series<bool>(this);
                neutral = new Series<bool>(this);
                #endregion

            }
		}

		protected override void OnBarUpdate()
		{
            #region RSI
            if (downRma[0] == 0)
            {
                rsi[0] = 100;
            }
            else if (upRma[0] == 0)
            {
                rsi[0] = 0;
            }
            else
            {
                rsi[0] = 100 - (100 / (1 + (upRma[0] / downRma[0])));
            } 
            #endregion

            #region Signals
            buy[0] = ((tsi[0] > tsiSignalLine[0]) && (rsi[0] > RsiBuyFilterLevel));
            sell[0] = ((tsi[0] < tsiSignalLine[0]) && (rsi[0] < RsiSellFilterLevel));
            neutral[0] = !buy[0] && !sell[0];
            #endregion

            Value[0] = tsi[0];

            #region Color Bar
            bool isUpBar = Close[0] > Open[0];

            if (Buy[0])
            {
                BarBrushes[0] = isUpBar ? upBuyBrush : downBuyBrush;
                //CandleOutlineBrushes[0] = upBuyBrush;
            }
            else if (Sell[0])
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
		}

		#region Properties
        #region TSI Parameters
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiLongLength", Order = 1, GroupName = "Parameters")]
        public int TsiLongLength
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiShortLength", Order = 2, GroupName = "Parameters")]
        public int TsiShortLength
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "TsiSignalLength", Order = 3, GroupName = "Parameters")]
        public int TsiSignalLength
        { get; set; } 
        #endregion

        #region RSI Parameters
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "RsiLength", Order = 4, GroupName = "Parameters")]
        public int RsiLength
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "RsiBuyFilterLevel", Order = 5, GroupName = "Parameters")]
        public int RsiBuyFilterLevel
        { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "RsiSellFilterLevel", Order = 6, GroupName = "Parameters")]
        public int RsiSellFilterLevel
        { get; set; }
        #endregion

        #region Series
        [Browsable(false)]
        [XmlIgnore()]
        public Series<double> Rsi
        {
            get
            {
                Update();
                return rsi;
            }
        }
        
        [Browsable(false)]
        [XmlIgnore()]
        public Series<bool> Buy
        {
            get
            {
                Update();
                return buy;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public Series<bool> Sell
        {
            get
            {
                Update();
                return sell;
            }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public Series<bool> Neutral
        {
            get
            {
                Update();
                return neutral;
            }
        }
        #endregion
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaTkp[] cacheGuerillaTkp;
		public GuerillaTkp GuerillaTkp(int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			return GuerillaTkp(Input, tsiLongLength, tsiShortLength, tsiSignalLength, rsiLength, rsiBuyFilterLevel, rsiSellFilterLevel);
		}

		public GuerillaTkp GuerillaTkp(ISeries<double> input, int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			if (cacheGuerillaTkp != null)
				for (int idx = 0; idx < cacheGuerillaTkp.Length; idx++)
					if (cacheGuerillaTkp[idx] != null && cacheGuerillaTkp[idx].TsiLongLength == tsiLongLength && cacheGuerillaTkp[idx].TsiShortLength == tsiShortLength && cacheGuerillaTkp[idx].TsiSignalLength == tsiSignalLength && cacheGuerillaTkp[idx].RsiLength == rsiLength && cacheGuerillaTkp[idx].RsiBuyFilterLevel == rsiBuyFilterLevel && cacheGuerillaTkp[idx].RsiSellFilterLevel == rsiSellFilterLevel && cacheGuerillaTkp[idx].EqualsInput(input))
						return cacheGuerillaTkp[idx];
			return CacheIndicator<GuerillaTkp>(new GuerillaTkp(){ TsiLongLength = tsiLongLength, TsiShortLength = tsiShortLength, TsiSignalLength = tsiSignalLength, RsiLength = rsiLength, RsiBuyFilterLevel = rsiBuyFilterLevel, RsiSellFilterLevel = rsiSellFilterLevel }, input, ref cacheGuerillaTkp);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaTkp GuerillaTkp(int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			return indicator.GuerillaTkp(Input, tsiLongLength, tsiShortLength, tsiSignalLength, rsiLength, rsiBuyFilterLevel, rsiSellFilterLevel);
		}

		public Indicators.GuerillaTkp GuerillaTkp(ISeries<double> input , int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			return indicator.GuerillaTkp(input, tsiLongLength, tsiShortLength, tsiSignalLength, rsiLength, rsiBuyFilterLevel, rsiSellFilterLevel);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaTkp GuerillaTkp(int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			return indicator.GuerillaTkp(Input, tsiLongLength, tsiShortLength, tsiSignalLength, rsiLength, rsiBuyFilterLevel, rsiSellFilterLevel);
		}

		public Indicators.GuerillaTkp GuerillaTkp(ISeries<double> input , int tsiLongLength, int tsiShortLength, int tsiSignalLength, int rsiLength, int rsiBuyFilterLevel, int rsiSellFilterLevel)
		{
			return indicator.GuerillaTkp(input, tsiLongLength, tsiShortLength, tsiSignalLength, rsiLength, rsiBuyFilterLevel, rsiSellFilterLevel);
		}
	}
}

#endregion
