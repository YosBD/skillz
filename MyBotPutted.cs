using System.Collections.Generic;
using System;
using Pirates;

namespace MyBot
{
    #region Enum
    public enum EnemyAction
    {
        Nothing = 0,
        DontKnow = 1,
        WillSail = 2,
        WillAtack = 3,
        WillDefence = 4
    }
    public enum TypeOfAction
    {
        SailHome = 0,
        AttackOrcrash = 1,
        GetTreasure = 2,
        Attack = 3,
        Defence = 4,
        GetPowerUp = 5,
        RunAway = 6,
        SaveHim = 7,
        Crash = 8,
        HelpHim = 9
    }
    public enum PirateOrTreasure
    {
        Pirate = 0,
        Treasure = 1
    }

    #endregion

    public class MyBot : IPirateBot
    {
        public CollisionAvoider CA;
        private EnemyPredictor EP = new EnemyPredictor();
        public void DoTurn(IPirateGame game)
        {
            EP.AddTurn(game);
            CA = new CollisionAvoider(game);
            CA.init();

            //game.debug("Pirate 0 Realoding turns: {0}", game.EnemyPirates()[0].ReloadTurns);

            foreach (var e in game.AllEnemyPirates())
            {
                EnemyAction a = EP.GetEnemyNextAction(e);
                Location l = EP.GetEnemyNextLocation(e);
                //Location l1 = EP.GetEnemyNextDestention(e);
                //game.debug("Enemy number {0} going to do {1}", e.Id, a);
                //game.debug("Enemy next location: {0} - {1}", l.Row, l.Col);
            }


            List<Ship> ships = new List<Ship>();
            List<ActionsListForShip> allActions = new List<ActionsListForShip>();

            foreach (Pirate p in game.AllMyPirates())
            {
                Ship s = new Ship(game, p, EP);
                ships.Add(s);
                ActionsListForShip actionsForShip = new ActionsListForShip(game, p, EP);
                actionsForShip.getAllPossibleActions();
                allActions.Add(actionsForShip);

            }

            Strategy t1 = new Strategy(game, allActions);

            t1.StrategyA();


            List<Actions> toDo = t1.ToDo;

            int numberOfturnsToDo = game.GetActionsPerTurn();
            foreach (Actions a in toDo)
            {
                if (a as ActionDefence != null)
                {
                    //game.debug("Should turn defence on to  {0}", a.Player);
                }
                int actualMoves = a.DoIt(numberOfturnsToDo, CA);
                numberOfturnsToDo -= actualMoves;

            }
            //if (a as ActionSailHome != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        //game.debug("Moves == {0}", actualMoves);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //
            //if (a as ActionAttack != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        //game.debug("Moves == {0}", actualMoves);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //List<int> UsedPirates = new List<int>();
            //if (a as ActionGetTreasure != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        //game.debug("Moves == {0}", numberOfturnsToDo);
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //if (a as ActionDefence != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        //game.debug("Moves == {0}", numberOfturnsToDo);
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //if (a as ActionAttackOrcrash != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        //game.debug("Moves == {0}", numberOfturnsToDo);
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //if (a as ActionCrash != null)
            //{
            //    if (!UsedPirates.Contains(a.Player.Id))
            //    {
            //        //game.debug("4, Type == {0}", a.ActionType.ToString());
            //        //game.debug("Moves == {0}", numberOfturnsToDo);
            //        int actualMoves = a.DoIt(numberOfturnsToDo, CA);
            //        numberOfturnsToDo -= actualMoves;
            //        UsedPirates.Add(a.Player.Id);
            //    }
            //}
            //
            //
            //
            //
            //
            //
            //
            //}
        }
    }
    public class Ship
    {
        private Pirate Player;
        public Pirate PLAYER { get { return Player; } }

        private IPirateGame Game;
        private EnemyPredictor EP;

        public Ship(Pirate p)
        {
            Player = p;
        }

        public Ship(IPirateGame g, Pirate p, EnemyPredictor ep)
        {
            Game = g;
            Player = p;
            EP = ep;
        }
    }
    public class ActionsListForShip
    {
        private IPirateGame Game;
        private EnemyPredictor EP;
        public Pirate Player;

        private List<ActionDefence> DefenceActions;
        private List<ActionAttack> AttackActions;
        private List<ActionSailHome> SailHomeActions;
        private List<ActionGetTreasure> GetTreasureActions;
        private List<ActionAttackOrcrash> AttackOrcrashActions;
        private List<ActionCrash> CrashActions;
        private List<ActionGetPowerUp> GetPowerUpActions;
        private List<ActionRunAway> RunAwayActions;
        private List<ActionSaveHim> SaveHimActions;
        private List<ActionHelpHim> HelpHimActions;
        public ActionDefence bestDefence { get; private set; }
        public ActionAttack bestAttack { get; private set; }
        public ActionSailHome bestSailHome { get; private set; }
        public ActionGetTreasure bestGetTreasure { get; private set; }
        public ActionAttackOrcrash bestAttackOrcrash { get; private set; }
        public ActionCrash bestCrash { get; private set; }
        public ActionGetPowerUp bestGetPowerUp { get; private set; }
        public ActionRunAway bestRunAway { get; private set; }
        public ActionSaveHim bestSaveHim { get; private set; }
        public ActionHelpHim bestHelpHim { get; private set; }

        public ActionsListForShip(IPirateGame g, Pirate p, EnemyPredictor ep)
        {
            Game = g;
            EP = ep;
            Player = p;

            DefenceActions = new List<ActionDefence>();
            AttackActions = new List<ActionAttack>();
            SailHomeActions = new List<ActionSailHome>();
            GetTreasureActions = new List<ActionGetTreasure>();
            AttackOrcrashActions = new List<ActionAttackOrcrash>();
            CrashActions = new List<ActionCrash>();
            GetPowerUpActions = new List<ActionGetPowerUp>();
            RunAwayActions = new List<ActionRunAway>();
            SaveHimActions = new List<ActionSaveHim>();

            bestDefence = null;
            bestAttack = null;
            bestSailHome = null;
            bestGetTreasure = null;
            bestAttackOrcrash = null;
            bestCrash = null;
            bestGetPowerUp = null;
            bestRunAway = null;
            bestSaveHim = null;
        }
        public void getAllPossibleActions()
        {
            if (Player.TurnsToRevive > 0 || Player.TurnsToSober > 0)
                return;

            if (Player.DefenseReloadTurns == 0)
            {
                DefenceActions = getDefenceAction();
            }

            if (Player.HasTreasure)
            {
                SailHomeActions = getSailHomeAction();
            }
            else
            {
                if (Player.ReloadTurns == 0)
                {
                    AttackActions = getAttackAction();
                }

                GetTreasureActions = getTreasureAction();

                AttackOrcrashActions = getAttackOrCrashAction();

                SaveHimActions = getSaveHimAction();

                CrashActions = getCrashAction();
            }

            GetPowerUpActions = getPowerUpsAction();

            RunAwayActions = getRunAwayActions();
        }
        private List<ActionDefence> getDefenceAction()
        {
            List<ActionDefence> defenceActions = new List<ActionDefence>();
            foreach (Pirate e in Game.EnemyPiratesWithoutTreasures())
            {
                EnemyAction nextAction = EP.GetEnemyNextAction(e);
                if (nextAction == EnemyAction.WillAtack || nextAction == EnemyAction.DontKnow)
                {
                    if (Game.Distance(Player, e) <= e.AttackRadius)
                    {
                        defenceActions.Add(new ActionDefence(Game, Player, EP));
                        break;
                    }
                }
            }
            return defenceActions;
        }
        private List<ActionAttack> getAttackAction()
        {
            List<ActionAttack> AttackActions = new List<ActionAttack>();
            foreach (Pirate e in Game.EnemyPirates())
            {
                if (e.TurnsToSober > 0 || e.IsLost || e.DefenseExpirationTurns > 0)
                    continue;
                //if (EP.GetEnemyNextAction(e) != EnemyAction.WillDefence)
                {
                    AttackActions.Add(new ActionAttack(Game, Player, e, EP));
                }
            }
            return AttackActions;
        }
        private List<ActionSailHome> getSailHomeAction()
        {
            List<ActionSailHome> SailHomeAction = new List<ActionSailHome>();
            SailHomeAction.Add(new ActionSailHome(Game, Player, EP));
            return SailHomeAction;
        }
        private List<ActionGetTreasure> getTreasureAction()
        {
            List<ActionGetTreasure> TresureActions = new List<ActionGetTreasure>();
            foreach (Treasure t in Game.Treasures())
                TresureActions.Add(new ActionGetTreasure(Game, Player, EP, t));
            return TresureActions;
        }
        private List<ActionAttackOrcrash> getAttackOrCrashAction()
        {
            List<ActionAttackOrcrash> AttackOrCrashAction = new List<ActionAttackOrcrash>();

            foreach (Pirate e in Game.EnemyPirates())
            {
                if (e.TurnsToSober > 0 || e.IsLost)
                    continue;
                AttackOrCrashAction.Add(new ActionAttackOrcrash(Game, Player, e, EP));
            }
            return AttackOrCrashAction;
        }
        private List<ActionCrash> getCrashAction()
        {
            List<ActionCrash> Crash = new List<ActionCrash>();

            foreach (Pirate e in Game.EnemyPirates())
            {
                if (e.TurnsToSober > 0 || e.IsLost)
                    continue;

                Crash.Add(new ActionCrash(Game, Player, e, EP));
            }

            return Crash;
        }
        private List<ActionGetPowerUp> getPowerUpsAction()
        {
            List<ActionGetPowerUp> PowerUpsAction = new List<ActionGetPowerUp>();
            foreach (Powerup u in Game.Powerups())
            {
                PowerUpsAction.Add(new ActionGetPowerUp(Game, Player, EP, u));
            }
            return PowerUpsAction;
        }
        private List<ActionRunAway> getRunAwayActions()
        {
            List<ActionRunAway> RunAwayAction = new List<ActionRunAway>();
            RunAwayAction.Add(new ActionRunAway(Game, Player, EP));

            return RunAwayAction;
        }
        private List<ActionSaveHim> getSaveHimAction()
        {
            List<ActionSaveHim> SaveHimAction = new List<ActionSaveHim>();

            foreach (Pirate mdp in Game.MyDrunkPirates())
            {
                SaveHimAction.Add(new ActionSaveHim(Game, Player, EP, mdp));
            }
            return SaveHimAction;
        }
        private List<ActionHelpHim> getHelpHimAction()
        {
            List<ActionHelpHim> list = new List<ActionHelpHim>();

            foreach (Pirate th in Game.MyPiratesWithTreasures())
            {
                foreach (Pirate helper in Game.MyPiratesWithoutTreasures())
                {
                    if (helper.TurnsToSober == 0 && helper.TurnsToRevive == 0)
                    {
                        list.Add(new ActionHelpHim(Game, helper, EP, th));
                    }
                }
            }
            return list;
        }
        public void tA_findBestActionForEveryType()
        {
            tA_findBestDefenceAction();
            // tA_findBestAttackAction();
            tA_findBestSailHomeAction();
            tA_findBestGetTreasureAction();
            tA_findBestAttackOrcrashAction();
            //  tA_findBestCrashAction();
            tA_findBestGetPowerUpAction();
            tA_findBestRunAwayAction();
            tA_findBestSaveHimAction();
            tA_findBestHelpHimAction();
        }
        private void tA_findBestDefenceAction()
        {

            foreach (ActionDefence action in DefenceActions)
            {
                if (action.Player.HasTreasure && action.HasEnemyNextToHim() && !(action.Player.DefenseExpirationTurns > 0))
                    bestDefence = action;


            }
        }

