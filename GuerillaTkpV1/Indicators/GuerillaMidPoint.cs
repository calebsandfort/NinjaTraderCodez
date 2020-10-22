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
	public class GuerillaMidPoint : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaMidPoint";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				AddPlot(Brushes.Yellow, "GuerillaMidPointPlot");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
            Value[0] = Low[0] + ((High[0] - Low[0]) / 2);
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> GuerillaMidPointPlot
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaMidPoint[] cacheGuerillaMidPoint;
		public GuerillaMidPoint GuerillaMidPoint()
		{
			return GuerillaMidPoint(Input);
		}

		public GuerillaMidPoint GuerillaMidPoint(ISeries<double> input)
		{
			if (cacheGuerillaMidPoint != null)
				for (int idx = 0; idx < cacheGuerillaMidPoint.Length; idx++)
					if (cacheGuerillaMidPoint[idx] != null &&  cacheGuerillaMidPoint[idx].EqualsInput(input))
						return cacheGuerillaMidPoint[idx];
			return CacheIndicator<GuerillaMidPoint>(new GuerillaMidPoint(), input, ref cacheGuerillaMidPoint);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaMidPoint GuerillaMidPoint()
		{
			return indicator.GuerillaMidPoint(Input);
		}

		public Indicators.GuerillaMidPoint GuerillaMidPoint(ISeries<double> input )
		{
			return indicator.GuerillaMidPoint(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaMidPoint GuerillaMidPoint()
		{
			return indicator.GuerillaMidPoint(Input);
		}

		public Indicators.GuerillaMidPoint GuerillaMidPoint(ISeries<double> input )
		{
			return indicator.GuerillaMidPoint(input);
		}
	}
}

#endregion
