using System.Collections;

namespace Gpremacy {
class Orig_Play5Build : State {
	ArrayList unitsToBuild;
	
	public Orig_Play5Build (Game game_i) : base(game_i,5,6)
	{
		unitsToBuild = new ArrayList();
	}
	
	public override string Name()
	{
		return "Phase 5: Build Forces and Weapons";
	}
		
	public override bool mouseClick(Territory target, uint Button)
	{
		Unit nu;
		Player me = Game.GetInstance().State.CurrentPlayer;
		
		if (target.MapTerritory.isLand)
			nu = new Army(me, target);
		else
			nu = new Navy(me, target);
		
		/* Can't use Contains since hash values are different ... search stupidly */
		foreach(Unit u in unitsToBuild)
		{
			if (u.Name == nu.Name)
			{
				unitsToBuild.Remove(u);				
				Game.GetInstance().GUI.writeToLog("Building " + nu.Name);
				Orig_BuildUnit cmd = new Orig_BuildUnit(nu, target, me);
				Game.GetInstance().State.Execute(cmd);
				break; // must break, because another iteration will fault
			}			
		}
	
		displayUnitsLeftToBuild();
		return true;
	}
	
	public void displayUnitsLeftToBuild() {
		if (unitsToBuild.Count < 1) return;
		
		Dictionary dict = new Dictionary();
		foreach(Unit u in unitsToBuild) {
			dict.IncValue(u);
		}
		
		Game.GetInstance().GUI.writeToResourcesTextBox(dict.toString());
	}
	
	public override void beginPlayer(Player p) {
		Game.GUI.showUnitBuy();
		displayUnitsLeftToBuild();
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		displayUnitsLeftToBuild();
		return false;
	}
	
	public ArrayList UnitsToBuild
	{
		get { return unitsToBuild; }
		set { unitsToBuild = value; }
	}
}

class Orig_ChargeMoney : Command {
	int Cost;
	Player curPlay;
	public Orig_ChargeMoney(int c, Player p)
	{
		Cost = c; curPlay = p;
	}
	public override void Execute(Game game)
	{
		curPlay.Money -= Cost;
	}
}

class Orig_PurchaseUnit : Command {
	Unit unit;
	Player curPlay;
	public Orig_PurchaseUnit(Unit nu, Player p)
	{
		unit = nu; curPlay = p;
	}
	public override void Execute(Game game)
	{
		foreach(Stock s in unit.CostResources)
		{
			curPlay.changeResourceStockpile(s);
		}
		curPlay.Money -= unit.CostMoney;
	}
}

class Orig_BuildUnit : Command {
	Unit unit;
	Territory target;
	Player curPlay;

	public Orig_BuildUnit(Unit aunit, Territory atarget, Player p) 
	{
		unit = aunit; target = atarget; curPlay = p;
		undoable = true;
	}
	
	public override void Execute(Game game) 
	{
		if (target != null) {
			target.addUnit(unit);   			
			game.GUI.redrawTerritory(target);
		}
		curPlay.ActiveUnits.Add(unit);
	}
	public override void Unexecute(Game game) 
	{
		if (target != null) {
			target.removeUnit(unit);   			
			game.GUI.redrawTerritory(target);
		}
		curPlay.ActiveUnits.Remove(unit);
	}
}
}
