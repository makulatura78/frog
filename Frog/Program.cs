using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Frog
{
     class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int[,] size = new int[16,10];
                Console.WriteLine("Старт");
                var startCoordinate = GetStartCoordinate();
                var goal = GetGoalCoordinate();
                var treesCoordinate = GetTrees();
                var path = FindPath(size, startCoordinate, goal, treesCoordinate);
                if (path.Item1 == null)
                    throw new Exception("Шляху немає");

                Console.WriteLine($"К-ть ходів {path.Item2}");
                Console.WriteLine("X : Y");
                foreach (var step in path.Item1)
                    Console.WriteLine(step.X + ":" + step.Y);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
        }

        // Дістаємо початкові координати 
        public static Coordinate GetStartCoordinate()
        {
            Console.Write("Ввведіть початкові координачи через кому від 1 до 16 та від 1 до 10: ");
            var start = Console.ReadLine().Split(",");
            if (start.Length < 2)
            {
                Console.Write("Некоректно введені дані спробуйте знову: ");
                start = Console.ReadLine().Split(",");
            }
            if (start.Length < 2)
                throw new InvalidOperationException("Некоректно введені дані!");
            int.TryParse(start[0], out int xstart);
            int.TryParse(start[1], out int ystart);
            if (xstart > 16 || ystart > 10)
                throw new ArgumentException("Некоректно введені дані!");
            return new Coordinate(xstart, ystart);
        }

        // Дістаємо кінцеві координати 
        public static Coordinate GetGoalCoordinate()
        {
            Console.Write("Ввведіть початкові координачи через кому від 1 до 16 та від 1 до 10: ");
            var goal = Console.ReadLine().Split(",");
            if (goal.Length < 2)
            {
                Console.Write("Некоректно введені дані спробуйте знову: ");
                goal = Console.ReadLine().Split(",");
            }
            if (goal.Length < 2)
                throw new InvalidOperationException("Некоректно введені дані!");
            int.TryParse(goal[0], out int xgoal);
            int.TryParse(goal[1], out int ygoal);
            if (xgoal > 16 || ygoal > 10)
                throw new ArgumentException("Некоректно введені дані!");
            return new Coordinate(xgoal, ygoal);
        }

        // Створюємо дерева
        public static List<Coordinate> GetTrees()
        {
            List<Coordinate> trees = new List<Coordinate>();
            Console.Write("Введіть к-ть дерев: ");
            var count = Console.ReadLine();
           
            var parse = int.TryParse(count, out int treesCount);
            if (!parse)
                throw new InvalidCastException("Невірний формат");
            for(int i = 0; i < treesCount; i++)
            {
                Console.Write("Введіть координати дерева через кому від 1 до 16 та від 1 до 10: ");
                var coordinate = Console.ReadLine().Split(",");
                if (coordinate.Length < 2)
                    throw new InvalidOperationException("Некоректно введені дані!");
                int.TryParse(coordinate[0], out int xtree);
                int.TryParse(coordinate[1], out int ytree);
                if (xtree > 16 || ytree > 10)
                    throw new ArgumentException("Некоректно введені дані!");
                if (trees.FirstOrDefault(x => x.X == xtree && x.Y == ytree) == null)
                    trees.Add(new Coordinate(xtree, ytree));
                else
                {
                    Console.Write("Дані координати зайняті введіть ще раз: ");
                    --i;
                }
            }
            return trees;
        }

        // Пошук шляху
        public static (List<Coordinate>,int) FindPath(int[,] field, Coordinate start, Coordinate goal,List<Coordinate> trees)
        {
            // Перевірені ноди.
            var closedSet = new Collection<Node>();
            // Потрібно перевірити
            var openSet = new Collection<Node>();
            // Стартова нода
            Node startNode = new Node()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                // Дістаємо ноду яка ймовірно найближче до фінішу
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();
                
                // Перевірка чи фініш
                if (currentNode.Position.Equals(goal))
                    return GetPathForNode(currentNode);
                // Перенесення з для перевірки в перевірене
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // Дістаємо сусідні
                var neighboursNodes =  GetNeighbours(currentNode, goal, field,trees);

                foreach (var neighbourNode in neighboursNodes)
                {
                    // Перевіряємо чи перевіряли
                    if (closedSet.Count(node => node.Position.Equals(neighbourNode.Position)) > 0)
                        continue;

                    // Дивимося  чи треба перевірити
                    var openNode = openSet.FirstOrDefault(node =>
                      node.Position.Equals(neighbourNode.Position));
                    // Якщо немає для перевірки то додаємо
                    if (openNode == null)
                        openSet.Add(neighbourNode);
                    else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            // якщо нічого не знайшли то повертаємо null
            return (null,0);
        }


        //Орієнтовна відстань
        private static int GetHeuristicPathLength(Coordinate from, Coordinate to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        //Дістаємо сусідні
        private static Collection<Node> GetNeighbours(Node pathNode,
                    Coordinate goal, int[,] field,List<Coordinate> trees)
        {
            var result = new Collection<Node>();

            // Задаємо координати по яких можемо рухатися.
            Coordinate[] neighbourPoints = new Coordinate[5];
            neighbourPoints[0] = new Coordinate(
                (pathNode.Position.X + 1) >= 17 ? 
                (pathNode.Position.X + 1) - 16 : 
                pathNode.Position.X + 1, pathNode.Position.Y + 2);
            neighbourPoints[1] = new Coordinate(
                (pathNode.Position.X + 1) >= 17 ? 
                (pathNode.Position.X + 1) - 16 : 
                pathNode.Position.X + 1, pathNode.Position.Y - 2);
            neighbourPoints[2] = new Coordinate((pathNode.Position.X + 2) >= 17 ? (pathNode.Position.X + 2) - 16 : (pathNode.Position.X + 2), pathNode.Position.Y + 1);
            neighbourPoints[3] = new Coordinate((pathNode.Position.X + 2) >= 17 ? (pathNode.Position.X + 2) - 16 : (pathNode.Position.X + 2), pathNode.Position.Y - 1);
            neighbourPoints[4] = new Coordinate((pathNode.Position.X + 3) >= 17 ? (pathNode.Position.X + 3) - 16 : (pathNode.Position.X + 3), pathNode.Position.Y);

            foreach (var point in neighbourPoints)
            {
                // Перевірки чи не вийшли за межі
                if (point.X < 0 || point.X > field.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y > field.GetLength(1))
                    continue;
                if (trees.FirstOrDefault(t => t.Equals(point)) != null)
                    continue;

                // Заповнення можливих ходів.
                var neighbourNode = new Node()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + 1,
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };

                result.Add(neighbourNode);
            }
            return result;
        }


        // Якщо шлях знайдено повертаємо для виведення
        private static (List<Coordinate>,int) GetPathForNode(Node pathNode)
        {
            var result = new List<Coordinate>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return (result,pathNode.PathLengthFromStart);
        }
    }
}
