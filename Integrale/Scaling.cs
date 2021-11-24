using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integrale
{
    public partial class Scaling : Form
    {
        public Scaling()
        {
            InitializeComponent();
        }
        public Graph graph;

        public void ResetScaling()
        {
            trackBar1.Value = trackBar2.Value = trackBar3.Value = 7;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (graph != null)
            {
                graph.scalingX = trackBar1.Value;
                graph.Clear();
                graph.DrawAxis();
                graph.DrawGraph();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (graph != null)
            {
                graph.scalingY = trackBar2.Value;
                graph.Clear();
                graph.DrawAxis();
                graph.DrawGraph();
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (graph != null)
            {
                graph.scalingX = trackBar3.Value;
                graph.scalingY = trackBar3.Value;
                trackBar1.Value = trackBar2.Value = trackBar3.Value;
                graph.Clear();
                graph.DrawAxis();
                graph.DrawGraph();
            }
        }
    }
}
