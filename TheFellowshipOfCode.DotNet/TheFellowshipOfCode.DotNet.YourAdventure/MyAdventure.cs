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
                        if (map.Tiles[j, i] == "A")
                        {
                            start = map.Tiles[j, i];
                        }

                        if (map.Tiles[j, i] == "F")
                        {
                            finish = map.Tiles[j, i];
                        }
                    }
                }

                var activeTiles = new List<Tile>();
                activeTiles.Add(start);
                var visitedTiles = new List<Tile>();
            }
        }
    }
}