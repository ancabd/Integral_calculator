using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Integrale
{
    public partial class Modif_integrala : Form
    {
        public Modif_integrala()
        {
            InitializeComponent();

            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula.png";

            WpfExpression exp = new WpfExpression("");
            exp.OutputPng();

            pictureBox1.ImageLocation = fileName;
        }

        public Form1 f;

        private void button1_Click(object sender, EventArgs e)
        {
            //error checking
            if (textBox1.Text == "" || textBox2.Text == "") { MessageBox.Show("Nu ati introdus limitele intervalului!"); return; }
            if (richTextBox1.Text == "") { MessageBox.Show("Nu ati introdus expresia!"); return; }

            string expression = richTextBox1.Text, error;
            double a = double.Parse(textBox1.Text);

            expression = expression.Replace(" ", String.Empty);

            Expression thisExp = new Expression(expression, out error);
            if (error != "")
            {
                MessageBox.Show(error);
                return;
            }

            double x = thisExp.EvaluateAtP(a, out error);
            if (error != "")
            {
                MessageBox.Show(error);
                return;
            }

            //sending data to form 1
            f.a = double.Parse(textBox2.Text);
            f.b = double.Parse(textBox1.Text);
            f.expression = richTextBox1.Text;
            f.ok = true;

            //outputting the png
            WpfExpression exp = new WpfExpression(richTextBox1.Text);
            exp.OutputPngWithInterval(f.a, f.b);

            this.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula.png";

            WpfExpression exp = new WpfExpression(richTextBox1.Text);
            exp.OutputPng();

            pictureBox1.ImageLocation = fileName;
        }

        private void Modif_integrala_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.ok = true;
        }
    }
}
