using System;
using System.Collections.Generic;
using System.Text;

namespace Frog
{
    public class Node
    {
        // Координати
        public Coordinate Position { get; set; }

        //Шлях від старту якщо > шляху нумає
        public int PathLengthFromStart { get; set; }

        // Звідки прийшли.
        public Node CameFrom { get; set; }

        // Приблизно до фінішу .
        public int HeuristicEstimatePathLength { get; set; }

        // Приблизне до фінішу .
        public int EstimateFullPathLength
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
    }
}
