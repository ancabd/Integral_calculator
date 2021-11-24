using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WpfMath;

namespace Integrale
{
    class WpfExpression
    {
        public WpfExpression() { }
        public WpfExpression(string _s)
        {
            s = _s;
        }

        public void OutputPng()
        {
            FormatIntegral();
            try
            {
                string latex = @" \int " + s + @"\,dx ";
                string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula.png";

                var parser = new TexFormulaParser();
                var formula = parser.Parse(latex);
                var pngBytes = formula.RenderToPng(25.0, 0.0, 0.0, "Arial");
                File.WriteAllBytes(fileName, pngBytes);
                
            }
            catch (Exception ex) { };
        }

        public void OutputPngWithInterval(double a, double b)
        {
            FormatIntegral();
            string latex = @" \int_{" + a + "}^{" + b + "}" + s + @"\,dx ";
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\formula_int.png";

            var parser = new TexFormulaParser();
            var formula = parser.Parse(latex);
            var pngBytes = formula.RenderToPng(25.0, 0.0, 0.0, "Arial");
            File.WriteAllBytes(fileName, pngBytes);
        }
        private void FormatIntegral()
        {
            string news = "";
            int[] next, prev, fracEnd;
            int length = s.Length;

            next = new int[length];
            prev = new int[length];
            fracEnd = new int[length];

            FindParanthesis(ref next, ref prev);
            FindFractions(ref fracEnd, in next, in prev);

            for (int i = 0; i < length; ++ i)
            {
                if (fracEnd[i] != 0)
                {
                    int mid = fracEnd[i];
                    string first, second;

                    first = s.Substring(i, mid - i);
                    second = s.Substring(mid + 1, fracEnd[mid] - mid);

                    if (first[0] == '(')
                    {
                        first = first.Remove(0, 1);
                        first = first.Remove(first.Length - 1, 1);
                    }
                    if (second[0] == '(')
                    {
                        second = second.Remove(0, 1);
                        second = second.Remove(second.Length - 1, 1);
                    }

                    news += @"\frac{" + first + "}{" + second + "}";
                    i = fracEnd[mid];
                }
                else if (s[i] == '*')
                {
                    news += @" \cdot ";
                }
                else if (s[i] == '\\') 
                {
                    if (i + 5 < length && s.Substring(i + 1, 4) == "sqrt") //sqrt
                    {   
                        int beginParant = i + 5;
                        int endParant = next[beginParant];
                        if (endParant != 0)
                        {
                            news += @"\sqrt{";
                            news += s.Substring(beginParant + 1, endParant - beginParant - 1) + "}";
                            i = endParant;
                        }
                        else news += s[i];
                    }
                    else if (i + 1 < length && s[i + 1] == 'e') //e
                    {
                        news += 'e';
                        ++i;
                    }
                    else news += s[i];
                }
                else news += s[i];
            }
            s = news;
        }

        private void FindFractions(ref int[] end, in int[] next, in int[] prev)
        {
            for (int i = 1; i < s.Length - 1; ++ i)
            {
                if (s[i] == '/')
                {
                    int begInd = 0, endInd = 0;

                    //finding befInd
                    if (s[i - 1] == ')')
                        begInd = prev[i - 1];
                    else if (s[i - 1] == 'x')
                        begInd = i - 1;
                    else if (s[i - 1] >= '0' && s[i - 1] <= '9')
                        ParseNumberFromEnd(out begInd, i - 1);

                    //finding endInd
                    if (s[i + 1] == '(')
                        endInd = next[i + 1];
                    else if (s[i + 1] == 'x')
                        endInd = i + 1;
                    else if (s[i + 1] >= '0' && s[i + 1] <= '9')
                        ParseNumberFromBeg(i + 1, out endInd);

                    if (endInd != 0)
                    {
                        end[begInd] = i;
                        end[i] = endInd;
                    }
                    
                }
            }
        }
        private void FindParanthesis(ref int[] next, ref int[] prev)
        {
            Stack<int> st = new Stack<int>();
            int l = s.Length, front;
            for (int i = 0; i < l; ++i)
                if (s[i] == '(')
                    st.Push(i);
                else if (s[i] == ')')
                {
                    if (st.Count != 0)
                    {
                        front = st.Pop();
                        next[front] = i;
                        prev[i] = front;
                    }
                }
            return;
        }

        private void ParseNumberFromEnd(out int beg, int end)
        {
            int i = end;

            while (i >= 0 && s[i] >= '0' && s[i] <= '9')
                --i;

            beg = i + 1;
            if (i < 0 || s[i] != '.') return;

            --i;
            while (i >= 0 && s[i] >= '0' && s[i] <= '9')
                --i;

            beg = i + 1;
            return;
        }
        void ParseNumberFromBeg(int beg, out int end)
        {
            int i = beg;

            while (i < s.Length && s[i] >= '0' && s[i] <= '9')
                ++i;

            end = i - 1;
            if (i >= s.Length || s[i] != '.') return;

            ++i;
            while (i < s.Length && s[i] >= '0' && s[i] <= '9')
                ++i;

            end = i - 1;
            return;
        }
        private string s;
    }
}
