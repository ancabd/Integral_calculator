using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Integrale
{
    public class Graph
    {
        public Graph() 
        {
            points = new List<Tuple<float, float>>();
        }
        public Graph(ref Graphics gfx_, int h, int w)
        {
            gfx = gfx_;
            height = h;
            width = w;
            P = new Pen(Color.Black);
            BRed = new SolidBrush(Color.PaleVioletRed);
            PGrey = new Pen(Color.LightBlue);
            points = new List<Tuple<float, float>>();
            gfx.Clear(Color.White);
        }
        public Graph(ref Graphics gfx_, int h, int w, float scalingx, float scalingy)
        {
            scalingX = scalingx;
            scalingY = scalingy;
            gfx = gfx_;
            height = h;
            width = w;
            P = new Pen(Color.Black);
            BRed = new SolidBrush(Color.PaleVioletRed);
            PGrey = new Pen(Color.LightBlue);
            points = new List<Tuple<float, float>>();
            gfx.Clear(Color.White);
        }

        ~Graph()
        {
            gfx.Clear(Color.White);
        }

        public void AddOffsetX(float x)
        {
            offsetX += x;
        }
        public void AddOffsety(float y)
        {
            offsetY += y;
        }
        public void Clear()
        {
            gfx.Clear(Color.White);
        }
        public void DrawAxis()
        {
            for (int i = ((int)((-width/2-offsetX)/(10*scalingX)))*10; i * scalingX < width/2 - offsetX; i += 10)
            {
                for (int j = ((int)((-height/2-offsetY) / (10*scalingY))) * 10; j * scalingY < height/2 - offsetY; j += 10)
                {
                    gfx.DrawLine(PGrey, 0, height / 2 + offsetY + j * scalingY, width, height / 2 + offsetY + j * scalingY);
                    gfx.DrawLine(PGrey, width / 2 + offsetX + i * scalingX, 0, width / 2 + offsetX + i * scalingX, height);

                }
            }
            gfx.DrawLine(P, 0, height / 2 + offsetY, width, height / 2 + offsetY);
            gfx.DrawLine(P, width / 2 + offsetX, 0, width / 2 + offsetX, height);
        }
        public void AddPoint(float x, float y)
        {
            points.Add(new Tuple<float, float>(x, y));
        }

        public void DrawGraph()
        {
            float x, y, prevx, prevy;

            prevx = width/2 + points[0].Item1 * scalingX + offsetX;
            prevy = height/2 - points[0].Item2 * scalingY + offsetY;

            for (int i = 1; i < points.Count; i++)
            {
                x = width / 2 + points[i].Item1 * scalingX + offsetX;
                y = height/2 - points[i].Item2 * scalingY + offsetY;

                PointF[] ps = new PointF[4];
                ps[0] = new PointF(prevx, height / 2 + offsetY);
                ps[1] = new PointF(x, height / 2 + offsetY);
                ps[2] = new PointF(x, y);
                ps[3] = new PointF(prevx, prevy);

                gfx.FillPolygon(BRed, ps);
                gfx.DrawLine(P, x, y, prevx, prevy);
                prevx = x;
                prevy = y;
            }
        }
        public float scalingX, scalingY;
        private int height, width;
        private float offsetX = 0, offsetY = 0;
        private List<Tuple<float, float>> points;
        private Graphics gfx;
        private Pen P, PGrey;
        private Brush BRed;
    }
}
