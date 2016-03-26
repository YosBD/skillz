using Pirates;
using System.Collections.Generic;

namespace Skillz
{
    public class Bot : IPirateBot
    {
        private const int MAX_SPEED = 6;

        private const int COLLECT_SPEED = 3;

        private static int left;

        private static Pirate chaser;

        private static List<Treasure> targets;

        private static void InitTurn()
        {
            left = MAX_SPEED;
            chaser = null;
            targets = new List<Treasure>();
        }

        public void DoTurn(IPirateGame game)
        {
            InitTurn();
            Return(game);
            Chase(game);
            foreach (var pirate in game.MyPiratesWithoutTreasures())
            {
                if (pirate != chaser)
                {
                    Collect(game, pirate, COLLECT_SPEED);
                }
            }
        }

        private static void Return(IPirateGame game)
        {
            foreach (var pirate in game.MyPiratesWithTreasures())
            {
                Goto(game, pirate, pirate.InitialLocation, pirate.CarryTreasureSpeed);
            }
        }

        private static void Chase(IPirateGame game)
        {
            var targets = game.EnemyPiratesWithTreasures();
            if (targets.Count > 0)
            {
                chaser = GetNearestPirate(game, targets[0]);
                if (chaser == null)
                {
                    return;
                }
                Goto(game, chaser, targets[0].Location, MAX_SPEED);
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

        private static void Goto(IPirateGame game, Pirate pirate, Location destination, int moves)
        {
            if (pirate.TurnsToSober > 0 || TryDefend(game, pirate) || TryAttack(game, pirate))
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
            foreach (var pirate in game.MySoberPirates())
            {
                if (pirate.HasTreasure || pirate.ReloadTurns > 0)
                {
                    continue;
                }
                int dist = game.Distance(enemy, pirate);
                if (nearest == null || dist < minDist)
                {
                    nearest = pirate;
                    minDist = dist;
                }
            }
            return nearest;
        }

        private static bool IsSafe(IPirateGame game, Pirate pirate)
        {
            int space = -1;
            foreach (var enemy in game.EnemySoberPirates())
            {
                if (enemy.HasTreasure || enemy.ReloadTurns > 0)
                {
                    continue;   
                }
                int dist = game.Distance(pirate, enemy);
                if (space < 0 || dist < space)
                {
                    space = dist;
                }
            }
            return space < 0 || space > pirate.AttackRadius;
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

        private static bool TryDefend(IPirateGame game, Pirate pirate)
        {
            if (game.GetDefenseExpirationTurns() > 0 || game.GetDefenseReloadTurns() > 0 || IsSafe(game, pirate))
            {
                return false;
            }
            game.Defend(pirate);
            return true;
        }

        /*private static Pirate GetNearestReturn(IPirateGame game)
        {
            var targets = game.EnemyPiratesWithTreasures();
            Pirate target = null;
            int minDist = 0;
            foreach (var enemy in targets)
            {
                int dist = GetDistToReturn(game, enemy);
                if (target == null || dist < minDist)
                {
                    target = enemy;
                    minDist = dist;
                }
            }
            return target;
        }

        private static int GetDistToReturn(IPirateGame game, Pirate pirate)
        {
            return game.Distance(pirate, pirate.InitialLocation);
        }*/
    }
}
