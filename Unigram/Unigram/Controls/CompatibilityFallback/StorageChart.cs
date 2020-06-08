using System.Collections.Generic;
using Unigram.Common;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Unigram.Controls.CompatibilityFallback
{
    public class StorageChart : Panel
    {
        private readonly FrameworkElement storageChartControl;

        public IList<StorageChartItem> Items
        {
            get
            {
                if (storageChartControl is StorageChartUWPv5 old) 
                    return old.Items;
                else if (storageChartControl is Controls.StorageChart modern)
                    return modern.Items;
                return null;
            }
            set
            {
                if (storageChartControl is StorageChartUWPv5 old)
                {
                    storageChartControl.Height = Height;
                    storageChartControl.Width = Width;
                    old.Items = value;
                }
                else if (storageChartControl is Controls.StorageChart modern)
                    modern.Items = value;
            }
        }

        public StorageChart()
        {
            if (ApiInfo.IsUniversalApiContract6Present)
                storageChartControl = new Controls.StorageChart();
            else
                storageChartControl = new StorageChartUWPv5();

            Children.Add(storageChartControl);
        }

        public void Update(int index, bool v)
        {
            if (storageChartControl is StorageChartUWPv5 old)
                old.Update();
            else if (storageChartControl is Controls.StorageChart modern)
                modern.Update(index, v);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            storageChartControl.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }
    }
}
