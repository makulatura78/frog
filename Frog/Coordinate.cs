using System;
using System.Collections.Generic;
using System.Text;

namespace Frog
{

   public  class Coordinate
   {
        // Конструктор для створення нових координат
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Точка Х на площині по горизонталі
        public int X { get; set; }

        // Точка Y на площині по вертикалі
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Coordinate coordinate &&
                   X == coordinate.X &&
                   Y == coordinate.Y;
        }
    }
}
