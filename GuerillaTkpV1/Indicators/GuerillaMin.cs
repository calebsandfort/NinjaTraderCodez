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
	public class GuerillaMin : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaMin";
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
				IsSuspendedWhileInactive					= true;
                CompareValue = 0;

                AddPlot(Brushes.Goldenrod, "GuerillaMinPlot");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
            Value[0] = Math.Min(Input[0], this.CompareValue);
		}

        #region Properties
        [NinjaScriptProperty]
        [Range(int.MinValue, int.MaxValue)]
        [Display(Name = "CompareValue", Order = 1, GroupName = "Parameters")]
        public double CompareValue
        { get; set; }
        #endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaMin[] cacheGuerillaMin;
		public GuerillaMin GuerillaMin(double compareValue)
		{
			return GuerillaMin(Input, compareValue);
		}

		public GuerillaMin GuerillaMin(ISeries<double> input, double compareValue)
		{
			if (cacheGuerillaMin != null)
				for (int idx = 0; idx < cacheGuerillaMin.Length; idx++)
					if (cacheGuerillaMin[idx] != null && cacheGuerillaMin[idx].CompareValue == compareValue && cacheGuerillaMin[idx].EqualsInput(input))
						return cacheGuerillaMin[idx];
			return CacheIndicator<GuerillaMin>(new GuerillaMin(){ CompareValue = compareValue }, input, ref cacheGuerillaMin);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaMin GuerillaMin(double compareValue)
		{
			return indicator.GuerillaMin(Input, compareValue);
		}

		public Indicators.GuerillaMin GuerillaMin(ISeries<double> input , double compareValue)
		{
			return indicator.GuerillaMin(input, compareValue);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaMin GuerillaMin(double compareValue)
		{
			return indicator.GuerillaMin(Input, compareValue);
		}

		public Indicators.GuerillaMin GuerillaMin(ISeries<double> input , double compareValue)
		{
			return indicator.GuerillaMin(input, compareValue);
		}
	}
}

#endregion
