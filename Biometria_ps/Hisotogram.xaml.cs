using LiveCharts;
using LiveCharts.Wpf;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Biometria_ps
{
    /// <summary>
    /// Interaction logic for Hisotogram.xaml
    /// </summary>
    public partial class Hisotogram : Window
    {
        public SeriesCollection SeriesCollectionR { get; set; }
        public SeriesCollection SeriesCollectionG { get; set; }
        public SeriesCollection SeriesCollectionB { get; set; }
        public SeriesCollection SeriesCollectionRGB { get; set; }
        public Hisotogram(Bitmap bm)
        {


            var histR = new int[256];
            var histG = new int[256];
            var histB = new int[256];
            var tmp = new System.Drawing.Color();
            for (int i = 0; i < 256; i++)
            {
                histB[i] = 0;
                histG[i] = 0;
                histR[i] = 0;
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    tmp = bm.GetPixel(i, j);
                    histR[tmp.R]++;
                    histG[tmp.G]++;
                    histB[tmp.B]++;
                }
            }
            SeriesCollectionR = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "R",
                    Values = new ChartValues<int>()
                }
            };
            SeriesCollectionG = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "G",
                    Values = new ChartValues<int>()
                }
            };
            SeriesCollectionB = new SeriesCollection
            {
            new StackedAreaSeries
            {
                Title = "B",
                Values = new ChartValues<int>()
            }
            };
            SeriesCollectionRGB = new SeriesCollection
            { 
            new StackedAreaSeries
                {
                    Title = "RGB/3",
                    Values = new ChartValues<int>()
                },
            };

            for (int i = 0; i < 255; i++)
            {
                SeriesCollectionR[0].Values.Add(histR[i]);
                SeriesCollectionG[0].Values.Add(histG[i]);
                SeriesCollectionB[0].Values.Add(histB[i]);
                SeriesCollectionRGB[0].Values.Add((histR[i]+histG[i]+histB[i])/3);
            }


            DataContext = this;

            InitializeComponent();


        }
    }
}
