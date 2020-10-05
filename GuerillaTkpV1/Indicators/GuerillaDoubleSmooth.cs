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
	public class GuerillaDoubleSmooth : Indicator
	{
        private EMA firstSmooth;
        private EMA secondSmooth;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaDoubleSmooth";
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
				LongPeriod					= 14;
				ShortPeriod					= 5;


                AddPlot(Brushes.Goldenrod, "GuerillaDoubleSmoothPlot");
			}
			else if (State == State.Configure)
			{
            }
            else if (State == State.DataLoaded)
            {
                firstSmooth = EMA(Input, this.LongPeriod);
                secondSmooth = EMA(firstSmooth, this.ShortPeriod);
            }
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
            this.Value[0] = secondSmooth[0];
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LongPeriod", Order=1, GroupName="Parameters")]
		public int LongPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ShortPeriod", Order=2, GroupName="Parameters")]
		public int ShortPeriod
		{ get; set; }
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaDoubleSmooth[] cacheGuerillaDoubleSmooth;
		public GuerillaDoubleSmooth GuerillaDoubleSmooth(int longPeriod, int shortPeriod)
		{
			return GuerillaDoubleSmooth(Input, longPeriod, shortPeriod);
		}

		public GuerillaDoubleSmooth GuerillaDoubleSmooth(ISeries<double> input, int longPeriod, int shortPeriod)
		{
			if (cacheGuerillaDoubleSmooth != null)
				for (int idx = 0; idx < cacheGuerillaDoubleSmooth.Length; idx++)
					if (cacheGuerillaDoubleSmooth[idx] != null && cacheGuerillaDoubleSmooth[idx].LongPeriod == longPeriod && cacheGuerillaDoubleSmooth[idx].ShortPeriod == shortPeriod && cacheGuerillaDoubleSmooth[idx].EqualsInput(input))
						return cacheGuerillaDoubleSmooth[idx];
			return CacheIndicator<GuerillaDoubleSmooth>(new GuerillaDoubleSmooth(){ LongPeriod = longPeriod, ShortPeriod = shortPeriod }, input, ref cacheGuerillaDoubleSmooth);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaDoubleSmooth GuerillaDoubleSmooth(int longPeriod, int shortPeriod)
		{
			return indicator.GuerillaDoubleSmooth(Input, longPeriod, shortPeriod);
		}

		public Indicators.GuerillaDoubleSmooth GuerillaDoubleSmooth(ISeries<double> input , int longPeriod, int shortPeriod)
		{
			return indicator.GuerillaDoubleSmooth(input, longPeriod, shortPeriod);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaDoubleSmooth GuerillaDoubleSmooth(int longPeriod, int shortPeriod)
		{
			return indicator.GuerillaDoubleSmooth(Input, longPeriod, shortPeriod);
		}

		public Indicators.GuerillaDoubleSmooth GuerillaDoubleSmooth(ISeries<double> input , int longPeriod, int shortPeriod)
		{
			return indicator.GuerillaDoubleSmooth(input, longPeriod, shortPeriod);
		}
	}
}

#endregion
