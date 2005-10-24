using System.Collections;

namespace Gpremacy {
class Orig_Play3Attack : State {
	Territory previousTerritory;
	Territory currentTerritory;
	bool arrowOn;
	
	public Orig_Play3Attack (Game game_i) : base(game_i,3,4)
	{
	}
	
	public override string Name()
	{
		return "Phase 3: Attack";
	}
	
	public override void beginPlayer(Player p) {
		previousTerritory = currentTerritory = null;
		arrowOn = false;		
	}
	
	public override bool mouseClick(Territory target, uint Button)
	{
		Player player = Game.GetInstance().State.CurrentPlayer;
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
