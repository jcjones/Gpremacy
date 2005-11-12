using System;
using System.Collections;

namespace Gpremacy {
class Orig_Play3Attack : State {
	Territory previousTerritory;
	Territory currentTerritory;
	bool arrowOn;
	bool selectingTargets;
	CombatView combatView;
	
	public Orig_Play3Attack (Game game_i) : base(game_i,3,4)
	{
	}
	
	public override string Name()
	{
		return "Phase 3: Attack";
	}
	
	public override void beginPlayer(Player p) {
		previousTerritory = currentTerritory = null;
		arrowOn = selectingTargets = false;		
	}
	
	public void strategicAttack()
	{
		selectingTargets = true;
		Game game = Game.GetInstance();
		
		if (!game.LocalPlayers.Contains(game.State.CurrentPlayer))
			return;
		
		combatView = game.GUI.CombatView;
		
		combatView.Attacker = game.State.CurrentPlayer;
		combatView.showStrategicTargetSelection();
	}
	public void doneSelectingStrategicTargets()
	{
		selectingTargets = false;
	}
	
	public override bool mouseClick(Territory target, uint Button)
	{
		Player player = Game.GetInstance().State.CurrentPlayer;
		
		if (selectingTargets)
		{
			combatView.AddStrategicTarget(target);
			return true;
		}
		
		/* Conventional Attack Logic */
		if (Button != 1) 
		{
			/* Cancelling */
			arrowOn = false;
			
			if (previousTerritory != null)
				Game.GUI.redrawTerritory(previousTerritory);

			Game.GUI.redrawTerritory(target);			
			previousTerritory = null;			
			return false;
		}
		
		if (previousTerritory == null)
		{
			if ( target.occupiedBy(player) && ( 
				 (!target.IsLand) || (target.Owner == player ) ) )
			{
				previousTerritory = target;
				arrowOn = true;
				return true;
			}
			return false;
		}
		
		currentTerritory = target;

//		Game.GetInstance().GUI.showConventionalBattle(currentTerritory, previousTerritory, player, currentTerritory.Owner);
		
		/* Confirm that they're adjacent */
		if (Game.GUI.Map.distanceBetween(previousTerritory, currentTerritory) > 1)
			return false;
			
		/* Confirm that they're of the same type */
		if (previousTerritory.IsLand != currentTerritory.IsLand)
			return false;
		
		/* Confirm that it's occupied */
		if (currentTerritory.Units.Count < 1)
			return false;
		
		Player defender = currentTerritory.Owner;
		if (defender == Game.GetInstance().PlayerNobody)
		{
			/* Resort to alternate measures to determine ownership */
			defender = ((Unit)(currentTerritory.Units[0])).Owner;
			System.Console.WriteLine("Alternate selection of defender: Chose " + defender.Name);
		}
		
		
		Orig_AttackConventionalStart cmd = new Orig_AttackConventionalStart(player, defender, currentTerritory, previousTerritory);
		Game.GetInstance().State.Execute(cmd);
		
		previousTerritory = currentTerritory = null;
		arrowOn = false;

		return true;
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		if (arrowOn && previousTerritory != null && curTerr != null && curTerr != currentTerritory)
		{
			currentTerritory = curTerr;
			System.Console.WriteLine("Arrow from " + previousTerritory.Name + " to " + curTerr.Name);
			Game.GUI.drawArrow(previousTerritory, curTerr);
			return true;
		}
		return false;
	}
}

[Serializable]
class NuclearTarget
{
	public string tname;
	public int icbms;
	
	public NuclearTarget(Territory t, int i)
	{ tname = t.Name; icbms = i; }
	
	public Territory territory {
		get { return Game.GetInstance().TerritoryByName(tname); }
	}
	
}

[Serializable]
class Orig_AttackStrategicStart : Command
{
	public Player attacker;
	public ArrayList defenders; // of Player
	public ArrayList nuclearTargetList; // of NuclearTarget
	
