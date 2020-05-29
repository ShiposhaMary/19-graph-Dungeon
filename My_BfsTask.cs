
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class My_BfsTask
    {
        private static IEnumerable<Point> Steps(Point point)
        {
            yield return new Point(point.X - 1, point.Y);
            yield return new Point(point.X, point.Y - 1);
            yield return new Point(point.X + 1, point.Y);
            yield return new Point(point.X, point.Y + 1);
        }

        private static IEnumerable<Point> AllowedSt(Map map, Point point) =>
            Steps(point).Where(pt => map.InBounds(pt) &&
                                      map.Dungeon[pt.X, pt.Y] == MapCell.Empty);

        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
        {
            var chestsSet = new HashSet<Point>();
            var visited = new HashSet<Point>();
            var queue = new Queue<SinglyLinkedList<Point>>();
            foreach (var chest in chests)
                chestsSet.Add(chest);
            visited.Add(start);
            queue.Enqueue(new SinglyLinkedList<Point>(start));
            while (0 < queue.Count)
            {
                var point = queue.Dequeue();
                foreach (var step in AllowedSt(map, point.Value))
                {
                    if (visited.Contains(step)) continue;
                    var pathToChest = new SinglyLinkedList<Point>(step, point);
                    visited.Add(step);
                    queue.Enqueue(pathToChest);
                    if (chestsSet.Contains(step))
                        yield return pathToChest;
                }
            }
            yield break;
        }
    }
}