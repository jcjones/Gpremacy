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
		
		
		Game.GetInstance().GUI.showConventionalBattle(currentTerritory, previousTerritory, player, defender);
		previousTerritory = currentTerritory = null;
		arrowOn = false;

		//Game.GetInstance().GUI.showConventionalBattle(target, attacker, defender);
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

class NuclearTarget
{
	public NuclearTarget(Territory t, int i)
	{ territory = t; icbms = i; }
	public Territory territory;
	public int icbms;
}

class Orig_AttackStrategicStart : Command
{
	Player attacker;
	ArrayList defenders; // of Player
	ArrayList nuclearTargetList; // of NuclearTarget
	
	public Orig_AttackStrategicStart(Player a, ArrayList b, ArrayList c)
	{
		attacker = a; defenders = b; nuclearTargetList = c;		
	}
	
	public override void Execute(Game game)
	{
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
			if (Game.GetInstance().LocalPlayers.Contains(player))
			{
				System.Console.WriteLine("Opening strategic battle window for " + player.Name);
				game.GUI.CombatView.showStrategicBattle(nuclearTargetList, player);
				break;
			}		
	}	
}

class Orig_AttackStrategicDetonate : Command
{
	Territory target;
	public Orig_AttackStrategicDetonate (Territory a)
	{
		target = a;
	}
	
	public override void Execute(Game game)
	{
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
		
		/* Show detonation */
		game.GUI.showNuclearDetonationAnimation(target);
	}
}

class Orig_AttackConventionalStart : Command
{
	Player attacker;
	Player defender;
	bool fullDefense;
	
	public Orig_AttackConventionalStart(Player a, Player d, bool fd)
	{
		attacker = a;
		defender = d;
		fullDefense = fd;
	}
	
	public override void Execute(Game game)
	{
		foreach (Resource r in Game.GetInstance().AllResources)
		{		
			attacker.changeResourceStockpile(r, -1);
			
			if (fullDefense)
				defender.changeResourceStockpile(r, -1);
		}		
	}	
}

class Orig_AttackDeleteUnits : Command
{
	ArrayList units;
	Player curPlay;
	Territory land;
	
	public Orig_AttackDeleteUnits(ArrayList us, Player pl, Territory te) 
	{
		units = us;
		land = te;
		curPlay = pl;
	}
	
	public override void Execute(Game game)
	{
		System.Console.Write("DeleteUnits from " + curPlay.Name + " and " + land.Name + ": "); 
		foreach(Unit unit in units)
		{
			if (land != null) {
				land.removeUnit(unit);   			
			}
			curPlay.ActiveUnits.Remove(unit);
			System.Console.Write(unit.Name + " ");
			if (unit is TacticalUnit)
			{
				if (((TacticalUnit)unit).CanHoldTroops)
					((TacticalUnit)unit).DeleteUnitsAboard(-1);
			}
		}
		System.Console.WriteLine(".");
		if (land != null)
			game.GUI.redrawTerritory(land);
	}
	
	
}

}