        /*attacks priority:
            1. with treasure, in range (closest to home and has most valueable treasure)
            2. without treasure, in range (minimal attack reload turns)
            3. with treasure, not in range ()
            4. without treasure, not in range (closest to you)
        in every priority, enemies with power ups win.*/
        private void tA_findBestAttackAction()
        {
            List<ActionAttack> WithTreasureInRange = new List<ActionAttack>();
            List<ActionAttack> WithTreasureNotInRange = new List<ActionAttack>();
            List<ActionAttack> WithoutTreasureInRange = new List<ActionAttack>();
            List<ActionAttack> WithoutTreasureNotInRange = new List<ActionAttack>();

            foreach (ActionAttack AttackToCheck in AttackActions)
            {
                Pirate enemy = AttackToCheck.Enemy;
                Location nextEnemyLocation = EP.GetEnemyNextLocation(enemy);
                if (Game.InRange(Player, nextEnemyLocation))
                {
                    if (enemy.HasTreasure)
                    {
                        WithTreasureInRange.Add(AttackToCheck);
                    }
                    else
                    {
                        WithoutTreasureInRange.Add(AttackToCheck);
                    }
                }
                else
                {
                    if (enemy.HasTreasure)
                    {
                        WithTreasureNotInRange.Add(AttackToCheck);
                    }
                    else
                    {
                        WithoutTreasureNotInRange.Add(AttackToCheck);
                    }
                }
            }
            // get closest to me.
            int bestDist = 1000;
            foreach (ActionAttack AttackToCheck in WithoutTreasureNotInRange)
            {
                Pirate enemy = AttackToCheck.Enemy;
                if (enemy.Powerups.Count > 0)
                {
                    bestAttack = AttackToCheck;
                    break;
                }
                int dist = Game.Distance(Player, enemy);
                if (dist < bestDist)
                {
                    bestAttack = AttackToCheck;
                    bestDist = dist;
                }
            }
            // get most valueable treasure if can reach.
            double bestScore = 1000;
            foreach (ActionAttack AttackToCheck in WithTreasureNotInRange)
            {
                Pirate enemy = AttackToCheck.Enemy;
                if (enemy.Powerups.Count > 0)
                {
                    bestAttack = AttackToCheck;
                    break;
                }
                int distToPlayer = Game.Distance(Player, enemy);
                int distToHome = Game.Distance(enemy, enemy.InitialLocation);
                int turnsToReach = distToPlayer / (Game.GetActionsPerTurn() / 2);
                int turnsToHome = distToHome / enemy.CarryTreasureSpeed;
                if (turnsToReach < turnsToHome)
                {
                    int value = enemy.TreasureValue;
                    double score = (double)(turnsToHome - turnsToReach) / value;
                    if (score < bestScore)
                    {
                        bestAttack = AttackToCheck;
                        bestScore = score;
                    }
                }
            }
            // gets minimal attack reload turns.
            int bestReloadTurns = 1000;
            foreach (ActionAttack AttackToCheck in WithoutTreasureInRange)
            {
                Pirate enemy = AttackToCheck.Enemy;
                if (enemy.Powerups.Count > 0)
                {
                    bestAttack = AttackToCheck;
                    break;
                }
                int reloadTurns = enemy.ReloadTurns;
                if (reloadTurns < bestReloadTurns)
                {
                    bestAttack = AttackToCheck;
                    bestReloadTurns = reloadTurns;
                }
            }
            bestScore = 1000;
            foreach (ActionAttack AttackToCheck in WithTreasureInRange)
            {
                Pirate enemy = AttackToCheck.Enemy;
                if (enemy.Powerups.Count > 0)
                {
                    bestAttack = AttackToCheck;
                    break;
                }
                int distToHome = Game.Distance(enemy, enemy.InitialLocation);
                int value = enemy.TreasureValue;
                double score = (double)distToHome / value;
                if (score < bestScore)
                {
                    bestAttack = AttackToCheck;
                    bestScore = score;
                }
            }
        }

