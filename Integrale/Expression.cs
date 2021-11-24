using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrale
{
    class Expression
    {
        public Expression() { }
        public Expression(string exp, out string error) 
        {
            error = "";

            expression = exp;
            expression = expression.Replace(" ", String.Empty);

            expLength = expression.Length;
            next = new int[expLength];
            prev = new int[expLength];

            //init resultExpression
            resultExpression = new double[expLength][];
            for (int i = 0; i < expLength; ++i)
                resultExpression[i] = new double[expLength];

            //init paranthesis places
            if (!InitParantPlace(ref expression, ref next, ref prev))
                error = "Nu e valida expresia!";

            //parse all numbers
            nrbeg = new int[expLength]; nrend = new int[expLength];
            for (int i = 0; i < expLength; ++i)
            {
                nrbeg[i] = nrend[i] = -1;
                if (expression[i] >= '0' && expression[i] <= '9')
                {
                    double x;
                    int end;
                    x = ParseNumber(ref expression, i, out end);
                    resultExpression[i][end] = x;
                    nrbeg[end] = i;
                    nrend[i] = end;
                    i = end;
                }
                else if (expression[i] == '\\')
                {
                    if (i + 2 < expLength && expression.Substring(i + 1, 2) == "pi")
                    {
                        nrbeg[i + 2] = i;
                        nrend[i] = i + 2;
                        resultExpression[i][i + 2] = Math.PI;
                        i += 2;
                    }
                    else if (i + 1 < expLength && expression[i + 1] == 'e')
                    {
                        nrbeg[i + 1] = i;
                        nrend[i] = i + 1;
                        resultExpression[i][i + 1] = Math.E;
                        i ++;
                    }
                }
            }

        }
       
        public double EvaluateAtP(double point, out string error)
        {
            p = point;
            //resetting calculated parts
            calcbeg = new int[expLength]; Array.Copy(nrbeg, calcbeg, expLength);
            calcend = new int[expLength]; Array.Copy(nrend, calcend, expLength);

            //put value instead of x
            for (int j = 0; j < expLength; ++j)
                if (expression[j] == 'x')
                {
                    resultExpression[j][j] = p;
                    calcbeg[j] = calcend[j] = j;
                }

            if (EvalExpression(0, expLength - 1, out error)) return resultExpression[0][expLength - 1];
            else return -1;
        }

        private string expression;
        private double p;
        private int expLength;
        

        //private variables used just in this class
        double[][] resultExpression;

        int[] calcbeg, calcend; //beginning and end of a calculated subsequence
        int[] next, prev; //next and previous paranthesis index
        int[] nrbeg, nrend; //beginning nd end of a number

        //private methods
        bool EvalExpression(int beg, int fin, out string error)
        {
            //paranthesis
            error = "";
            for (int i = beg; i <= fin; i++)
            {
                if (expression[i] == '(')
                {
                    if (!EvalExpression(i + 1, next[i] - 1, out error)) return false;
                    resultExpression[i][next[i]] = resultExpression[i + 1][next[i] - 1];
                    calcbeg[next[i]] = i;
                    calcend[i] = next[i];
                    i = next[i];
                }
            }

            //functions
            for (int i = beg; i <= fin; ++ i)
            {
                try
                {
                    if (expression[i] == '\\')
                    {
                        if (!CalcFunction(ref i, out error)) return false;
                    }
                }
                catch (Exception e) { error = e.Message + " Expresie invalida::functii"; return false; }
            }

            //exponents
            for (int i = beg; i <= fin; ++i)
            {
                try
                {
                    if (expression[i] == '^')
                    {
                        if (!CalcOperation(ref i, out error)) return false;
                    }
                }
                catch (Exception e) { error = e.Message + " Expresie invalida::exponenti"; return false; }
            }

            //multiplication and division
            for (int i = beg; i <= fin; ++i)
            {
                try
                {
                    if (expression[i] == '*' || expression[i] == '/')
                    {
                        if (!CalcOperation(ref i, out error)) return false;
                    }
                }
                catch (Exception e) {error = e.Message + " Expresie invalida::inmultire & impartire"; return false; }

            }

            //addition and subtraction
            for (int i = beg; i <= fin; ++i)
            {
                try
                {
                    if (expression[i] == '+' || expression[i] == '-')
                    {
                        if (expression[i] == '-' && (i == 0 || expression[i - 1] == '('))
                        {
                            if (calcend[i + 1] == -1)
                            {
                                error = "Expresie invalida::adunare & scadere";
                                return false;
                            }
                            resultExpression[i][calcend[i + 1]] = -1 * resultExpression[i + 1][calcend[i + 1]];
                            calcend[i] = calcend[i + 1];
                            calcbeg[calcend[i]] = i;
                        }
                        else if (!CalcOperation(ref i, out error)) return false;
                    }
                }
                catch (Exception e) { error= e.Message + " Expresie invalida::adunare & scadere"; return false; }
            }
            return true;
        }

        bool CalcOperation(ref int i, out string error) //evaluates a given operation +-*/^
        {
            error = "";

            double first, second;
            int begExp, endExp;

            if (calcbeg[i - 1] == -1 || calcend[i + 1] == -1)
            {
                error = "Expresie invalida:: " + expression[i];
                return false;
            }

            begExp = calcbeg[i - 1];
            endExp = calcend[i + 1];
            first = resultExpression[begExp][i - 1];
            second = resultExpression[i + 1][endExp];

            switch(expression[i])
            {
                case '+':
                    resultExpression[begExp][endExp] = first + second;
                    break;
                case '-':
                    resultExpression[begExp][endExp] = first - second;
                    break;
                case '*':
                    resultExpression[begExp][endExp] = first * second;
                    break;
                case '/':
                    resultExpression[begExp][endExp] = first / second;
                    break;
                case '^':
                    resultExpression[begExp][endExp] = Math.Pow(first,second);
                    break;
                default:
                    error = "Expresie invalida";
                    return false;
            }

            calcbeg[endExp] = begExp;
            calcend[begExp] = endExp;

            i = endExp;
            return true;
        }
        private bool CalcFunction(ref int i, out string error) //evaluates a function sin, cos, arcsin, arccos
        {
            error = "";
            double x;
            int begExp = i, endExp;
            if (i + 5 < expression.Length && expression.Substring(i+1, 4) == "sqrt")//sqrt
            {
                i += 5;
                if (expression[i] != '('){ error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];

                if (x < 0){ error = "Radical din numar negativ!"; return false; }

                resultExpression[begExp][endExp] = Math.Sqrt(x);
            }
            else if (i + 4 < expression.Length && expression.Substring(i + 1, 3) == "sin")//sin
            {
                i += 4;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Sin(x);
            }
            else if (i + 4 < expression.Length && expression.Substring(i + 1, 3) == "cos")//cos
            {
                i += 4;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Cos(x);
            }
            else if (i + 3 < expression.Length && expression.Substring(i + 1, 2) == "tg")//tg
            {
                i += 3;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Tan(x);
            }
            else if (i + 4 < expression.Length && expression.Substring(i + 1, 3) == "ctg")//ctg
            {
                i += 4;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = 1/Math.Tan(x);
            }
            else if (i + 7 < expression.Length && expression.Substring(i + 1, 6) == "arcsin")//arcsin
            {
                i += 7;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Asin(x);
            }
            else if (i + 7 < expression.Length && expression.Substring(i + 1, 6) == "arccos")//arccos
            {
                i += 7;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Acos(x);
            }
            else if (i + 6 < expression.Length && expression.Substring(i + 1, 5) == "arctg")//arctg
            {
                i += 6;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Atan(x);
            }
            else if (i + 7 < expression.Length && expression.Substring(i + 1, 6) == "arcctg")//arcctg
            {
                i += 7;
                if (expression[i] != '(') { error = "Expresie invalida!"; return false; }

                endExp = next[i];
                x = resultExpression[i][endExp];
                resultExpression[begExp][endExp] = Math.Atan(1/x);
            }
            else
            {
                error = "Expresie invalida:: nu exista functia";
                return false;
            }
            calcbeg[endExp] = begExp;
            calcend[begExp] = endExp;
            i = endExp;
            return true;
        }
        double ParseNumber(ref string s, int beg, out int end)
        {
            int i = beg;

            while (i < s.Length && s[i] >= '0' && s[i] <= '9')
                ++i;

            end = i - 1;
            if (i >= s.Length || s[i] != '.') return int.Parse(s.Substring(beg, end - beg + 1));

            ++i;
            while (i < s.Length && s[i] >= '0' && s[i] <= '9')
                ++i;

            end = i - 1;
            return double.Parse(s.Substring(beg, end - beg + 1));
        }


        private bool InitParantPlace(ref string exp, ref int[] next, ref int[] prev)
        {
            Stack<int> s = new Stack<int>();
            int l = exp.Length, front;
            for (int i = 0; i < l; ++i)
                if (exp[i] == '(')
                    s.Push(i);
                else if (exp[i] == ')')
                {
                    if (s.Count != 0)
                    {
                        front = s.Pop();
                        next[front] = i;
                        prev[i] = front;
                    }
                    else return false;
                }
            if (s.Count != 0) return false;
            return true;
        }

        
    }
}
