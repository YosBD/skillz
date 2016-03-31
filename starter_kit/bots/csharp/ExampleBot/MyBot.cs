using Pirates;
using System.Collections.Generic;
using System.Linq;

namespace Skillz
{
    public class Bot : IPirateBot
    {
        private static int left;

        private static Pirate chaser;

        private static List<Treasure> treasures;

        private static void InitTurn(IPirateGame game)
        {
            left = game.GetActionsPerTurn();
            chaser = null;
            treasures = game.Treasures();
        }

        //public Treasure HigestValue(List<Treasure> list)
        //{
        //    int Max = -1000;
        //    Treasure find = list[0];

        //    foreach (Treasure t in list)
        //    {
        //        if (t.Value > Max)
        //        {
        //            Max = t.Value;
        //            find = t;
        //        }
        //    }
        //    return find;
        //}


        public void DoTurn(IPirateGame game)
        {
            InitTurn(game);
            Return(game);
            Chase(game);
            var collectors = GetCollectors(game);
            int count = collectors.Count();
            if (count > 0)
            {
                foreach (var pirate in collectors)
                {
                    Collect(game, pirate, left / count);
                    count--;
                }
            }
            if (left > 0)
            {
                game.Debug(left.ToString());
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
            var target = GetClosestReturn(game);
            if (target == null)
            {
                return;
            }
            chaser = GetNearestPirate(game, target);
            if (chaser == null)
            {
                return;
            }
            Goto(game, chaser, target.Location, left);
        }

        private static void Collect(IPirateGame game, Pirate pirate, int moves)
        {
            var treasure = GetNearestTreasure(game, pirate);
            if (treasure == null)
            {
                return;
            }
            treasures.Add(treasure);
            Goto(game, pirate, treasure.Location, moves);
        }

        private static void Goto(IPirateGame game, Pirate pirate, Location destination, int moves)
        {
            if (IsBusy(game, pirate))
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

        private static IEnumerable<Pirate> GetCollectors(IPirateGame game)
        {
            var collectors = game.MyPiratesWithoutTreasures();
            foreach (var pirate in collectors)
            {
                if (pirate != chaser && !IsBusy(game, pirate))
                {
                    yield return pirate;
                }
            }
        }

        private static Treasure GetNearestTreasure(IPirateGame game, Pirate pirate)
        {
            Treasure nearest = null;
            int minDist = 0;
            foreach (var treasure in game.Treasures())
            {
                if (treasures.IndexOf(treasure) < 0)
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

        private static Pirate GetClosestReturn(IPirateGame game)
        {
            var targets = game.EnemyPiratesWithTreasures();
            Pirate closest = null;
            int minDist = 0;
            foreach (var enemy in targets)
            {
                int dist = GetDistToReturn(game, enemy);
                if (closest == null || dist < minDist)
                {
                    closest = enemy;
                    minDist = dist;
                }
            }
            return closest;
        }

        private static bool IsBusy(IPirateGame game, Pirate pirate)
        {
            return pirate.TurnsToSober > 0 || TryDefend(game, pirate) || TryAttack(game, pirate);
        }

        private static int GetDistToReturn(IPirateGame game, Pirate pirate)
        {
            return game.Distance(pirate, pirate.InitialLocation);
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
            return space < 0 || space > pirate.AttackRadius * 2;
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
            if (pirate.DefenseExpirationTurns > 0 || pirate.DefenseReloadTurns > 0 || IsSafe(game, pirate))
            {
                return false;
            }
            game.Defend(pirate);
            return true;
        }
    }
}
