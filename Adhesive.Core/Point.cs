using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adhesive.Core
{
    public class Point
    {
        public int X;
        public int Y;

        public Point()
        { }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(Adhesive.Core.Point point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }

        public static implicit operator Adhesive.Core.Point(System.Drawing.Point point)
        {
            return new Adhesive.Core.Point(point.X, point.Y);
        }

    }
}