        private void tA_findBestSailHomeAction()
        {
            double bestScore = 1000;
            foreach (ActionSailHome action in SailHomeActions)
            {
                double score = action.Score();
                if (score < bestScore && score != -1)
                {
                    bestSailHome = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestGetTreasureAction()
        {
            double bestScore = 1000;
            foreach (ActionGetTreasure action in GetTreasureActions)
            {
                double score = action.Score();
                if (score < bestScore && !Game.IsOccupied(action.TREASURE.Location))
                {
                    bestGetTreasure = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestAttackOrcrashAction()
        {

            double bestScore = 1000;
            foreach (ActionAttackOrcrash action in AttackOrcrashActions)
            {
                double score = action.Score();


                if (score < bestScore && score != -1)
                {

                    bestAttackOrcrash = action;
                    bestScore = score;
                }
            }

        }
        private void tA_findBestCrashAction()
        {
            double bestScore = 1000;
            foreach (ActionCrash action in CrashActions)
            {
                double score = action.Score();
                if (score < bestScore)
                {
                    bestCrash = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestGetPowerUpAction()
        {
            double bestScore = 1000;
            foreach (ActionGetPowerUp action in GetPowerUpActions)
            {
                double score = action.Score();
                if (score < bestScore && score != -1)
                {
                    bestGetPowerUp = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestRunAwayAction()
        {
            double bestScore = 1000;
            foreach (ActionRunAway action in RunAwayActions)
            {
                double score = action.Score();
                if (score < bestScore && score != -1)
                {
                    bestRunAway = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestSaveHimAction()
        {
            double bestScore = 1000;
            foreach (ActionSaveHim action in SaveHimActions)
            {
                double score = action.Score();
                if (score < bestScore && score != -1)
                {
                    bestSaveHim = action;
                    bestScore = score;
                }
            }
        }
        private void tA_findBestHelpHimAction()
        {
            double bestScore = 1000;
            foreach (ActionHelpHim action in getHelpHimAction())
            {
                double score = action.Score();
                if (score < bestScore && score != -1)
                {
                    bestHelpHim = action;
                    bestScore = score;
                }
            }
        }
    }
    public class Strategy
    {
        private IPirateGame Game;
        private List<ActionsListForShip> AllPossibleAction = new List<ActionsListForShip>();
        private List<Actions> Strategy1 = new List<Actions>();
        public List<Actions> ToDo = new List<Actions>();

        public Strategy(IPirateGame g, List<ActionsListForShip> APA)
        {
            Game = g;
            AllPossibleAction = APA;
        }

        public void StrategyA()
        {


            # region General Instructions
            /*
             ######Steps######
             * 1) Save him option 
             * 2) PowerUps - Enemy crah 
             * 3) Calculate How many moves will take to - collect powerup , or collect tresure and come back 
             * 4) if tresure is better - Calculate  how many moves will take (if enemy has tresure) kill enemy, or go and get tresure 
             * 5) Turn Conclution 
             * 6) Do the turn, with enemy predictor and etc.
                     
             ###### Notice and special condictions ######
             * A) Enemy about to win
             * B) We about to win
             * C) Game will be  over soon
             * D) Lost and drunk pirates enemy and mine
             * E) Colition avoider !
             */
            #endregion

            foreach (ActionsListForShip clfs in AllPossibleAction)
            {
                clfs.getAllPossibleActions();
                clfs.tA_findBestActionForEveryType();
            }

            if (Game.MyPiratesWithTreasures().Count > 0)
            {
                AllPossibleAction = GetDefence(AllPossibleAction);
                AllPossibleAction = SailHome(AllPossibleAction);
             
            }
            if (Game.EnemyPiratesWithTreasures().Count > 0)
            {
                //  AllPossibleAction = GetAttackingStratgey(AllPossibleAction);
                AllPossibleAction = GetAttackOrCrash(AllPossibleAction); // Need To Change!

                // AllPossibleAction = GetCrash(AllPossibleAction); // Need To Changes

            }
            if (Game.MyPiratesWithTreasures().Count > 0) 
            {
                 AllPossibleAction = GetHelpHim(AllPossibleAction);
            }
          
            if (Game.Powerups().Count > 0)
            {

                AllPossibleAction = GetPowerUps(AllPossibleAction);

            }
            if (Game.Treasures().Count > 0)
            {
                AllPossibleAction = GetTresureStratgey(AllPossibleAction);
            }



            //List<Actions> GetTreasures = T1_AddGetTreasuresAction(AllPossibleAction);
            ////     GetTreasures = EveryPirateOne(GetTreasures);
            //
            //List<Actions> GetDefence = T1_AddDefence(AllPossibleAction);
            //
            //List<Actions> SailHome = T1_SailHome(AllPossibleAction);
            //
            //List<Actions> Attack = T1_AddAttack(AllPossibleAction);


            /*
            List<Actions> AttackOrcrash = T1_AddAttackOrcrash(AllPossibleAction);
            Attack = EveryPirateOne(Attack);
            List<Actions> GetPowerUp = T1_GetPowerUpsAction(AllPossibleAction);
            GetTreasures = EveryPirateOne(GetTreasures);
            List<Actions> BestPossibleActions = new List<Actions>();
            */


        }

        public List<ActionsListForShip> GetTresureStratgey(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            ActionGetTreasure agt = null;
            ActionsListForShip curShip = null;
            foreach (ActionsListForShip forship in Actions)
            {

                ActionGetTreasure Current = forship.bestGetTreasure;

                if (Current != null)
                {

                    if (agt == null)
                    {

                        agt = Current;
                        curShip = forship;
                    }
                    if (Current.Score() < agt.Score())
                    {

                        agt = Current;
                        curShip = forship;
                    }
                    //game.debug("Pirate {0} want to go to tresure {1}", Current.Player.Id, Current.TREASURE.Id);
                }
            }


            if (agt != null)
            {
                //game.debug("Desided -  Pirate {0} will to go to tresure {1}", agt.Player.Id, agt.TREASURE.Id);
                ToDo.Add(agt);
                restShips.Remove(curShip);
            }
            return restShips;
        }
        public List<ActionsListForShip> SailHome(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            if (Actions != null)
            {

                foreach (ActionsListForShip forship in Actions)
                {
                    ActionSailHome Current = forship.bestSailHome;
                    if (Current != null)
                    {
                        ////game.debug("Pirate {0} Want to go home ", Current.Player);                   
                        ToDo.Add(Current);

                    }
                }
            }
            return restShips;
        }
        public List<ActionsListForShip> GetAttackingStratgey(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            if (Actions != null)
            {


                ActionAttack agt = null;
                ActionsListForShip curShip = null;
                foreach (ActionsListForShip forship in Actions)
                {
                    ActionAttack Current = forship.bestAttack;
                    if (Current != null)
                    {
                        if (agt == null)
                        {
                            agt = Current;
                            curShip = forship;
                        }
                        if (Current.Score() < agt.Score())
                        {
                            agt = Current;
                            curShip = forship;
                        }
                    }
                }

                if (agt != null)
                {
                    //game.debug("Pirate {0} want to attack or crash  {1}", agt.Player.Id, agt.Enemy.Id);

                    ////game.debug("Desided -  Pirate {0} will to go to tresure {1}", agt.Player.Id, agt.TREASURE.Id);
                    ToDo.Add(agt);
                    restShips.Remove(curShip);
                }
            }
            return restShips;
        }
        public List<ActionsListForShip> GetDefence(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = new List<ActionsListForShip>(Actions);

            ActionDefence curShip = null;
            if (restShips != null)
            {

                foreach (ActionsListForShip forship in Actions)
                {
                    ActionDefence current = forship.bestDefence;
                    if (current != null)
                    {
                        curShip = current;
                        ToDo.Add(curShip);
                        restShips.Remove(forship);
                    }
                }
            }
            return restShips;
        }
        public void StrategyDefensive()
        {

        }
        public List<ActionsListForShip> GetAttackOrCrash(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            if (Actions != null)
            {
                ActionAttackOrcrash agt = null;
                ActionsListForShip curShip = null;

                foreach (ActionsListForShip forship in Actions)
                {


                    ActionAttackOrcrash Current = forship.bestAttackOrcrash;

                    if (agt == null && Current != null)
                    {

                        agt = Current;
                        curShip = forship;
                    }
                    if (Current != null && agt != null)
                    {
                        if (Current.Score() < agt.Score())
                        {

                            agt = Current;
                            curShip = forship;
                        }
                    }
                }
                if (agt != null)
                {

                    ToDo.Add(agt);
                    restShips.Remove(curShip);
                }
            }
            return restShips;
        }
        public List<ActionsListForShip> GetCrash(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            ActionsListForShip curShip = null;
            ActionCrash agt = null;
            foreach (ActionsListForShip forship in Actions)
            {

                ActionCrash Current = forship.bestCrash;
                if (agt != null)
                {
                    agt = Current;
                    curShip = forship;
                }
                if (Current != null)
                {
                    if (Current.Score() < agt.Score())
                    {
                        agt = Current;
                        curShip = forship;
                    }
                }
            }
            if (agt != null)
            {
                ToDo.Add(agt);
                restShips.Remove(curShip);
            }
            return restShips;
        }
        public List<ActionsListForShip> GetPowerUps(List<ActionsListForShip> Actions)
        {
            List<ActionsListForShip> restShips = Actions;
            ActionGetPowerUp agt = null;
            ActionsListForShip curShip = null;
            foreach (ActionsListForShip forship in Actions)
            {

                ActionGetPowerUp Current = forship.bestGetPowerUp;

                if (Current != null)
                {
                    if (agt == null)
                    {

                        agt = Current;
                        curShip = forship;
                    }
                    if (Current.Score() < agt.Score())
                    {
                        //game.debug("4");
                        agt = Current;
                        curShip = forship;
                    }
                    //game.debug("Pirate {0} want to go to Powerup", Current.Player.Id);
                }
            }


            if (agt != null)
            {
                //game.debug("Desided -  Pirate {0} will to go to Powerup ", agt.Player.Id);
                ToDo.Add(agt);
                restShips.Remove(curShip);
            }
            return restShips;
        }
        public List<ActionsListForShip> GetHelpHim(List<ActionsListForShip> Actions)
        {

            List<ActionsListForShip> restShips = Actions;
            if (Actions != null)
            {
                ActionHelpHim agt = null;
                ActionsListForShip curShip = null;

                foreach (ActionsListForShip forship in Actions)
                {


                    ActionHelpHim Current = forship.bestHelpHim;

                    if (agt == null && Current != null)
                    {

                        agt = Current;
                        curShip = forship;
                    }
                    if (Current != null && agt != null)
                    {
                        if (Current.Score() < agt.Score())
                        {

                            agt = Current;
                            curShip = forship;
                        }
                    }
                }
                if (agt != null)
                {

                    ToDo.Add(agt);
                    restShips.Remove(curShip);
                }
            }
            return restShips;
        }

        /*private List<Actions> T1_GetPowerUpsAction(List<Actions> AllPossibleAction)
        {
          
        }*/

        private List<Actions> T1_AddGetTreasuresAction(List<Actions> AllPossibleAction)
        {
            List<Actions> T = new List<Actions>();
            if (AllPossibleAction.Count == 0)
                return T;
            foreach (Actions a in AllPossibleAction)
            {
                if (a.ActionType == TypeOfAction.GetTreasure)
                    T.Add(a);
            }
            double BestDist = -1000;
            List<Actions> bestActions = new List<Actions>();
            Actions curBestAction = T[0];
            int prevId = -1;
            foreach (Actions a in T)
            {
                if (a.Player.Id != prevId)
                {
                    if (BestDist > -1)
                        bestActions.Add(curBestAction);
                    BestDist = -1000;
                }
                ActionGetTreasure curAc = (ActionGetTreasure)a;
                double ScoreDist = curAc.Score();
                if (ScoreDist > BestDist || (ScoreDist == BestDist && new Random().Next(0, 2) == 0))
                {
                    curBestAction = a;
                    BestDist = ScoreDist;
                }
                prevId = a.Player.Id;
            }

            return bestActions;
        }

        private List<Actions> T1_AddGetPowerUp(List<Actions> AllPossibleAction)
        {
            List<Actions> PU = new List<Actions>();
            foreach (Actions a in AllPossibleAction)
            {
                if (a.ActionType == TypeOfAction.GetPowerUp)
                    PU.Add(a);
            }
            int minDist = 1000;
            List<Actions> bestActions = new List<Actions>();
            foreach (Actions a in PU)
            {
                ActionGetPowerUp curAc = (ActionGetPowerUp)a;
                int curDist = curAc.HowManySteps();
                if (curDist < minDist)
                {
                    bestActions.Add(a);
                    minDist = curDist;
                }
            }

            return bestActions;
        }

        private List<Actions> T1_SailHome(List<Actions> AllPossibleAction)
        {
            List<Actions> SH = new List<Actions>();
            foreach (Actions a in AllPossibleAction)
            {
                if (a.ActionType == TypeOfAction.SailHome)
                {
                    SH.Add(a);
                }
            }
            return SH;
        }

        private List<Actions> T1_AddAttack(List<Actions> AllPossibleAction)
        {
            List<Actions> allAttacks = new List<Actions>();
            foreach (Actions a in AllPossibleAction)
            {
                if (a.ActionType == TypeOfAction.Attack)
                {
                    allAttacks.Add(a);
                }
            }
            List<Actions> AT2 = new List<Actions>();
            List<Actions> AT3 = new List<Actions>();
            List<Actions> AT4 = new List<Actions>();
            List<Actions> BestWayToAttack = new List<Actions>();
            List<Actions> BestWayToAttackChecker = new List<Actions>();
            List<Actions> BestWayToAttackChecker2 = new List<Actions>();
            int min1 = 1000;
            int min2 = 1000;
            int min3 = 1000;
            foreach (Actions a in allAttacks)
            {
                ActionAttack CurA = (ActionAttack)a;
                if (CurA.Enemy.HasTreasure)
                {
                    if (CurA.attack() == -1)
                    {
                        BestWayToAttack.Add(a);
                    }
                    if (Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation) <= min1)
                    {
                        if (Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation) < min1)
                            AT2.Clear();
                        AT2.Add(a);
                        min1 = Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation);
                    }
                    if (CurA.attack() <= min2)
                    {
                        if (CurA.attack() < min2)
                            AT3.Clear();
                        AT3.Add(a);
                        min2 = CurA.attack();
                    }
                }
                else
                {
                    if (CurA.attack() <= min3)
                    {
                        if (CurA.attack() < min3)
                            AT4.Clear();
                        AT4.Add(a);
                        min3 = CurA.attack();
                    }
                }
            }
            foreach (Actions a in AT2)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            foreach (Actions a in AT3)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            foreach (Actions a in AT4)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            return BestWayToAttack;
        }

        private List<Actions> T1_AddAttackOrcrash(List<Actions> AllPossibleAction)
        {
            List<Actions> AT1 = new List<Actions>();
            List<Actions> AT2 = new List<Actions>();
            List<Actions> AT3 = new List<Actions>();
            List<Actions> AT4 = new List<Actions>();
            List<Actions> BestWayToAttack = new List<Actions>();
            List<Actions> BestWayToAttackChecker = new List<Actions>();
            List<Actions> BestWayToAttackChecker2 = new List<Actions>();
            foreach (Actions a in AllPossibleAction)
            {
                if (a.ActionType == TypeOfAction.AttackOrcrash)
                {
                    AT1.Add(a);
                }
            }
            int min1 = 1000;
            int min2 = 1000;
            int min3 = 1000;
            foreach (Actions a in AT1)
            {
                ActionAttackOrcrash CurA = (ActionAttackOrcrash)a;
                if (CurA.Enemy.HasTreasure)
                {
                    if (CurA.attack() == -1)
                    {
                        BestWayToAttack.Add(a);
                    }
                    if (Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation) <= min1)
                    {
                        if (Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation) < min1)
                            AT2.Clear();
                        AT2.Add(a);
                        min1 = Game.Distance(CurA.Enemy, CurA.Enemy.InitialLocation);
                    }
                    if (CurA.attack() <= min2)
                    {
                        if (CurA.attack() < min2)
                            AT3.Clear();
                        AT3.Add(a);
                        min2 = CurA.attack();
                    }
                }
                else
                {
                    if (CurA.attack() <= min3)
                    {
                        if (CurA.attack() < min3)
                            AT4.Clear();
                        AT4.Add(a);
                        min3 = CurA.attack();
                    }
                }
            }
            foreach (Actions a in AT2)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            foreach (Actions a in AT3)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            foreach (Actions a in AT4)
            {
                bool Check = true;
                foreach (Actions b in BestWayToAttack)
                {
                    if (b.Player == a.Player)
                    {
                        Check = false;
                    }
                }
                if (Check)
                {
                    BestWayToAttack.Add(a);
                }
            }
            return BestWayToAttack;
        }

        private void removeAllActionsOfShip(Pirate p)
        {
            //foreach (Actions a in AllPossibleAction)
            //{
            //    if (a.Player == p)
            //        AllPossibleAction.Remove(a);
            //}
        }

        private List<Actions> EveryPirateOne(List<Actions> LA)
        {
            List<Actions> ActionPirateOne = new List<Actions>();
            while (LA.Count != 0)
            {
                ActionPirateOne.Add(LA[0]);
                foreach (Actions a in ActionPirateOne)
                {
                    LA = removeAllActionsOfShip(a.Player, LA);
                }
            }
            return ActionPirateOne;
        }

        private List<Actions> removeAllActionsOfShip(Pirate p, List<Actions> A)
        {
            foreach (Actions a in A)
            {
                if (a.Player == p)
                    A.Remove(a);
            }
            return A;
        }

    }

    #region Moves
    public class EnemyPredictor
    {
        class GameState
        {
            private IPirateGame game;
            public IPirateGame Game { get { return this.game; } set { this.game = value; } }

            private List<Ship> myShips;
            public List<Ship> MyShips { get { return this.myShips; } set { this.myShips = value; } }

            private List<Ship> enemyShips;
            public List<Ship> EnemyShips { get { return this.enemyShips; } set { this.enemyShips = value; } }

            private List<Treasure> allTreasures;
            public List<Treasure> AllTreasures { get { return this.allTreasures; } set { this.allTreasures = value; } }

            private List<Powerup> allPowerUps;
            public List<Powerup> AllPowerups { get { return this.allPowerUps; } set { this.allPowerUps = value; } }

            public GameState(IPirateGame g)
            {
                Game = g;
                myShips = new List<Ship>();
                enemyShips = new List<Ship>();
                allTreasures = new List<Treasure>();
                allPowerUps = new List<Powerup>();

                foreach (Pirate P in g.AllMyPirates())
                {
                    Pirate p = new Pirate(P.Id, P.Owner, P.Location, P.InitialLocation, P.AttackRadius);
                    p.CarryTreasureSpeed = P.CarryTreasureSpeed;
                    p.DefenseExpirationTurns = P.DefenseExpirationTurns;
                    p.DefenseReloadTurns = P.DefenseReloadTurns;
                    p.HasTreasure = P.HasTreasure;
                    p.IsLost = P.IsLost;
                    p.Powerups = P.Powerups;
                    p.ReloadTurns = P.ReloadTurns;
                    p.TreasureValue = P.TreasureValue;
                    p.TurnsToRevive = P.TurnsToRevive;
                    p.TurnsToSober = P.TurnsToSober;
                    Ship s = new Ship(p);
                    MyShips.Add(s);
                }

                foreach (Pirate E in g.AllEnemyPirates())
                {
                    Pirate e = new Pirate(E.Id, E.Owner, E.Location, E.InitialLocation, E.AttackRadius);
                    e.CarryTreasureSpeed = E.CarryTreasureSpeed;
                    e.DefenseExpirationTurns = E.DefenseExpirationTurns;
                    e.DefenseReloadTurns = E.DefenseReloadTurns;
                    e.HasTreasure = E.HasTreasure;
                    e.IsLost = E.IsLost;
                    e.Powerups = E.Powerups;
                    e.ReloadTurns = E.ReloadTurns;
                    e.TreasureValue = E.TreasureValue;
                    e.TurnsToRevive = E.TurnsToRevive;
                    e.TurnsToSober = E.TurnsToSober;
                    Ship s = new Ship(e);
                    EnemyShips.Add(s);
                }

                foreach (Treasure T in g.Treasures())
                {
                    Treasure t = new Treasure(T.Id, T.Location, T.Value);
                    AllTreasures.Add(t);
                }

                foreach (Powerup PU in g.Powerups())
                {
                    Powerup pu = new Powerup(PU.Id, PU.Type, PU.Location, PU.ActiveTurns, PU.EndTurn);
                    AllPowerups.Add(pu);
                }
            }

        }
        private List<GameState> AllTurns;
        private List<Treasure> AllTreasureInGame = new List<Treasure>();
        private IPirateGame Game;

        public EnemyPredictor()
        {
            AllTurns = new List<GameState>();
        }
        public void AddTurn(IPirateGame g)
        {
            Game = g;
            GameState state = new GameState(g);
            AllTurns.Add(state);
            if (AllTreasureInGame.Count == 0)
                AllTreasureInGame.AddRange(Game.Treasures());
        }
        public Location GetEnemyNextLocation(Pirate e)
        {
            // calculate what this enemy did and what do you think he will do next
            // FOR NOW return his current location or his best move if he has a treasure.
            if (e.HasTreasure)
            {
                List<Location> EnemyWay = Game.GetSailOptions(e, e.InitialLocation, e.CarryTreasureSpeed);
                if (EnemyWay.Count == 0)
                    return e.Location;
                else
                    return EnemyWay[0];
            }
            else
            {
                if (GetEnemyNextAction(e) == EnemyAction.WillSail)
                {
                    return WhatDoYouThinkEnemyWillDo(e) ?? e.Location;
                }
                return e.Location;
            }
        }
        public EnemyAction GetEnemyNextAction(Pirate e)
        {
            // FOR NOW the enemy allways sail, later check earlier turns and try to understand what is going on.
            bool check = false;
            EnemyAction ac = EnemyAction.DontKnow;
            if (e.IsLost || e.TurnsToSober > 0 && !check)
            {
                check = true;
                ac = EnemyAction.Nothing;
            }
            if (IsGoingToDefence(e) && !check)
            {
                check = true;
                ac = EnemyAction.WillDefence;
            }
            if (IsGoingToAttack(e) && !check)
            {
                check = true;
                ac = EnemyAction.WillAtack;
            }
            if (!check && AllTurns.Count > 1)
            {
                foreach (Ship e1 in AllTurns[AllTurns.Count - 2].EnemyShips)
                {
                    if (e.Id == e1.PLAYER.Id)
                    {
                        if (!e1.PLAYER.Location.Equals(e.Location))
                        {
                            ac = EnemyAction.WillSail;
                        }
                    }
                }
            }
            return ac;
        }
        private bool IsGoingToAttack(Pirate e)
        {
            if (e.ReloadTurns == 0 && !e.IsLost && e.TurnsToSober == 0 && !e.HasTreasure)
            {
                foreach (Pirate p in Game.MyPiratesWithTreasures())
                {
                    if (Game.InRange(e, p))
                        return true;
                }
            }
            return false;
        }
        private bool IsGoingToDefence(Pirate e)
        {
            foreach (Pirate p in Game.MyPiratesWithoutTreasures())
            {
                if (p.ReloadTurns == 0 && Game.InRange(p, e) && e.DefenseReloadTurns > 0 && e.DefenseExpirationTurns > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private Location WhatDoYouThinkEnemyWillDo(Pirate e)
        {
            int count = AllTurns.Count;
            if (count < 3)
            {
                return e.Location;
            }

            List<Dictionary<Tuple<int, PirateOrTreasure>, int>> EveryThing = new List<Dictionary<Tuple<int, PirateOrTreasure>, int>>();

            for (int i = 0; i < 3; i++)
            {
                EveryThing.Add(new Dictionary<Tuple<int, PirateOrTreasure>, int>());
                GameState gs = AllTurns[count - i - 1];
                Pirate Enemy = Game.AllEnemyPirates()[0];
                foreach (var e1 in gs.EnemyShips)
                {
                    if (e1.PLAYER.Id == e.Id)
                        Enemy = e1.PLAYER;
                }
                foreach (Ship p in gs.MyShips)
                {
                    int Distance = -1;
                    if (!p.PLAYER.IsLost)
                    {
                        Distance = Game.Distance(Enemy, p.PLAYER.Location);
                        //     ////game.debug("Enemy Location: {0} - {1}",Enemy.Location.Row, Enemy.Location.Col);
                        //     ////game.debug("My pirate {0} Location: {1} - {2}",p.PLAYER.Id, p.PLAYER.Location.Row, p.PLAYER.Location.Col);
                        //     ////game.debug("Distance: {0}", Distance);                       
                    }
                    //////game.debug("Ship ID: {0}    distance to Enemy: {1} is {2}", p.PLAYER.Id, e.Id, Distance);
                    EveryThing[i].Add(Tuple.Create(p.PLAYER.Id, PirateOrTreasure.Pirate), Distance);
                }
                foreach (Treasure t in AllTreasureInGame)
                {
                    int Distance = -1;
                    if (IsOccured(gs, t))
                    {
                        Distance = Game.Distance(Enemy, t.Location);
                        //   ////game.debug("Enemy Location: {0} - {1}", Enemy.Location.Row, Enemy.Location.Col);
                        //   ////game.debug("Treasure ID {0} Location: {1} - {2}", t.Id, t.Location.Row, t.Location.Col);
                        //   ////game.debug("Distance: {0}", Distance);
                    }
                    //////game.debug("Treasure ID: {0}    distance to Enemy: {1} is {2}", t.Id, e.Id, Distance);
                    EveryThing[i].Add(Tuple.Create(t.Id, PirateOrTreasure.Treasure), Distance);
                }
            }
            //    foreach (var thing in EveryThing)
            //    {
            //        foreach (var item in thing)
            //        {
            //            ////game.debug("ID: {0}, TYPE: {1}, DIST: {2}", item.Key.Item1, item.Key.Item2, item.Value);
            //        }
            //    }
            //    Game.StopPoint("Pls work");


            Tuple<Tuple<int, PirateOrTreasure>, int> delta = Delta(EveryThing);
            Tuple<int, PirateOrTreasure> key = delta.Item1;
            //////game.debug("Delta === ID: {0} , Type: {1} , Distance: {2}",delta.Item1.Item1, delta.Item1.Item2, delta.Item2);
            int change = delta.Item2;
            if (key == null)
            {
                ////game.debug("Key == Null");
                return null;
            }
            else
            {
                Location destenation = GetDestenation(key);
                if (destenation == null)
                {
                    ////game.debug("Destention == Null");
                    return null;
                }
                else
                {
                    Location next = GetNextMove(e, destenation);
                    ////game.debug("Next location {0} - {1}", next.Row, next.Col);
                    return next;
                }
            }
        }
        private bool IsOccured(GameState gs, Treasure Treasure)
        {
            return gs.AllTreasures.Contains(Treasure);
        }
        private Tuple<Tuple<int, PirateOrTreasure>, int> Delta(List<Dictionary<Tuple<int, PirateOrTreasure>, int>> EveryThing)
        {
            Tuple<int, PirateOrTreasure> keyOfP = Tuple.Create(0, PirateOrTreasure.Pirate);
            int maxChange = 0;
            int minDist = 1000;
            foreach (var t in EveryThing[0])
            {
                //   //game.debug("Object : {0} - {1}", t.Key.Item1, t.Key.Item2);
                for (int i = 0; i < 3; i++)
                {
                    //  //game.debug("distance : {0}", EveryThing[i][t.Key]);
                }

                bool Add = true;
                int last = t.Value;
                for (int i = 1; i < 3; i++)
                {
                    if (last != -1 && EveryThing[i].ContainsKey(t.Key))
                    {
                        int dist = EveryThing[i][t.Key];
                        ////game.debug("Last: {0} ,Distance: {1}",last, dist);
                        if (dist < last)
                        {
                            ////game.debug("Break!! And Add == False");
                            Add = false;
                            break;
                        }
                        last = dist;
                    }
                    else
                    {
                        Add = false;
                        break;
                    }
                }
                if (Add)
                {
                    int change = last - t.Value;
                    // //game.debug("Change == {0} {1}", change, t.Value);
                    if (change >= maxChange && t.Value < minDist)
                    {
                        minDist = t.Value;
                        maxChange = change;
                        keyOfP = t.Key;
                    }
                }
            }
            return Tuple.Create(keyOfP, maxChange);
        }
        private Location GetDestenation(Tuple<int, PirateOrTreasure> key)
        {
            int id = key.Item1;
            PirateOrTreasure Type = key.Item2;
            if (Type == PirateOrTreasure.Pirate)
            {
                Pirate p = Game.GetMyPirate(id);
                //game.debug("Destination: {0} - {1}", p.Location.Row, p.Location.Col);
                return p.Location;
            }
            else
            {
                foreach (var T in Game.Treasures())
                {
                    if (T.Id == id)
                    {
                        //game.debug("Destination: {0} - {1}", T.Location.Row, T.Location.Col);
                        return T.Location;
                    }
                }
            }
            return null;
        }
        private Location GetNextMove(Pirate e, Location destenation)
        {
            int PossibleMoves = Game.GetActionsPerTurn();
            foreach (var en in Game.EnemyPiratesWithTreasures())
            {
                PossibleMoves -= en.CarryTreasureSpeed;
            }
            List<Location> PossibleLocation = Game.GetSailOptions(e, destenation, PossibleMoves);
            if (PossibleLocation.Count == 0)
                return e.Location;
            return PossibleLocation[0];
        }

        private void HowEnemyAttacks()
        {
            GameState TurnBeforeHeAttacked = AllTurns[AllTurns.Count - 2];
            List<Ship> EnemyAbleToShoot = new List<Ship>();
            List<Ship> MyAvailble = AllTurns[AllTurns.Count - 1].MyShips;
            List<Ship> PiratesShooted = new List<Ship>();
            List<Ship> EnemyAbleToShoot1 = WhoCanShootInTurn(TurnBeforeHeAttacked);
            List<Ship> EnemyAbleToShoot2 = WhoCanShootInTurn(AllTurns[AllTurns.Count - 1]);
            Dictionary<Ship, Ship> Paires = new Dictionary<Ship, Ship>();


            if (EnemyAbleToShoot1.Count > 0)
            {
                PiratesShooted = WhoShooted(EnemyAbleToShoot1, EnemyAbleToShoot2);
            }

            if (PiratesShooted.Count > 0)
            {
                Paires = WhoGotShot(PiratesShooted, MyAvailble);
                if (Paires.Count > 0)
                {
                    for (int i = 0; i < Paires.Count; i++)
                    {
                        if (Paires[PiratesShooted[i]].PLAYER.DefenseReloadTurns > 0)
                        {

                        }
                    }
                }
            }

        }
        private List<Ship> WhoShooted(List<Ship> P1, List<Ship> P2)
        {
            List<Ship> Shoots = new List<Ship>();
            foreach (Ship e in P1)
            {
                if (P2.Contains(e))
                {
                    continue;
                }
                Shoots.Add(e);
            }
            return Shoots;
        }
        private List<Ship> WhoCanShootInTurn(GameState TurnBeforeHeAttacked)
        {
            List<Ship> CanShoot = new List<Ship>();
            foreach (Ship e in TurnBeforeHeAttacked.EnemyShips)
            {
                if (e.PLAYER.TurnsToSober == 0 && e.PLAYER.ReloadTurns == 0 && !e.PLAYER.HasTreasure)
                    CanShoot.Add(e);
            }
            return CanShoot;
        }
        private Dictionary<Ship, Ship> WhoGotShot(List<Ship> PiratesShooted, List<Ship> MyPirates)
        {
            Dictionary<Ship, Ship> GotShot = new Dictionary<Ship, Ship>();
            foreach (Ship e in PiratesShooted)
            {
                foreach (Ship p in MyPirates)
                {
                    if (Game.InRange(e.PLAYER, p.PLAYER))
                    {
                        if (p.PLAYER.TurnsToSober == Game.GetSoberTurns() || p.PLAYER.DefenseExpirationTurns == Game.GetDefenseExpirationTurns()) //Check if -1
                        {
                            GotShot.Add(e, p);
                            continue;
                        }
                    }
                }
            }
            return GotShot;
        }

    }
    public class CollisionAvoider
    {
        private List<Location> CaptureLocation = new List<Location>();

        private IPirateGame Game;
        private bool WithTreasures = false;
        private bool MyDrunkPirates = true;
        private bool Enemy = true;

        public CollisionAvoider(IPirateGame g)
        {
            Game = g;
        }

        public void init(bool WT, bool MDP, bool E)
        {
            WithTreasures = WT;
            MyDrunkPirates = MDP;
            Enemy = E;
            init();
        }

        public void init()
        {
            Reset();
            InitAvoider();
        }

        public void Reset()
        {
            this.CaptureLocation.Clear();
        }

        public void InitAvoider()
        {
            if (MyDrunkPirates && Game.MyDrunkPirates() != null)
                foreach (Pirate p in Game.MyDrunkPirates()) // ספינות שלי שיכורות
                {
                    CaptureLocation.Add(p.Location);
                }
            if (Enemy)
            {
                if (Game.EnemyPiratesWithTreasures() != null)
                    foreach (Pirate e in Game.EnemyPiratesWithTreasures()) // ספינות אוייב עם אוצר
                    {
                        List<Location> ETW = Game.GetSailOptions(e, e.InitialLocation, e.CarryTreasureSpeed);
                        foreach (Location l in ETW)
                        {
                            CaptureLocation.Add(l);
                        }
                    }
                if (Game.EnemyPirates() != null)
                {
                    foreach (Pirate e in Game.EnemyPirates())
                    {
                        CaptureLocation.Add(e.Location);
                    }
                }
            }
            if (Game.MyPirates() != null)
            {
                foreach (Pirate p in Game.MyPirates()) // ספינות שלי 
                {
                    if (p.TurnsToSober == 0)
                    {
                        CaptureLocation.Add(p.Location);
                    }
                }
            }

            if (WithTreasures)
            {
                foreach (Treasure T in Game.Treasures())
                {
                    CaptureLocation.Add(T.Location);
                }
            }
        }

        public bool CheckLocation(Location l)
        {
            if (CaptureLocation.Contains(l))
                return false;
            return true;
        }

        public Location TryAdd(List<Location> PossibleLocations)
        {
            //game.debug("*******");
            List<Location> Possible = new List<Location>();
            Random rnd = new Random();
            ////game.debug("Possile Location = {0}, {1}-{2}", PossibleLocations.Count,PossibleLocations[0].Row,PossibleLocations[0].Col);
            foreach (Location l in PossibleLocations)
            {
                if (!HasLocation(l))
                {
                    CaptureLocation.Add(l);
                    Possible.Add(l);

                }
                else
                {

                }
            }

            if (Possible.Count == 0)
            {
                //////game.debug("Did not Found Solution");
                return null;
            }
            int num = rnd.Next(0, Possible.Count);
            return Possible[num];
        }

        private bool HasLocation(Location l)
        {
            foreach (Location r in CaptureLocation)
            {
                if (r.Row == l.Row && r.Col == l.Col)
                    return true;
            }
            return false;
        }
    }
    #endregion

    #region Actions
    public abstract class Actions
    {
        protected IPirateGame Game;
        public Pirate Player { get; private set; }
        protected EnemyPredictor EP;
        protected CollisionAvoider CA;

        public Actions(IPirateGame g, Pirate p, EnemyPredictor ep)
        {
            Game = g;
            Player = p;
            EP = ep;
            CA = new CollisionAvoider(Game);
        }

        public abstract TypeOfAction ActionType { get; }

        public abstract double Score();

        public abstract int DoIt(int PM, CollisionAvoider CA);
    }
    public class ActionDefence : Actions
    {
        public ActionDefence(IPirateGame g, Pirate p, EnemyPredictor ep) : base(g, p, ep) { }

        public override TypeOfAction ActionType { get { return TypeOfAction.Defence; } }

        //no priority.
        public override double Score()
        {
            bool found = false;
            foreach (Pirate E in Game.EnemyPiratesWithoutTreasures())
            {
                if (E.TurnsToSober == 0 && !E.IsLost && E.ReloadTurns == 0)
                {
                    if (Game.InRange(E, Player))
                    {
                        if (Player.HasTreasure)
                        {
                            return 2;
                        }
                        else
                        {
                            return 1;
                        }

                    }
                }
            }
            return 0;
        }

        public bool HasEnemyNextToHim()
        {

            foreach (Pirate E in Game.EnemyPiratesWithoutTreasures())
            {
                if (E.TurnsToSober == 0 && !E.IsLost && E.ReloadTurns == 0)
                {
                    if (Game.InRange(E, Player))
                    {
                        return true;

                    }
                }
            }
            return false;
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            Game.Defend(Player);
            return 0;
        }
    }
    public class ActionAttack : Actions
    {
        public Pirate Enemy { get; private set; }
        private int PossibleMoves;
        private int NeededMoves;

        public ActionAttack(IPirateGame g, Pirate p, Pirate e, EnemyPredictor ep)
            : base(g, p, ep)
        {
            Enemy = e;
        }
        public int attack()
        {
            if (Game.InRange(Player, EP.GetEnemyNextLocation(Enemy)))
            {
                NeededMoves = -1;
            }
            else
            {
                NeededMoves = Game.Distance(Player, Enemy);
            }
            return NeededMoves;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.Attack; } }

        public override double Score()
        {
            double a = Game.Distance(Player.Location, Enemy.Location) / (Game.GetActionsPerTurn() - Game.MyPiratesWithTreasures().Count);
            double b = Game.Distance(Enemy.Location, Enemy.InitialLocation);
            double c = Enemy.TreasureValue;

            if (c > 0 && Player.TurnsToSober == 0 && Player.TurnsToRevive == 0)
            {
                return (a + b) / c;
            }
            else
            {
                return -1;
            }
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            PossibleMoves = PM;
            if (attack() < 0)
            {
                Game.Attack(Player, Enemy);
                return 0;
            }
            else
            {
                if (PM == 0)
                    return 0;
                Location l = CA.TryAdd(Game.GetSailOptions(Player, Enemy, PossibleMoves));
                Game.SetSail(Player, l);
            }
            return PossibleMoves;
        }
    }
    public class ActionSailHome : Actions
    {
        private int PossibleMoves;
        private int NeededMoves;

        public ActionSailHome(IPirateGame g, Pirate p, EnemyPredictor ep)
            : base(g, p, ep)
        {
            HowManySteps();
        }

        public int HowManySteps()
        {
            if (!(Player.TurnsToSober == 0 && Player.TurnsToRevive == 0))
            {
                return -1;
            }

            if (Game.Distance(Player, Player.InitialLocation) < Player.CarryTreasureSpeed)
            {
                NeededMoves = Game.Distance(Player, Player.InitialLocation);
            }
            else
            {
                NeededMoves = Player.CarryTreasureSpeed;
            }
            return NeededMoves;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.SailHome; } }

        //closest to me.
        public override double Score()
        {
            return HowManySteps();
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;

            PossibleMoves = PM;
            if (PossibleMoves >= NeededMoves)
            {

                Location l = CA.TryAdd(Game.GetSailOptions(Player, Player.InitialLocation, NeededMoves));
                if (l != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", l.Row, l.Col);
                    Game.SetSail(Player, l);
                    return NeededMoves;
                }
                Location NewHomw1 = new Location(Player.InitialLocation.Row - 1, Player.InitialLocation.Col - 1);
                Location l1 = CA.TryAdd(Game.GetSailOptions(Player, NewHomw1, NeededMoves));
                if (l1 != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", NewHomw1.Row, NewHomw1.Col);
                    Game.SetSail(Player, l1);
                    return NeededMoves;
                }
                Location NewHomw2 = new Location(Player.InitialLocation.Row + 1, Player.InitialLocation.Col + 1);
                Location l2 = CA.TryAdd(Game.GetSailOptions(Player, NewHomw2, NeededMoves));
                if (l2 != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", NewHomw2.Row, NewHomw2.Col);
                    Game.SetSail(Player, l2);
                    return NeededMoves;
                }
                else
                {
                    return 0;
                }
            }
            else
            {

                Location l = CA.TryAdd(Game.GetSailOptions(Player, Player.InitialLocation, PossibleMoves));
                if (l != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", l.Row, l.Col);
                    Game.SetSail(Player, l);
                    return PossibleMoves;
                }
                Location NewHomw1 = new Location(Player.InitialLocation.Row - 1, Player.InitialLocation.Col - 1);
                Location l1 = CA.TryAdd(Game.GetSailOptions(Player, NewHomw1, PossibleMoves));
                if (l1 != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", NewHomw1.Row, NewHomw1.Col);
                    Game.SetSail(Player, l1);
                    return PossibleMoves;
                }
                Location NewHomw2 = new Location(Player.InitialLocation.Row + 1, Player.InitialLocation.Col + 1);
                Location l2 = CA.TryAdd(Game.GetSailOptions(Player, NewHomw2, PossibleMoves));
                if (l2 != null)
                {
                    //game.debug("Pirate {0} Says", Player.Id);
                    //game.debug("My Location {0} - {1} ", Player.Location.Row, Player.Location.Col);
                    //game.debug("Sail To {0} - {1} ", NewHomw2.Row, NewHomw2.Col);
                    Game.SetSail(Player, l2);
                    return PossibleMoves;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class ActionGetTreasure : Actions
    {
        private int PossibleMoves = 0;
        private int NeededMoves;
        private Treasure SelectedTreasure;

        public Treasure TREASURE { get { return SelectedTreasure; } }

        public ActionGetTreasure(IPirateGame g, Pirate p, EnemyPredictor ep, Treasure T)
            : base(g, p, ep)
        {
            SelectedTreasure = T;
            HowManySteps();
        }

        public int HowManySteps()
        {
            NeededMoves = Game.Distance(Player, SelectedTreasure);
            return NeededMoves;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.GetTreasure; } }

        public override double Score()
        {
            if (!(Player.TurnsToSober == 0 && Player.TurnsToRevive == 0))
            {
                return -1;
            }
            double a = Game.Distance(Player.Location, SelectedTreasure.Location) / (Game.GetActionsPerTurn() - Game.MyPiratesWithTreasures().Count);
            double b = Game.Distance(Player.InitialLocation, SelectedTreasure.Location);
            double c = SelectedTreasure.Value;
            double d = 0;
            double f = 0;
            if (Player.Powerups.Contains("Speed"))
                d = 5;
            if (Player.Powerups.Contains("Attack"))
                f = 5;

            //Value , Tresure Distance From House , Tresure Distase From Player
            double Score = (a + b + f) / (c + d);
            ////game.debug("Pirate {0} with Tresure {1} has {2} Score ", Player.Id, SelectedTreasure.Id, Score);
            return Score;
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;


            ////game.debug("1.1");
            PossibleMoves = PM;
            if (PossibleMoves >= NeededMoves)
            {
                Location l = CA.TryAdd(Game.GetSailOptions(Player, SelectedTreasure, NeededMoves));
                if (l != null)
                {
                    Game.SetSail(Player, l);
                    ////game.debug("1.2");
                    return NeededMoves;

                }
                else
                {
                    ////game.debug("1.3");
                    return 0;
                }
            }
            else
            {

                Location l = CA.TryAdd(Game.GetSailOptions(Player, SelectedTreasure, PossibleMoves));
                if (l != null)
                {
                    Game.SetSail(Player, l);
                    ////game.debug("1.4");
                    return PossibleMoves;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class ActionAttackOrcrash : Actions
    {
        public Pirate Enemy { get; private set; }
        private int PossibleMoves;
        private int NeededMoves;
        private bool CrashNow = true;
        private bool ShootNow = true;

        public ActionAttackOrcrash(IPirateGame g, Pirate p, Pirate e, EnemyPredictor ep)
            : base(g, p, ep)
        {
            Enemy = e;
        }

        public int attack()
        {
            if (Enemy.DefenseExpirationTurns > 0 || Player.ReloadTurns > 0 || Enemy.Powerups.Count > 0)
            {
                ShootNow = false;
                return crash();
            }
            else
            {
                CrashNow = false;
                return shoot();
            }
        }

        public int crash()
        {
            Location EnemyLocation = EP.GetEnemyNextLocation(Enemy);
            int dist = Game.Distance(Player, EnemyLocation);
            if (dist > Game.GetActionsPerTurn())
            {
                CrashNow = false;
            }
            else
            {
                CrashNow = true;
            }
            NeededMoves = dist;
            return NeededMoves;
        }

        public int shoot()
        {
            Location EnemyLocation = EP.GetEnemyNextLocation(Enemy);
            if (!Game.InRange(Player, EnemyLocation))
            {
                ShootNow = false;
                return crash();
            }
            return -1;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.AttackOrcrash; } }

        //

        public override double Score()
        {
            double a = Game.Distance(Player.Location, Enemy.Location) / (Game.GetActionsPerTurn() - Game.MyPiratesWithTreasures().Count);
            double b = Game.Distance(Enemy.Location, Enemy.InitialLocation);
            double c = Enemy.TreasureValue;
            double d = 0;

            if (Player.Powerups.Contains("Attack"))
                d = 10;

            if (c > 0 && Player.TurnsToSober == 0 && !Player.IsLost && Enemy.TurnsToSober == 0 && Enemy.TurnsToRevive == 0)
            {

                return (a + b) / (c + d);
            }

            else
            {
                return -1;
            }
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            attack();
            PossibleMoves = PM;
            if (ShootNow)
            {
                Game.Attack(Player, Enemy);
                return PossibleMoves;
            }
            if (PM == 0)
                return 0;

            if (PossibleMoves >= NeededMoves && CrashNow)
            {
                Game.SetSail(Player, EP.GetEnemyNextLocation(Enemy));
                return NeededMoves;
            }
            else
            {

                Location l = CA.TryAdd(Game.GetSailOptions(Player, Enemy, PossibleMoves));
                if (l != null)
                {
                    Game.SetSail(Player, l);
                    return PossibleMoves;
                }
                return 0;
            }
        }
    }
    public class ActionCrash : Actions
    {
        public Pirate Enemy { get; private set; }
        private int PossibleMoves;
        private int NeededMoves;
        private bool CrashNow = true;

        public ActionCrash(IPirateGame g, Pirate p, Pirate e, EnemyPredictor ep)
            : base(g, p, ep)
        {
            Enemy = e;
        }

        public int Attack()
        {
            Location EnemyLocation = EP.GetEnemyNextLocation(Enemy);
            int dist = Game.Distance(Player, EnemyLocation);
            if (dist > Game.GetActionsPerTurn())
            {
                CrashNow = false;
            }
            NeededMoves = dist;
            return NeededMoves;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.Crash; } }

        //
        public override double Score()
        {
            if (!(Player.TurnsToSober == 0 && Player.TurnsToRevive == 0))
            {
                return -1;
            }
            int distToPlayer = Game.Distance(Player, Enemy);
            int turnsToReach = distToPlayer / (Game.GetActionsPerTurn() / 2);
            if (Enemy.HasTreasure)
            {
                int distToHome = Game.Distance(Enemy, Enemy.InitialLocation);
                int turnsToHome = distToHome / Enemy.CarryTreasureSpeed;
                if (turnsToReach < turnsToHome)
                {
                    int value = Enemy.TreasureValue;
                    return (double)(turnsToHome - turnsToReach) / value;
                }
                return 1000;
            }
            else
            {
                return distToPlayer;
            }
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;
            PossibleMoves = PM;
            if (PossibleMoves >= NeededMoves && CrashNow)
            {
                Game.SetSail(Player, EP.GetEnemyNextLocation(Enemy));
                return NeededMoves;
            }
            else
            {
                Location l = CA.TryAdd(Game.GetSailOptions(Player, Enemy, PossibleMoves));
                Game.SetSail(Player, l);
                return PossibleMoves;
            }
        }
    }
    public class ActionGetPowerUp : Actions
    {
        private int PossibleMoves = 0;
        private int NeededMoves;
        private Powerup SelectedPowerUp;

        public ActionGetPowerUp(IPirateGame g, Pirate p, EnemyPredictor ep, Powerup pu)
            : base(g, p, ep)
        {
            SelectedPowerUp = pu;
        }

        public int HowManySteps()
        {
            if (Player.TurnsToSober == 0 && Player.TurnsToRevive == 0)
            {
                NeededMoves = Game.Distance(Player, SelectedPowerUp.Location);
                return NeededMoves;
            }
            return -1;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.GetPowerUp; } }

        //closest to me.
        public override double Score()
        {
            return HowManySteps();
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;
            PossibleMoves = PM;

            if (PossibleMoves >= NeededMoves)
            {
                Location l = CA.TryAdd(Game.GetSailOptions(Player, SelectedPowerUp.Location, NeededMoves));
                Game.SetSail(Player, l);
                return NeededMoves;
            }
            else
            {
                CA.init(true, true, true);
                Location l = CA.TryAdd(Game.GetSailOptions(Player, SelectedPowerUp.Location, PossibleMoves));
                if (l != null)
                {
                    Game.SetSail(Player, l);
                    return PossibleMoves;
                }
                return 0;
            }
        }
    }
    public class ActionRunAway : Actions
    {
        private List<Pirate> EnemiesInRange = new List<Pirate>();
        private List<Location> PossibleLocations = new List<Location>();
        private Location NewLocation;
        private int NeededMoves;
        private int PossibleMoves;

        public ActionRunAway(IPirateGame g, Pirate p, EnemyPredictor ep) : base(g, p, ep) { }

        public List<Location> AmIInRange()
        {
            foreach (Pirate e in Game.EnemyPiratesWithoutTreasures())
            {
                if (Game.InRange(e, Player) && e.TurnsToSober == 0)
                    EnemiesInRange.Add(e);
            }
            return FindBestLocation();
        }

        private List<Location> FindBestLocation()
        {
            if (EnemiesInRange.Count > 0)
            {
                for (int i = -(Game.GetActionsPerTurn()); i <= Game.GetActionsPerTurn(); i++)
                {
                    for (int j = -(Game.GetActionsPerTurn()); j <= Game.GetActionsPerTurn(); j++)
                    {
                        Location l = new Location(Player.Location.Row + i, Player.Location.Col + j);
                        bool Add = true;
                        foreach (Pirate e in EnemiesInRange)
                        {
                            if (Game.InRange(e, l))
                            {
                                Add = false;
                                break;
                            }
                        }
                        if (Add && CA.CheckLocation(l))
                        {
                            PossibleLocations.Add(l);
                        }
                    }
                }
            }
            return PossibleLocations;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.RunAway; } }

        //if has treasure, closest to home. else, closest to treasure.
        public override double Score()
        {
            if (!(Player.TurnsToSober == 0 && Player.TurnsToRevive == 0))
            {
                return -1;
            }


            List<Location> l = AmIInRange();
            if (l.Count > 0)
            {
                if (Player.HasTreasure)
                {
                    int dist = Game.Distance(l[0], Player.InitialLocation);
                    return dist;
                }
                else
                {
                    int bestDist = 1000;
                    foreach (Treasure t in Game.Treasures())
                    {
                        int dist = Game.Distance(l[0], t.Location);
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                        }
                    }
                    return bestDist;
                }
            }
            else
            {
                return 1000;
            }
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;
            PossibleMoves = PM;
            if (PossibleMoves >= NeededMoves)
            {

                Location l = CA.TryAdd(PossibleLocations);
                Game.SetSail(Player, l);
                return NeededMoves;
            }
            return 0;
        }
    }
    public class ActionSaveHim : Actions
    {
        private Pirate MyDrunkPirate;
        private int PossibleMoves = 0;
        private int NeededMoves;

        public ActionSaveHim(IPirateGame g, Pirate p, EnemyPredictor ep, Pirate mdp)
            : base(g, p, ep)
        {
            MyDrunkPirate = mdp;
        }

        public int HowManySteps()
        {
            NeededMoves = Game.Distance(Player, MyDrunkPirate);
            return NeededMoves;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.SaveHim; } }

        //closest to me.
        public override double Score()
        {
            return HowManySteps();
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            if (PM == 0)
                return 0;
            PossibleMoves = PM;
            if (PossibleMoves >= NeededMoves)
            {
                Game.SetSail(Player, MyDrunkPirate.Location);
                return NeededMoves;
            }
            else
            {

                Location l = CA.TryAdd(Game.GetSailOptions(Player, MyDrunkPirate, PossibleMoves));
                Game.SetSail(Player, l);
                return PossibleMoves;
            }
        }
    }
    public class ActionHelpHim : Actions
    {
        private Pirate PlayerToHelp;

        private Pirate EnemyToAttack;

        public ActionHelpHim(IPirateGame g, Pirate p, EnemyPredictor ep, Pirate pth)
            : base(g, p, ep)
        {
            PlayerToHelp = pth;
        }

        public override TypeOfAction ActionType { get { return TypeOfAction.HelpHim; } }

        private void WhichEnemyToAttack()
        {
            List<Pirate> Attackers = new List<Pirate>();
            int minDist = 1000;
            foreach (Pirate e in Game.EnemyPiratesWithoutTreasures())
            {
                if (e.TurnsToSober == 0)
                {
                    int dist = Game.Distance(PlayerToHelp, e);
                    if (dist <= minDist)
                    {
                        if (dist < minDist)
                            Attackers.Clear();
                        Attackers.Add(e);
                        minDist = dist;
                    }
                }
            }
            if (Attackers.Count > 0)
            {
                foreach (Pirate e in Attackers)
                {
                    if (e.Powerups.Contains("Attack"))
                    {
                        EnemyToAttack = e;
                        return;
                    }
                }
                foreach (Pirate e in Attackers)
                {
                    if (e.ReloadTurns == 0)
                    {
                        EnemyToAttack = e;
                        return;
                    }
                }
                Random rnd = new Random();
                EnemyToAttack = Attackers[rnd.Next(Attackers.Count)];
            }
            else
            {
                EnemyToAttack = Game.AllEnemyPirates()[0];
            }
        }

   
        private int CanReach()
        {
            WhichEnemyToAttack();
            int a = Game.Distance(Player, PlayerToHelp);
            int b = Game.Distance(EnemyToAttack, PlayerToHelp);
            if (b < a)
            {
                return -1;
            }
            int c = Game.GetActionsPerTurn();
            foreach (Pirate e in Game.EnemyPiratesWithTreasures())
            {
                c -= e.CarryTreasureSpeed;
            }
            double howManyTurnsEnemy = b / c;
            //game.debug("howManyTurnsEnemy: {0}", howManyTurnsEnemy);
            int d = Game.GetActionsPerTurn();
            int neededmoves = 0;
            //game.debug("3");
            for (int i = 0; i < d; i++)
            {
                //game.debug("1");
                if (a / (d - i) < howManyTurnsEnemy)
                {
                    //game.debug("2");
                    neededmoves = d - i;
                    //game.debug("neededmoves: {0}", neededmoves);

                }
            }
            //game.debug("Done!");
            return neededmoves;
        }

        public override double Score()
        {
            int steps = CanReach();
            if (steps <= 0)
            {
                return steps;
            }
            double a = Game.Distance(Player, PlayerToHelp) / (steps);
            double b = Game.Distance(PlayerToHelp, PlayerToHelp.InitialLocation) / PlayerToHelp.CarryTreasureSpeed;
            double c = PlayerToHelp.TreasureValue;

            return (a + b) / c;
        }

        public override int DoIt(int PM, CollisionAvoider CA)
        {
            ActionAttackOrcrash AAOC = new ActionAttackOrcrash(Game, Player, EnemyToAttack, EP);
            if (CanReach() <= 0)
            {
                if (CanReach() <= PM)
                {
                    return AAOC.DoIt(CanReach(), CA);
                }
                else
                {
                    return AAOC.DoIt(PM, CA);
                }
            }
            return 0;
        }
    }
    #endregion
}
