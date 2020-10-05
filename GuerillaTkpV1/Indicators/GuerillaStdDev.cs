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
	public class GuerillaStdDev : Indicator
	{
        StdDev stdDev;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GuerillaStdDev";
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
				Multiplier					= 2;
				AddPlot(Brushes.Orange, "GuerillaStdDevPlot");
			}
			else if (State == State.Configure)
			{
            }
            else if (State == State.DataLoaded)
            {
                stdDev = StdDev(Input, this.Period);
            }
		}

		protected override void OnBarUpdate()
		{
            Value[0] = Input[0] + (stdDev[0] * this.Multiplier);
		}

		#region Properties
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Period", Order = 1, GroupName = "Parameters")]
        public int Period
        { get; set; }

		[NinjaScriptProperty]
        [Range(double.MinValue, double.MaxValue)]
		[Display(Name="Multiplier", Order=2, GroupName="Parameters")]
		public double Multiplier
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> GuerillaStdDevPlot
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
		private GuerillaStdDev[] cacheGuerillaStdDev;
		public GuerillaStdDev GuerillaStdDev(int period, double multiplier)
		{
			return GuerillaStdDev(Input, period, multiplier);
		}

		public GuerillaStdDev GuerillaStdDev(ISeries<double> input, int period, double multiplier)
		{
			if (cacheGuerillaStdDev != null)
				for (int idx = 0; idx < cacheGuerillaStdDev.Length; idx++)
					if (cacheGuerillaStdDev[idx] != null && cacheGuerillaStdDev[idx].Period == period && cacheGuerillaStdDev[idx].Multiplier == multiplier && cacheGuerillaStdDev[idx].EqualsInput(input))
						return cacheGuerillaStdDev[idx];
			return CacheIndicator<GuerillaStdDev>(new GuerillaStdDev(){ Period = period, Multiplier = multiplier }, input, ref cacheGuerillaStdDev);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaStdDev GuerillaStdDev(int period, double multiplier)
		{
			return indicator.GuerillaStdDev(Input, period, multiplier);
		}

		public Indicators.GuerillaStdDev GuerillaStdDev(ISeries<double> input , int period, double multiplier)
		{
			return indicator.GuerillaStdDev(input, period, multiplier);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaStdDev GuerillaStdDev(int period, double multiplier)
		{
			return indicator.GuerillaStdDev(Input, period, multiplier);
		}

		public Indicators.GuerillaStdDev GuerillaStdDev(ISeries<double> input , int period, double multiplier)
		{
			return indicator.GuerillaStdDev(input, period, multiplier);
		}
	}
}

#endregion
