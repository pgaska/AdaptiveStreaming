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

            List<Tuple<int, int>> xy = simulation.Simulate();

            for(int i=0; i<xy.Count; i++)
            {
                chart1.Series[0].Points.AddXY(xy[i].Item2, xy[i].Item1);
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }

            chart1.Series.Add("pasmo");
            chart1.Series["pasmo"].Color = Color.Green;

            for(int i=0; i<simulation.downloadPoints.Count; i++)
            {
                chart1.Series["pasmo"].Points.AddXY(simulation.downloadPoints[i].Item2, simulation.downloadPoints[i].Item1);
                chart1.Series["pasmo"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            }
        }
    }
}
