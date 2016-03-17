using Pirates;
using System.Collections.Generic;

namespace Skillz
{
    public class Bot : IPirateBot
    {
        private static int left;

        private static List<Treasure> targets;

        private static void InitTurn()
        {
            left = 6;
            targets = new List<Treasure>();
        }

        public void DoTurn(IPirateGame game)
        {
            InitTurn();
            foreach (var pirate in game.MyPiratesWithTreasures())
            {
                Return(game, pirate);
            }
            var targets = game.EnemyPiratesWithTreasures();
            if (targets.Count > 0)
            {
                Goto(game, GetNearestPirate(game, targets[0]), targets[0].Location, 6);
            }
            else
            {
                foreach (var pirate in game.MyPiratesWithoutTreasures())
                {
                    Collect(game, pirate, 2);
                }
            }
        }

        private static void Collect(IPirateGame game, Pirate pirate, int moves)
        {
            Treasure treasure = GetNearestTreasure(game, pirate);
            if (treasure == null)
            {
                return;
            }
            targets.Add(treasure);
            Goto(game, pirate, treasure.Location, moves);
        }
        private static void Return(IPirateGame game, Pirate pirate)
        {
            Goto(game, pirate, pirate.InitialLocation, 1);
        }

        private static void Goto(IPirateGame game, Pirate pirate, Location destination, int moves)
        {
            if (TryAttack(game, pirate))
            {
                return;
            }
            if (moves > left)
            {
                moves = left;
            }
            List<Location> possibleLocations = game.GetSailOptions(pirate, destination, moves);
            foreach (var location in possibleLocations)
            {
                if (game.GetPirateOn(location) == null)
                {
                    game.SetSail(pirate, location);
                    left -= moves;
                    return;
                }
            }
        }

        private static bool TryAttack(IPirateGame game, Pirate pirate)
        {
            if (pirate.HasTreasure || pirate.ReloadTurns > 0)
            {
                return false;
            }
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

        private static Treasure GetNearestTreasure(IPirateGame game, Pirate pirate)
        {
            Treasure nearest = null;
            int minDist = 0;
            foreach (var treasure in game.Treasures())
            {
                if (targets.IndexOf(treasure) < 0)
                {
                    int dist = game.Distance(pirate, treasure);
                    if (nearest == null || dist < minDist)
                    {
                        nearest = treasure;
                        minDist = dist;
                    }
                }
            }
            return nearest;
        }

        private static Pirate GetNearestPirate(IPirateGame game, Pirate enemy)
        {
            Pirate nearest = null;
            int minDist = 0;
            foreach (var pirate in game.MyPiratesWithoutTreasures())
            {
                int dist = game.Distance(pirate, enemy);
                if (nearest == null || dist < minDist)
                {
                    nearest = pirate;
                    minDist = dist;
                }
            }
            return nearest;
        }

        /*private static Pirate GetNearestEnemy(IPirateGame game, Pirate pirate)
        {
            Pirate nearest = null;
            int minDist = 0;
            foreach (var enemy in game.EnemyPirates())
            {
                int dist = game.Distance(pirate, enemy);
                if (nearest == null || dist < minDist)
                {
                    nearest = enemy;
                    minDist = dist;
                }
            }
            return nearest;
        }

        private static void Protect(IPirateGame game, Pirate pirate, Pirate who, int moves)
        {
            if (pirate.IsLost || pirate.TurnsToSober > 0)
            {
                return;
            }
            Location destination;
            if (game.Distance(pirate, who) > 5)
            {
                destination = who.Location;
            }
            else
            {
                destination = GetNearestEnemy(game, who).Location;
            }
            Goto(game, pirate, destination, moves);
        }*/
    }
}