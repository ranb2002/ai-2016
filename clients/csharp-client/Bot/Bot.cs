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

        //constantes
        private static int COST_OF_BEER = 1;
        private static int GOBELIN_LIFE_COST = 25;
        private static int DEFENSE_LIFE_COST = 20;
        private static int PIKES_LIFE_COST = 10;
        private static int BEER_LIFE_GAIN = 25;
        private static int MOVE_LIFE_COST = 1;
        private static int MAX_LIFE = 100;

        private List<Pos> _bars;
        private List<Pos> _myMines;
        private List<Pos> _otherMines;
        private Tile _myMineID;
        private Tile _myHeroTile;
        private Pathfinder _pathFinder;
        private int _numPlayers;

        private const string _name = "RAWR";
        private int _IDMyHero;
        private List<int> _othersHeroes;

        /// <summary>
        /// This will be run before the game starts
        /// </summary>
        public void Setup(GameState state)
        {
            _pathFinder = new Pathfinder(state.board);
            ParseMap(state);
            Console.WriteLine("RAWR's - C# - RAWRBot v.1.1");
        }

        private void ParseMap(GameState state)
        {
            _othersHeroes = new List<int>();
            FindMyHero(state);

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
            _pathFinder.UpdateBoard(state.board);
            UpdateMines(state);

            var nearestBar = FindNearestBar(state);
            var nearestOtherMine = FindNearestOtherMine(state);
            var nearestHero = FindNearestHero(state);

            if (IsMyLifeAtRisk(state, nearestOtherMine) && state.myHero.gold >= COST_OF_BEER)
            {
                return _pathFinder.NavigateTowards(state.myHero.pos, nearestBar);
            }
            else if (IsBarInMyRange(state) && state.myHero.life < (MAX_LIFE - MOVE_LIFE_COST) && state.myHero.gold >= COST_OF_BEER)
            {
                return _pathFinder.NavigateTowards(state.myHero.pos, nearestBar);
            }
            else if (nearestHero.mineCount >= 3 && nearestHero.life + DEFENSE_LIFE_COST < state.myHero.life)
            {
                return _pathFinder.NavigateTowards(state.myHero.pos, nearestHero.pos);
            }
            return _pathFinder.NavigateTowards(state.myHero.pos, nearestOtherMine);
        }

        /// <summary>
        /// This is run after the game.
        /// </summary>
        public void Shutdown(GameState state)
        {
            int myGold = state.myHero.gold;

            int numBestGold = 0;
            string nameBestGold = "";

            Hero p;

            //Look who win this round
            for (int i = 0; i < _numPlayers; ++i)
            {
                p = state.heroes[i];

                if(p.gold >= numBestGold)
                {
                    numBestGold = p.gold;
                    nameBestGold = p.name;
                }
            }

            if (myGold == numBestGold)
                Console.WriteLine("RAWR's Bot Win this round with " + myGold + " golds!");
            else
                Console.WriteLine(nameBestGold + "'s Bot Win this round with " + numBestGold + " golds against " + myGold + " golds for RAWR's Bot!");
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
            foreach(var bar in _bars)
            {
                var pathToBar = _pathFinder.ShortestPath(state.myHero.pos, bar);
                int pathCost = PathCost(state, pathToBar);
                if (pathCost < nbMoves)
                {
                    nbMoves = pathCost;
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
            foreach (var mine in _otherMines)
            {
                var pathToMine = _pathFinder.ShortestPath(state.myHero.pos, mine);
                int pathCost = PathCost(state, pathToMine);
                if (pathCost < nbMoves)
                {
                    nbMoves = pathCost;
                    nearestOtherMine = mine;
                }
            }

            return nearestOtherMine;
        }

        private Hero FindNearestHero(GameState state)
        {
            //Trouver le bar le plus proche
            Hero nearestHero = new Hero();
            int nbMoves = 1000000000;
            foreach (var hero in state.heroes)
            {
                if(hero.id != state.myHero.id)
                {
                    var pathToHero = _pathFinder.ShortestPath(state.myHero.pos, hero.pos);
                    int pathCost = PathCost(state, pathToHero);
                    if (pathCost < nbMoves)
                    {
                        nbMoves = pathCost;
                        nearestHero = hero;
                    }
                }
            }

            return nearestHero;
        }

        private bool IsMyLifeAtRisk(GameState state, Pos nearestOtherMine)
        {
            var path = _pathFinder.ShortestPath(state.myHero.pos, nearestOtherMine);

            if (PathCost(state, path) * MOVE_LIFE_COST + GOBELIN_LIFE_COST >= state.myHero.life)
            {
                return true;
            }

            return false;
        }

        private int PathCost(GameState state, IList<Pos> listPos)
        {
            int cost = listPos.Count;
            var board = state.board;

            foreach (Pos p in listPos)
                cost += state.board[p.x][p.y] == Tile.SPIKES ? PIKES_LIFE_COST : 0;

            return cost;
        }

        private bool IsBarInMyRange(GameState state)
        {
            var neighbors = _pathFinder.GetNeighbors(state.myHero.pos);

            foreach(var neighbor in neighbors)
            {
                if(state.board[neighbor.x][neighbor.y] == Tile.TAVERN)
                {
                    return true;
                }
            }

            return false;
        }

        //Function that return true if at least one player is dead and lost all his mines
        private bool IsDeadPlayer(GameState state)
        {
            Hero h;
            for (int i = 0; i < _numPlayers; ++i)
            {
                h = state.heroes[i];

                //Is the player on its spawnpos AND has no mines AND has 100 life
                if (h.pos.Equals(h.spawnPos) && h.mineCount == 0 && h.life == 100)
                    return true;
            }

            return false;
        }

        private void UpdateMines(GameState state)
        {
            List<Pos> posAround = new List<Pos>();
            _numPlayers = state.heroes.Count;

            //We need to update all mines as at least one player is dead
            if (IsDeadPlayer(state))
            {
                List<Pos> allMinesPos = new List<Pos>();
                allMinesPos.AddRange(_myMines);
                allMinesPos.AddRange(_otherMines);

                _myMines.Clear();
                _otherMines.Clear();

                int numMines = allMinesPos.Count;
                var board = state.board;
                Pos p;

                for (int i = 0; i < numMines; ++i)
                {
                    p = allMinesPos[i];
                    if (state.board[p.x][p.y] == _myMineID)
                        _myMines.Add(p);
                    else
                        _otherMines.Add(p);
                }
            }
            else
            {
                //We can update only mines around each players
                //  Search all positions around players
                for (int i = 0; i < _numPlayers; ++i)
                    posAround.AddRange(_pathFinder.GetNeighbors(state.heroes[i].pos));

                int numPos = posAround.Count;
                Pos p;

                bool isMyMine;
                bool inMyMines;

                //  Update all mines in positions
                for (int i = 0; i < numPos; ++i)
                {
                    p = posAround[i];

                    //Is pos a mine?
                    if (_pathFinder.IsMine(p))
                    {
                        isMyMine = _pathFinder.IsMyMine(p, _myMineID);
                        inMyMines = _myMines.Contains(p);

                        //Is it my new mine?
                        if (isMyMine && !inMyMines)
                        {
                            _myMines.Add(p);
                            _otherMines.Remove(p);
                        } //Is it my lost mine?
                        else if (!isMyMine && inMyMines)
                        {
                            _myMines.Remove(p);
                            _otherMines.Add(p);
                        }
                    }
                }
            }
        }
    }
}
