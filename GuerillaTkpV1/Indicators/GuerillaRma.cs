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
	public class GuerillaRma : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaRma";
				Calculate									= Calculate.OnBarClose;
                IsOverlay = true;
                DrawOnPricePanel = true;
                DisplayInDataBox = false;
                IsAutoScale = false;
                IsSuspendedWhileInactive = true;
                Length = 5;

                AddPlot(Brushes.Goldenrod, "GuerillaRmaPlot");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
            if (CurrentBar == 0)
            {
                Value[0] = Input[0];
            }
            else
            {
                double alpha = this.Length;
                Value[0] = (Input[0] + (alpha - 1) * Value[1]) / alpha;
            }
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaRma[] cacheGuerillaRma;
		public GuerillaRma GuerillaRma(int length)
		{
			return GuerillaRma(Input, length);
		}

		public GuerillaRma GuerillaRma(ISeries<double> input, int length)
		{
			if (cacheGuerillaRma != null)
				for (int idx = 0; idx < cacheGuerillaRma.Length; idx++)
					if (cacheGuerillaRma[idx] != null && cacheGuerillaRma[idx].Length == length && cacheGuerillaRma[idx].EqualsInput(input))
						return cacheGuerillaRma[idx];
			return CacheIndicator<GuerillaRma>(new GuerillaRma(){ Length = length }, input, ref cacheGuerillaRma);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaRma GuerillaRma(int length)
		{
			return indicator.GuerillaRma(Input, length);
		}

		public Indicators.GuerillaRma GuerillaRma(ISeries<double> input , int length)
		{
			return indicator.GuerillaRma(input, length);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaRma GuerillaRma(int length)
		{
			return indicator.GuerillaRma(Input, length);
		}

		public Indicators.GuerillaRma GuerillaRma(ISeries<double> input , int length)
		{
			return indicator.GuerillaRma(input, length);
		}
	}
}

#endregion
