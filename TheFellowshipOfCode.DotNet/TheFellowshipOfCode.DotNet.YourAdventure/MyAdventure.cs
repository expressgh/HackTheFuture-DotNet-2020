using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using HTF2020.Contracts;
using HTF2020.Contracts.Enums;
using HTF2020.Contracts.Models;
using HTF2020.Contracts.Models.Adventurers;
using HTF2020.Contracts.Models.Enemies;
using HTF2020.Contracts.Models.Party;
using HTF2020.Contracts.Requests;
using HTF2020.GameController.State;


namespace TheFellowshipOfCode.DotNet.YourAdventure
{
    public class MyAdventure : IAdventure
    {
        private readonly Random _random = new Random();

        public Task<Party> CreateParty(CreatePartyRequest request)
        {
            var party = new Party
            {
                Name = "My Party",
                Members = new List<PartyMember>()
            };

            for (var i = 0; i < request.MembersCount; i++)
            {
                party.Members.Add(new Fighter()
                {
                    Id = i,
                    Name = $"Member {i + 1}",
                    Constitution = 11,
                    Strength = 12,
                    Intelligence = 11
                });
            }

            return Task.FromResult(party);
        }

        public Task<Turn> PlayTurn(PlayTurnRequest request)
        {
            return PlayToEnd();

            Task<Turn> PlayToEnd()
            {
                return Task.FromResult(request.PossibleActions.Contains(TurnAction.WalkEast)
                    ? new Turn(TurnAction.WalkEast)
                    : new Turn(request.PossibleActions[_random.Next(request.PossibleActions.Length)]));
            }

            Task<Turn> Strategic()
            {
                const double goingEastBias = 0.35;
                const double goingSouthBias = 0.25;

                if (request.PossibleActions.Contains(TurnAction.Loot))
                {
                    return Task.FromResult(new Turn(TurnAction.Loot));
                }

                if (request.PossibleActions.Contains(TurnAction.Attack))
                {
                    return Task.FromResult(new Turn(TurnAction.Attack));
                }

                if (request.PossibleActions.Contains(TurnAction.WalkEast) && _random.NextDouble() > (1 - goingEastBias))
                {
                    return Task.FromResult(new Turn(TurnAction.WalkEast));
                }

                if (request.PossibleActions.Contains(TurnAction.WalkSouth) &&
                    _random.NextDouble() > (1 - goingSouthBias))
                {
                    return Task.FromResult(new Turn(TurnAction.WalkSouth));
                }

                return Task.FromResult(new Turn(request.PossibleActions[_random.Next(request.PossibleActions.Length)]));
            }

            void Main(string[] args)
            {
                var map = request.Map;
                var start = new Tile();
                var finish = new Tile();

                for (var j = 0; j >= map.Tiles.Rank; j++)
                {
                    for (var i = 0; i >= map.Tiles.Col; i++)
                    {
                        if (map.Tiles[j, i].GetType() = StartPoint)
                        {
                            start = map.Tiles[j, i];
                        }

                        if (map.Tiles[j, i].GetType() = FinishPoint)
                        {
                            finish = map.Tiles[j, i];
                        }
                    }
                }

                var activeTiles = new List<Tile>();
                activeTiles.Add(start);
                var visitedTiles = new List<Tile>();
                
                private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile)
                {
                    var possibleTiles = new List<Tile>()
                    {
                        new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                        new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                        new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                        new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                    };

                    possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

                    var maxX = map.First().Length - 1;
                    var maxY = map.Count - 1;

                    return possibleTiles
                        .Where(tile => tile.X >= 0 && tile.X <= maxX)
                        .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                        .Where(tile => map[tile.Y][tile.X] == ' ' || map[tile.Y][tile.X] == 'B')
                        .ToList();
                }
                
                while(activeTiles.Any())
                {
                    var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                    if(checkTile.X == finish.X && checkTile.Y == finish.Y)
                    {
                        Console.Log(We are at the destination!);
                        //We can actually loop through the parents of each tile to find our exact path which we will show shortly. 
                        return;
                    }

                    visitedTiles.Add(checkTile);
                    activeTiles.Remove(checkTile);

                    var walkableTiles = GetWalkableTiles(map, checkTile, finish);

                    foreach(var walkableTile in walkableTiles)
                    {
                        //We have already visited this tile so we don't need to do so again!
                        if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                            continue;

                        //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                        if(activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        {
                            var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                            if(existingTile.CostDistance > checkTile.CostDistance)
                            {
                                activeTiles.Remove(existingTile);
                                activeTiles.Add(walkableTile);
                            }
                        }else
                        {
                            //We've never seen this tile before so add it to the list. 
                            activeTiles.Add(walkableTile);
                        }
                    }
                }

                Console.WriteLine("No Path Found!");
            }
            }
        }
    }
}