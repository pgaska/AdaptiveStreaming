using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptiveStreaming
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Simulation simulation = new Simulation();

            List<Tuple<double, double>> xy = simulation.Simulate();

            chart1.Series.Add("Bufor");
            chart1.Series["Bufor"].Color = Color.Green;

            for (int i=0; i<xy.Count; i++)
            {
                chart1.Series["Bufor"].Points.AddXY(xy[i].Item2, xy[i].Item1);
                chart1.Series["Bufor"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            chart1.Series.Add("Pasmo");
            chart1.Series["Pasmo"].Color = Color.Red;

            for(int i=0; i<simulation.downloadPoints.Count; i++)
            {
                chart1.Series["Pasmo"].Points.AddXY(simulation.downloadPoints[i].Item2, simulation.downloadPoints[i].Item1);
                chart1.Series["Pasmo"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            chart1.Series.Add("Przeplywnosc");
            chart1.Series["Przeplywnosc"].Color = Color.BlueViolet;

            for (int i = 0; i < simulation.downloadPoints.Count; i++)
            {
                chart1.Series["Przeplywnosc"].Points.AddXY(simulation.bitRatePoints[i].Item2, simulation.bitRatePoints[i].Item1);
                chart1.Series["Przeplywnosc"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
        }
    }
}
