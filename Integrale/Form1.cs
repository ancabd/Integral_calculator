using System;
using System.Drawing;
using System.Windows.Forms;

namespace Integrale
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        public double a, b;
        public string expression;
        int sign = 1;
        public bool ok
        {
            get { return _ok; }
            set
            {
                _ok = value;
                if (_ok == true)
                {
                    string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula_int.png";
                    pictureBox2.ImageLocation = fileName;
                }
            }
        }

        Graph graph;
        Scaling f;
        Graphics G;
        bool mouseDown = false;
        float pmouseX, pmouseY, mouseX, mouseY;
        bool _ok;
        private void button1_Click(object sender, EventArgs e)
        {
            if (ok == false) return;

            double precision = 1 / Math.Pow(10, trackBar1.Value);

            if (a > b) { double aux = a; a = b; b = aux; sign = -1; }

            string error = "";

            //solve integral

            double integral = 0;
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Nu ati introdus tipul de aproximare");
                return;
            }

            if (comboBox1.SelectedItem.ToString() == "Prima valoare")
                integral = SolveFirst(precision, out error);
            else if (comboBox1.SelectedItem.ToString() == "Ultima valoare")
                integral = SolveLast(precision, out error);
            else if (comboBox1.SelectedItem.ToString() == "Medie")
                integral = SolveMedium(precision, out error);

            if (error != "")
            {
                MessageBox.Show(error);
                return;
            }
            integral *= sign;

            label1.Text = "Valoarea integralei: " + integral.ToString("F" + trackBar1.Value.ToString());
            graph.Clear();
            graph.DrawAxis();
            graph.DrawGraph();

            if (f != null)
            {
                f.graph = graph;
                f.ResetScaling();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            G = pictureBox1.CreateGraphics();
            G.Clear(Color.White);

            expression = "x";
            a = 0;
            b = 1;
            ok = true;

            WpfExpression exp = new WpfExpression(expression);
            exp.OutputPngWithInterval(a, b);
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula_int.png";
            pictureBox2.ImageLocation = fileName;
            comboBox1.SelectedIndex = 0;
            //label1.BackColor = Color.Transparent;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            f = new Scaling();
            f.Show();
            f.graph = graph;
            f.SetDesktopLocation(Cursor.Position.X + 250, Cursor.Position.Y - 100);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                pmouseX = mouseX;
                pmouseY = mouseY;
                mouseX = MousePosition.X;
                mouseY = MousePosition.Y;

                if (graph != null)
                {
                    graph.AddOffsetX(mouseX - pmouseX);
                    graph.AddOffsety(mouseY - pmouseY);
                    graph.Clear();
                    graph.DrawAxis();
                    graph.DrawGraph();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Modif_integrala f = new Modif_integrala();
            ok = false;
            f.f = this;
            sign = 1;
            f.Show();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (!mouseDown)
            {
                mouseDown = true;
                mouseX = MousePosition.X;
                mouseY = MousePosition.Y;
            }  
        }
        private double SolveFirst(double precision, out string error)
        {
            error = "";

            int scalingX = 7, scalingY = 7;

            double integral = 0;

            Expression thisExp = new Expression(expression, out error);

            graph = new Graph(ref G, pictureBox1.Height, pictureBox1.Width, scalingX, scalingY);
            graph.DrawAxis();

            double i, x;
            int q, cnt = 1;
            if ((b - a) / precision <= 101) q = 1;
            else q = (int)Math.Floor((b - a) / precision / 100);

            for (i = a; i < b; i += precision)
            {
                x = thisExp.EvaluateAtP(i, out error);
                if (error != "") return 0;
                cnt--;
                if (cnt == 0)
                {
                    graph.AddPoint((float)i, (float)x);
                    cnt = q;
                }


                integral += x * precision;
            }

            //last point
            i -= precision;
            x = thisExp.EvaluateAtP(i, out error);
            if (error != "")return 0;

            graph.AddPoint((float)i, (float)x);

            integral += x * (b - i);
            return integral;
        }

        private double SolveMedium(double precision, out string error)
        {
            error = "";

            int scalingX = 7, scalingY = 7;

            double integral = 0;

            Expression thisExp = new Expression(expression, out error);

            graph = new Graph(ref G, pictureBox1.Height, pictureBox1.Width, scalingX, scalingY);
            graph.DrawAxis();

            double i, x = 0, y;
            int q, cnt = 1;
            if ((b - a) / precision <= 101) q = 1;
            else q = (int)Math.Floor((b - a) / precision / 100);

            //first point

            for (i = a; i < b; i += precision)
            {
                y = x;
                x = thisExp.EvaluateAtP(i, out error);
                if (error != "") return 0;
                cnt--;
                if (cnt == 0)
                {
                    graph.AddPoint((float)i, (float)x);
                    cnt = q;
                }

                if (i > a)
                    integral += (x+y)/2 * precision;
                
            }

            //last point
            y = x;

            i -= precision;
            x = thisExp.EvaluateAtP(i, out error);
            if (error != "") return 0;

            graph.AddPoint((float)i, (float)x);

            integral += (x+y)/2 * (b - i);
            return integral;
        }
        private double SolveLast(double precision, out string error)
        {
            error = "";

            int scalingX = 7, scalingY = 7;

            double integral = 0;

            Expression thisExp = new Expression(expression, out error);

            graph = new Graph(ref G, pictureBox1.Height, pictureBox1.Width, scalingX, scalingY);
            graph.DrawAxis();

            double i, x;
            int q, cnt = 1;
            if ((b - a) / precision <= 101) q = 1;
            else q = (int)Math.Floor((b - a) / precision / 100);

            for (i = b; i > a; i -= precision)
            {
                x = thisExp.EvaluateAtP(i, out error);
                if (error != "") return 0;
                cnt--;
                if (cnt == 0)
                {
                    graph.AddPoint((float)i, (float)x);
                    cnt = q;
                }


                integral += x * precision;
            }

            //last point
            i += precision;
            x = thisExp.EvaluateAtP(i, out error);
            if (error != "") return 0;

            graph.AddPoint((float)i, (float)x);

            integral += x * (i - a);
            return integral;
        }
    }
}
