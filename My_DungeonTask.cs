using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class My_DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            //Путь до выхода
            var pathToExit = My_BfsTask.FindPaths(map, map.InitialPosition, new[] { map.Exit }).FirstOrDefault();
            //Если нет пути до выхода
            if (pathToExit == null)
                return new MoveDirection[0];

            //Если найденый путь до выхода содержит хоть один сундук
            if (map.Chests.Any(chest => pathToExit.ToList().Contains(chest)))
                return pathToExit.ToList().PointToDirection();

            //Находим кратчайший путь
            var chests = My_BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
            var result = chests.Select(x => Tuple.Create(
                x, My_BfsTask.FindPaths(map, x.Value, new[] { map.Exit }).FirstOrDefault()))
                .SelectShortest();
            //Если кратчайший путь не проходит ни через один сундук
            if (result == null) return pathToExit.ToList().PointToDirection();

            //Парсим каждую часть пути (до сундука и от него) в путь MoveDirection и соединяем
            return result.Item1.ToList().PointToDirection().Concat(
                result.Item2.ToList().PointToDirection())
                .ToArray();
        }
    }

        public static class ExtentionMetods
        {
            //Поиск минимального пути, к котором путь до сундука и от него до выхода
            //Суммарно будут кратчайшими
            public static Tuple<SinglyLinkedList<Point>, SinglyLinkedList<Point>>
            SelectShortest(this IEnumerable<Tuple<SinglyLinkedList<Point>, SinglyLinkedList<Point>>> items)
        {
            if (items.Count() == 0 || items.First().Item2 == null)
                return null;

            var min = int.MaxValue;
            var shortest = items.First();
            foreach (var e in items)
                if (e.Item1.Length + e.Item2.Length < min)
                {
                    min = e.Item1.Length + e.Item2.Length;
                    shortest = e;
                }
            return shortest;
        }

        //Перевод из последовательности точек к последовательность направлений
        public static MoveDirection[] PointToDirection( this List<Point> items)
        {
            var resultList = new List<MoveDirection>();
            if (items == null)
                return new MoveDirection[0];
            var itemsLength = items.Count;

            for (var i = itemsLength - 1; i > 0; i--)
            {
                resultList.Add(Direction(items[i], items[i - 1]));
            }
            return resultList.ToArray();
        }

        //Направление между двумя точками
        static MoveDirection Direction(Point firstPoint, Point secondPoint)
        {
            var dx = firstPoint.X - secondPoint.X;
            var dy = firstPoint.Y - secondPoint.Y;
            switch(dx)
            {
                case 1: return MoveDirection.Left;
                case -1: return MoveDirection.Right;
            }
            switch(dy)
            {
                case 1: return MoveDirection.Up;
                case -1: return MoveDirection.Down;     
            }
            throw new ArgumentException();
        }
 }
}
        