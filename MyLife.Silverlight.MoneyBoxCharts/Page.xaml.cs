using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Browser;
using System.Windows.Media;
using Visifire.Charts;

namespace MyLife.Silverlight.MoneyBoxCharts
{
    [ScriptableType]
    public partial class Page
    {
        public Page()
        {
            InitializeComponent();
        }

        [ScriptableMember]
        public void EarningByCategories(string data)
        {
            var categories = Deserializes(data);
            var chart = new Chart {Width = 500, Height = 300, View3D = true};
            chart.Titles.Add(new Title {Text = "Thu nhập theo danh mục"});
            var dataSeries = new DataSeries {RenderAs = RenderAs.Doughnut};

            foreach (var category in categories)
            {
                var dataPoint = new DataPoint
                                    {
                                        YValue = category.Earnings,
                                        AxisXLabel = category.Name,
                                    };
                dataSeries.DataPoints.Add(dataPoint);
            }

            chart.Series.Add(dataSeries);
            LayoutRoot.Children.Clear();
            LayoutRoot.Children.Add(chart);
        }

        [ScriptableMember]
        public void ExpenseByCategories(string data)
        {
            var categories = Deserializes(data);
            var chart = new Chart {Width = 500, Height = 300, View3D = true};
            chart.Titles.Add(new Title {Text = "Chi tiêu theo danh mục"});
            var dataSeries = new DataSeries {RenderAs = RenderAs.Doughnut};

            foreach (var category in categories)
            {
                var dataPoint = new DataPoint
                {
                    YValue = Math.Abs(category.Expenses),
                    AxisXLabel = category.Name,
                    //Color = BuildBrush(category.ColorHex)
                };
                dataSeries.DataPoints.Add(dataPoint);
            }

            chart.Series.Add(dataSeries);
            LayoutRoot.Children.Clear();
            LayoutRoot.Children.Add(chart);
        }

        [ScriptableMember]
        public void Balance(int earning, int expense)
        {
            var chart = new Chart {Width = 500, Height = 300, View3D = true};
            chart.Titles.Add(new Title {Text = "Cân bằng tài chính"});
            var dataSeries = new DataSeries {RenderAs = RenderAs.Column};

            dataSeries.DataPoints.Add(new DataPoint {YValue = earning, AxisXLabel = "Tổng thu nhập"});
            dataSeries.DataPoints.Add(new DataPoint {YValue = expense, AxisXLabel = "Tổng chi tiêu"});

            chart.Series.Add(dataSeries);
            LayoutRoot.Children.Clear();
            LayoutRoot.Children.Add(chart);
        }

        private static IList<Category> Deserializes(string json)
        {
            var jsSer = new DataContractJsonSerializer(typeof (IList<Category>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var categories = jsSer.ReadObject(ms) as IList<Category>;
            ms.Close();
            return categories;
        }

        private static Color ColorFromHex(string hex)
        {
            hex = hex.Replace("#", "");
            byte a = 255;
            var start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes 
            var r = byte.Parse(hex.Substring(start, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(start + 2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(start + 4, 2), NumberStyles.HexNumber);
            return Color.FromArgb(a, r, g, b);
        }

        /*private static GradientBrush BuildBrush(string hex)
        {
            //var gradBrush = new GradientBrush();
        }*/
    }
}