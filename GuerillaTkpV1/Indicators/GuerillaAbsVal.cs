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
	public class GuerillaAbsVal : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaAbsVal";
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

                AddPlot(Brushes.Goldenrod, "GuerillaAbsValPlot");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
            Value[0] = Math.Abs(Input[0]);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaAbsVal[] cacheGuerillaAbsVal;
		public GuerillaAbsVal GuerillaAbsVal()
		{
			return GuerillaAbsVal(Input);
		}

		public GuerillaAbsVal GuerillaAbsVal(ISeries<double> input)
		{
			if (cacheGuerillaAbsVal != null)
				for (int idx = 0; idx < cacheGuerillaAbsVal.Length; idx++)
					if (cacheGuerillaAbsVal[idx] != null &&  cacheGuerillaAbsVal[idx].EqualsInput(input))
						return cacheGuerillaAbsVal[idx];
			return CacheIndicator<GuerillaAbsVal>(new GuerillaAbsVal(), input, ref cacheGuerillaAbsVal);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaAbsVal GuerillaAbsVal()
		{
			return indicator.GuerillaAbsVal(Input);
		}

		public Indicators.GuerillaAbsVal GuerillaAbsVal(ISeries<double> input )
		{
			return indicator.GuerillaAbsVal(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaAbsVal GuerillaAbsVal()
		{
			return indicator.GuerillaAbsVal(Input);
		}

		public Indicators.GuerillaAbsVal GuerillaAbsVal(ISeries<double> input )
		{
			return indicator.GuerillaAbsVal(input);
		}
	}
}

#endregion
