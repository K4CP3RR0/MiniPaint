using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPaint2.Klasy
{
    class Plotno : IDrawable
    {
        public List<Linia> Linie { get; set; } = new();

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            // rysowanie obrazka
            foreach (var linia in Linie)
            {
                canvas.StrokeColor = linia.Kolor;
                canvas.StrokeSize = 2;
                canvas.DrawLine(linia.Start, linia.Koniec);

            }
        }
    }


    class Linia
    {
        public PointF Start { get; set; }

        public PointF Koniec { get; set; }

        public Color Kolor { get; set; } = Colors.Black;
    }
}
