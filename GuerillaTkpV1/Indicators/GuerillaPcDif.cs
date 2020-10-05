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
	public class GuerillaPcDif : Indicator
	{
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "GuerillaPcDif";
                IsOverlay = true;
                DrawOnPricePanel = true;
                DisplayInDataBox = false;
                IsAutoScale = false;
                IsSuspendedWhileInactive = true;

                AddPlot(Brushes.Goldenrod, "GuerillaPcDifPlot");
            }
            else if (State == State.Configure)
            {
                
            }
        }

        protected override void OnBarUpdate()
        {
            Value[0] = (CurrentBar == 0 ? Input[0] : Input[0] - Input[1]);
        }

        #region Properties

        #endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private GuerillaPcDif[] cacheGuerillaPcDif;
		public GuerillaPcDif GuerillaPcDif()
		{
			return GuerillaPcDif(Input);
		}

		public GuerillaPcDif GuerillaPcDif(ISeries<double> input)
		{
			if (cacheGuerillaPcDif != null)
				for (int idx = 0; idx < cacheGuerillaPcDif.Length; idx++)
					if (cacheGuerillaPcDif[idx] != null &&  cacheGuerillaPcDif[idx].EqualsInput(input))
						return cacheGuerillaPcDif[idx];
			return CacheIndicator<GuerillaPcDif>(new GuerillaPcDif(), input, ref cacheGuerillaPcDif);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.GuerillaPcDif GuerillaPcDif()
		{
			return indicator.GuerillaPcDif(Input);
		}

		public Indicators.GuerillaPcDif GuerillaPcDif(ISeries<double> input )
		{
			return indicator.GuerillaPcDif(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.GuerillaPcDif GuerillaPcDif()
		{
			return indicator.GuerillaPcDif(Input);
		}

		public Indicators.GuerillaPcDif GuerillaPcDif(ISeries<double> input )
		{
			return indicator.GuerillaPcDif(input);
		}
	}
}

#endregion