	public Orig_AttackStrategicStart(Player a, ArrayList b, ArrayList c)
	{
		attacker = a; defenders = b; nuclearTargetList = c;		
	}
	
	public override void Execute(Game game)
	{
		attacker = game.GetLocalCopyOfPlayer(attacker);
		
		game.GUI.CombatView.Attacker = attacker;
		Nuke nuke = new Nuke(attacker);
		ArrayList nukes = attacker.getActiveUnitsOfType(nuke);
		
		/* Delete fired nukes */
		int j = 0;
		foreach(NuclearTarget target in nuclearTargetList)
		{
			for (int i=0; i<target.icbms; i++)
			{
				attacker.ActiveUnits.Remove(nukes[j++]);
			}
		}
		
		/* Open battle */
		foreach(Player player in defenders)
		{
			Player curPlay = game.GetLocalCopyOfPlayer(player);

			if (game.LocalPlayers.Contains(curPlay))
			{
				System.Console.WriteLine("Opening strategic battle window for " + curPlay.Name);
				game.GUI.CombatView.showStrategicBattle(nuclearTargetList, curPlay);
				break;
			} else {
				if (!curPlay.Active)
				{
					/*TODO: ?*/
				}
			}
		}
		
		if (defenders.Count < 1)
		{
			// There are no defenders for some reason, just nuke'em all.
			foreach(NuclearTarget target in nuclearTargetList)
			{
				Orig_AttackStrategicDetonate cmd = new Orig_AttackStrategicDetonate(target.territory);
				game.State.Execute(cmd);
			}
		}		
	}	
}

[Serializable]
class Orig_AttackStrategicDetonate : Command
{
	String tname;
	public Orig_AttackStrategicDetonate (Territory a)
	{
		tname = a.Name;
	}
	
	public override void Execute(Game game)
	{
		Territory target = game.TerritoryByName(tname);
		 
		target.Destroyed = true;
		ArrayList unitList = (ArrayList)target.Units.Clone();
		ArrayList playersInTarget = new ArrayList();
		
		/* Determine who's in that territory */
		foreach (Unit unit in unitList)
		{
			if (!playersInTarget.Contains(unit.Owner))
				playersInTarget.Add(unit.Owner);
		}
		
		foreach (Player player in playersInTarget)
		{
			Orig_AttackDeleteUnits cmd = new Orig_AttackDeleteUnits(unitList, player, target);
			game.State.Execute(cmd);
		}
		
		/* Disable any resources in that area */
		/*TODO: disable resources here and check Destroyed in RC interface */
		
		/* Show detonation */
		game.GUI.showNuclearDetonationAnimation(target);
	}
}

[Serializable]
class Orig_AttackConventionalStart : Command
{
	public Player attacker;
	public Player defender;
	public string curTerrName, prevTerrName;
	
	public Orig_AttackConventionalStart(Player a, Player d, Territory c, Territory p)
	{
		attacker = a;
		defender = d;
		curTerrName = c.Name;
		prevTerrName = p.Name;
	}
	
	public override void Execute(Game game)
	{
		attacker = game.GetLocalCopyOfPlayer(attacker);
		defender = game.GetLocalCopyOfPlayer(defender);
		Territory prevTerr = game.TerritoryByName(prevTerrName);
		Territory curTerr = game.TerritoryByName(curTerrName);
		
		if (game.LocalPlayers.Contains(attacker) || game.LocalPlayers.Contains(defender))
		{
			System.Console.WriteLine("Opening conventional battle window for battle between " + attacker.Name + " and " + defender.Name + " and " + prevTerr.Name + " and " + curTerr.Name);
			
			game.GUI.CombatView.Attacker = attacker;
			game.GUI.CombatView.Defender = defender;
			game.GUI.CombatView.BattleLocation = curTerr;
			game.GUI.CombatView.StagingLocation = prevTerr;			
			
			game.GUI.CombatView.showConventionalBattle();
		}
		
	}	
}

[Serializable]
class Orig_AttackConventionalDefenderReady : Command
{
	Player attacker;
	Player defender;
	bool fullDefense;

