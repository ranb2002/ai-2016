using System.Collections.Generic;
using System.Linq;
using System;

namespace CoveoBlitz.Bot
{
    public class Pathfinder
    {
        private Tile[][] Board;
        private static int PIKES_LIFE_COST = 10;
        private static int MOVE_LIFE_COST = 1;

        /// <summary>
        /// Creates a pathfinder.
        /// </summary>
        public Pathfinder(Tile[][] board)
        {
            Board = board;
        }

        /// <summary>
        /// Finds the shortest path from 'source' to 'destination' and returns the
        /// direction to take a single step towards that path.
        ///
        /// source and destination are in (row, column) positions.
        /// </summary>
        public string NavigateTowards(Pos source, Pos destination)
        {
            var path = ShortestPath(source, destination);
            if (path.Count > 0) return DirectionTowards(source, path[0]);
            return Direction.Stay;
        }

        /// <summary>
        /// Finds the shortest path from 'source' to 'destination' and returns the
        /// sequence of positions to follow to take the optimal path (excluding source,
        /// including destination).
        ///
        /// source and destination are in (row, column) positions.
        /// </summary>
        public IList<Pos> ShortestPath(Pos source, Pos destination)
        {
            var nodes = new HashSet<Pos>();
            nodes.Add(source);

            var distances = new Dictionary<Pos, int>();
            distances.Add(source, 0);

            var predecessors = new Dictionary<Pos, Pos>();

            while (nodes.Count > 0) {
                Pos u = nodes.OrderBy(n => distances[n]).First();
                nodes.Remove(u);

                var neighbors = GetNeighbors(u);

                if (neighbors.Contains(destination)) {
                    predecessors.Add(destination, u);
                    break;
                }

                foreach (var v in neighbors) {
                    var newDistV = distances[u] + 1 + (Board[v.x][v.y] == Tile.SPIKES ? PIKES_LIFE_COST : 0);
                    if(IsPassable(v))
                    {
                        if(!distances.ContainsKey(v)) 
                        {
                            distances.Add(v, newDistV);
                            predecessors.Add(v, u);
                            nodes.Add(v);
                        }
                        else if (distances[v] > newDistV)
                        {
                            distances[v] = newDistV;
                            predecessors[v] = u;
                        }
                    }


                }
            }

            return BuildPath (destination, predecessors);
        }

        private IList<Pos> BuildPath(Pos destination, IDictionary<Pos, Pos> predecessors)
        {
            var path = new List<Pos> ();
            var n = destination;

            while (predecessors.ContainsKey (n)) {
                path.Add (n);
                n = predecessors [n];
            }

            path.Reverse ();

            return path;
        }

        private string DirectionTowards(Pos src, Pos dst)
        {
            if (src.x > dst.x) return Direction.North;
            if (src.x < dst.x) return Direction.South;
            if (src.y > dst.y) return Direction.West;
            if (src.y < dst.y) return Direction.East;
            return Direction.Stay;
        }

        private bool IsPassable(Pos p)
        {
            var tile = Board[p.x][p.y];
            return tile == Tile.FREE || tile == Tile.SPIKES ||
                tile == Tile.HERO_1 || tile == Tile.HERO_2 ||
                tile == Tile.HERO_3 || tile == Tile.HERO_4;
        }

        public IList<Pos> GetNeighbors(Pos p)
        {
            var neighbors = new List<Pos>();
            if (p.x - 1 >= 0) neighbors.Add(new Pos(p.x - 1, p.y));
            if (p.x + 1 < Board.Length) neighbors.Add(new Pos(p.x + 1, p.y));
            if (p.y - 1 >= 0) neighbors.Add(new Pos(p.x, p.y - 1));
            if (p.y + 1 < Board[0].Length) neighbors.Add(new Pos(p.x, p.y + 1));
            return neighbors;
        }

        public bool IsMine(Pos p)
       {
           var tile = Board[p.x][p.y];
           return tile == Tile.GOLD_MINE_NEUTRAL || tile == Tile.GOLD_MINE_1 || tile == Tile.GOLD_MINE_2 ||
               tile == Tile.GOLD_MINE_3 || tile == Tile.GOLD_MINE_4;
       }

       public bool IsMyMine(Pos p, Tile t)
       {
           return Board[p.x][p.y] == t;
       }

       public void UpdateBoard(Tile[][] board)
       {
           Board = board;
       }
    }
}
