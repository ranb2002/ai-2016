// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoveoBlitz.Bot
{
    /// <summary>
    /// Bot
    ///
    /// This bot controls your player.
    /// </summary>
    public class Bot : ISimpleBot
    {
        private readonly Random random = new Random();
        private List<Pos> _bars;
        private List<Pos> _myMines;
        private List<Pos> _otherMines;
        private Tile _myMineID;
        private Tile _myHeroTile;

        private const string _name = "RAWR";
        private int _IDMyHero;
        private List<int> _othersHeroes;

        /// <summary>
        /// This will be run before the game starts
        /// </summary>
        public void Setup(GameState state)
        {
            ParseMap(state);
            Console.WriteLine("Coveo's C# RandomBot");
        }

        private void ParseMap(GameState state)
        {
            _othersHeroes = new List<int>();
            FindMyHero(state);

            switch(state.myHero.id)
            {
                case 1:
                    _myMineID = Tile.GOLD_MINE_1;
                    break;
                case 2:
                    _myMineID = Tile.GOLD_MINE_2;
                    break;

                case 3:
                    _myMineID = Tile.GOLD_MINE_3;
                    break;

                case 4:
                    _myMineID = Tile.GOLD_MINE_4;
                    break;
            }

            _bars = FindTile(state, Tile.TAVERN);
            _myMines = FindTile(state, _myMineID);
            _otherMines = FindTile(state, Tile.GOLD_MINE_NEUTRAL);
        }

        private void FindMyHero(GameState state)
        {
            for (int i = 0; i < state.heroes.Count; ++i)
            {
                if (state.heroes[i].name.Equals(_name))
                {
                    _IDMyHero = i;
                    _myMineID = Tile.GOLD_MINE_1 + i;
                    _myHeroTile = Tile.HERO_1 + 1;
                }
                else
                    _othersHeroes.Add(i);
            }
        }

        /// <summary>
        /// This will be run on each turns. It must return a direction fot the bot to follow
        /// </summary>
        /// <param name="state">The game state</param>
        /// <returns></returns>
        public string Move(GameState state)
        {
            var pathfinder = new Pathfinder (state.board);
            var nearestBar = FindNearestBar(state);
            var nearestOtherMine = FindNearestOtherMine(state);

            //return pathfinder.NavigateTowards(state.myHero.pos, nearestBar);
            return pathfinder.NavigateTowards(state.myHero.pos, nearestOtherMine);

            // TODO implement SkyNet here
            // Pathfinding example:
            // string direction = pathfinder.NavigateTowards(state.myHero.pos, new Pos(0, 0));
            string direction;

            switch (random.Next(0, 5)) {
                case 0:
                    direction = Direction.East;
                    break;

                case 1:
                    direction = Direction.West;
                    break;

                case 2:
                    direction = Direction.North;
                    break;

                case 3:
                    direction = Direction.South;
                    break;

                default:
                    direction = Direction.Stay;
                    break;
            }

            Console.WriteLine("Completed turn {0}, going {1}", state.currentTurn, direction);
            return direction;
        }

        /// <summary>
        /// This is run after the game.
        /// </summary>
        public void Shutdown()
        {
            Console.WriteLine("Done");
        }

        private List<Pos> FindTile(GameState state, Tile type)
        {
            List<Pos> liste = new List<Pos>();
            var board = state.board;

            //Trouver les tiles
            for (int i = 0; i < board.Length; ++i)
            {
                for (int j = 0; j < board.Length; ++j)
                {
                    if (board[i][j] == type)
                    {
                        liste.Add(new Pos(i, j));
                    }
                }
            }

            return liste;
        }

        private Pos FindNearestBar(GameState state)
        {
            //Trouver le bar le plus proche
            Pos nearestBar = new Pos(0,0);
            int nbMoves = 1000000000;
            var pathfinder = new Pathfinder(state.board);
            foreach(var bar in _bars)
            {
                var pathToBar = pathfinder.ShortestPath(state.myHero.pos, bar);
                if(pathToBar.Count() < nbMoves)
                {
                    nbMoves = pathToBar.Count();
                    nearestBar = bar;
                }
            }

            return nearestBar;
        }

        private Pos FindNearestOtherMine(GameState state)
        {
            //Trouver le bar le plus proche
            Pos nearestOtherMine = new Pos(0, 0);
            int nbMoves = 1000000000;
            var pathfinder = new Pathfinder(state.board);
            foreach (var mine in _otherMines)
            {
                var pathToMine = pathfinder.ShortestPath(state.myHero.pos, mine);
                if (pathToMine.Count() < nbMoves)
                {
                    nbMoves = pathToMine.Count();
                    nearestOtherMine = mine;
                }
            }

            return nearestOtherMine;
        }
    }
}
