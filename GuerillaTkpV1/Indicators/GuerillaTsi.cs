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
	public class GuerillaTsi : Indicator
	{
        private GuerillaPcDif pc;
        private GuerillaAbsVal absPc;
        private GuerillaDoubleSmooth doubleSmoothedPc;
        private GuerillaDoubleSmooth doubleSmoothedAbsPc;
        
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaTsi";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
                IsSuspendedWhileInactive = true;
                TsiLongLength = 25;
                TsiShortLength = 5;
                TsiSignalLength = 14;

                AddPlot(Brushes.Goldenrod, "GuerillaTsiPlot");
			}
			else if (State == State.Configure)
			{
            }
            else if (State == State.DataLoaded)
            {
                pc = GuerillaPcDif(Close);
                absPc = GuerillaAbsVal(pc);

                doubleSmoothedPc = GuerillaDoubleSmooth(pc, this.TsiLongLength, this.TsiShortLength);
                doubleSmoothedAbsPc = GuerillaDoubleSmooth(absPc, this.TsiLongLength, this.TsiShortLength);
            }
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
            Value[0] = 100 * (doubleSmoothedPc[0] / doubleSmoothedAbsPc[0]);
		}

        #region Properties
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
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaTsi[] cacheGuerillaTsi;
		public GuerillaTsi GuerillaTsi(int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			return GuerillaTsi(Input, tsiLongLength, tsiShortLength, tsiSignalLength);
		}

		public GuerillaTsi GuerillaTsi(ISeries<double> input, int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			if (cacheGuerillaTsi != null)
				for (int idx = 0; idx < cacheGuerillaTsi.Length; idx++)
					if (cacheGuerillaTsi[idx] != null && cacheGuerillaTsi[idx].TsiLongLength == tsiLongLength && cacheGuerillaTsi[idx].TsiShortLength == tsiShortLength && cacheGuerillaTsi[idx].TsiSignalLength == tsiSignalLength && cacheGuerillaTsi[idx].EqualsInput(input))
						return cacheGuerillaTsi[idx];
			return CacheIndicator<GuerillaTsi>(new GuerillaTsi(){ TsiLongLength = tsiLongLength, TsiShortLength = tsiShortLength, TsiSignalLength = tsiSignalLength }, input, ref cacheGuerillaTsi);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaTsi GuerillaTsi(int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			return indicator.GuerillaTsi(Input, tsiLongLength, tsiShortLength, tsiSignalLength);
		}

		public Indicators.GuerillaTsi GuerillaTsi(ISeries<double> input , int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			return indicator.GuerillaTsi(input, tsiLongLength, tsiShortLength, tsiSignalLength);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaTsi GuerillaTsi(int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			return indicator.GuerillaTsi(Input, tsiLongLength, tsiShortLength, tsiSignalLength);
		}

		public Indicators.GuerillaTsi GuerillaTsi(ISeries<double> input , int tsiLongLength, int tsiShortLength, int tsiSignalLength)
		{
			return indicator.GuerillaTsi(input, tsiLongLength, tsiShortLength, tsiSignalLength);
		}
	}
}

#endregion