	public Orig_AttackConventionalDefenderReady(Player a, Player d, bool fd)
	{
		attacker = a;
		defender = d;
		fullDefense = fd;
	}
	
	public override void Execute(Game game)
	{
		attacker = game.GetLocalCopyOfPlayer(attacker);
		defender = game.GetLocalCopyOfPlayer(defender);
		
		if (game.LocalPlayers.Contains(attacker) || game.LocalPlayers.Contains(defender))
		{	
			System.Console.WriteLine("Defender is ready, [" + fullDefense + "]");
			game.GUI.CombatView.conventionalDefenderReady(fullDefense);			
		}
	}
	
}

[Serializable]
class Orig_AttackConventionalSupplies : Command
{
	Player attacker;
	Player defender;
	bool fullDefense;
	
	public Orig_AttackConventionalSupplies(Player a, Player d, bool fd)
	{
		attacker = a;
		defender = d;
		fullDefense = fd;
	}
	
	public override void Execute(Game game)
	{
		attacker = game.GetLocalCopyOfPlayer(attacker);
		defender = game.GetLocalCopyOfPlayer(defender);
		
		foreach (Resource r in Game.GetInstance().AllResources)
		{		
			attacker.changeResourceStockpile(r, -1);
			
			if (fullDefense)
				defender.changeResourceStockpile(r, -1);
		}	
	}
}

[Serializable]
class Orig_AttackConventionalRoll : Command
{
	Player attacker;
	Player defender;
	Territory battleLocation;
	int attackerDice;
	int defenderDice;
	int attackerRoll;
	int defenderRoll;
	
	public Orig_AttackConventionalRoll(Player a, Player d, Territory t, int ad, int dd, int ar, int dr)
	{
		attacker = a;
		defender = d;
		battleLocation = t;
		attackerDice = ad;
		defenderDice = dd;
		attackerRoll = ar;
		defenderRoll = dr;
	}
	
	public override void Execute(Game game)
	{
		if ( (game.LocalPlayers.Contains(attacker)) || (game.LocalPlayers.Contains(defender)))
		{
			game.GUI.CombatView.conventionalDiceResults(attackerDice, defenderDice, attackerRoll, defenderRoll);
		}
		
		Game.GetInstance().GUI.writeToLog("Battle for " +battleLocation.Name+ ": Attacker ("+attacker.Name+") kills " + (attackerRoll/3) + " and Defender ("+defender.Name+") kills " + (defenderRoll/3) + ".");

	}
	
}
	

[Serializable]
class Orig_AttackDeleteUnits : Command
{
	ArrayList units;
	Player curPlay;
	string tname;
	
	public Orig_AttackDeleteUnits(ArrayList us, Player pl, Territory te) 
	{
		units = us;
		tname = te.Name;
		curPlay = pl;
	}
	
	public override void Execute(Game game)
	{
		curPlay = game.GetLocalCopyOfPlayer(curPlay);
		Territory land = game.TerritoryByName(tname);

		System.Console.Write("DeleteUnits from " + curPlay.Name + " and " + land.Name + ": "); 
		foreach(Unit unit in units)
		{
			Unit bob = game.GetLocalCopyOfUnit(unit);
			if (land != null) {
				land.removeUnit(bob);   			
			}
			curPlay.ActiveUnits.Remove(bob);
			System.Console.Write(bob.Name + "[" + bob.ID + "] ");
			if (bob is TacticalUnit)
			{
				if (((TacticalUnit)bob).CanHoldTroops)
					((TacticalUnit)bob).DeleteUnitsAboard(-1);
			}
		}
		System.Console.WriteLine(".");
		if (land != null)
			game.GUI.redrawTerritory(land);
	}
	
	
}

}
