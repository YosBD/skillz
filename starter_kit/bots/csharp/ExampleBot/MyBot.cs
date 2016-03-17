using Pirates;
using System.Collections.Generic;

namespace Skillz
{
    public class Bot : IPirateBot
    {
        public void DoTurn(IPirateGame game)
        {
            Pirate pirate = game.GetMyPirate(0);
            Collector(game, pirate);
        }

        private void Collector(IPirateGame game, Pirate pirate)
        {
            if (TryAttack(game, pirate))
            {
                return;
            }
            Location destination;
            int moves;
            if (pirate.HasTreasure)
            {
                destination = pirate.InitialLocation;
                moves = 1;
            }
            else
            {
                destination = GetNearestTreasure(game, pirate).Location;
                moves = 6;
            }
            Goto(game, pirate, destination, moves);
        }

        private void Goto(IPirateGame game, Pirate pirate, Location destination, int moves)
        {
            List<Location> possibleLocations = game.GetSailOptions(pirate, destination, moves);
            if (possibleLocations.Count > 0)
                game.SetSail(pirate, possibleLocations[0]);
        }

        private bool TryAttack(IPirateGame game, Pirate pirate)
        {
            foreach (var enemy in game.EnemyPirates())
            {
                if (game.InRange(pirate, enemy))
                {
                    game.Attack(pirate, enemy);
                    return true;
                }
            }
            return false;
        }

        private Treasure GetNearestTreasure(IPirateGame game, Pirate pirate)
        {
            Treasure nearest = null;
            int minDist = 0;
            foreach (var treasure in game.Treasures())
            {
                int dist = game.Distance(pirate, treasure);
                if (nearest == null || dist < minDist)
                {
                    nearest = treasure;
                    minDist = dist;
                }
            }
            return nearest;
        }
    }
}
