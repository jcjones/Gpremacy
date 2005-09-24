namespace Gpremacy {
class Orig_Play5Build : State {
	int numberOfArmies;
	public Orig_Play5Build (Game game_i) : base(game_i,5,6)
	{
		numberOfArmies = 3;
	}
	
	public override string Name()
	{
		return "Phase 5: Build Forces and Weapons";
	} 		
	
	public override bool mouseClick(Territory target, uint Button)
	{
		Unit nu;
		if (target.MapTerritory.isLand)
			nu = new Army(Game.State.CurrentPlayer, target);
		else
			nu = new Navy(Game.State.CurrentPlayer, target);
	
		if (numberOfArmies > 2) {
			numberOfArmies = 0;
			Orig_PurchaseUnit ucmd = new Orig_PurchaseUnit(nu);
			Game.State.Execute(ucmd);
		}
		
		numberOfArmies++;
		Game.GUI.writeToLog("You can build " + (3-numberOfArmies) + " more units for this price.");
	
		Orig_BuildUnit cmd = new Orig_BuildUnit(nu, target);
		Game.State.Execute(cmd);	
		
		return true;
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		return false;
	}

}
class Orig_PurchaseUnit : Command {
	Unit unit;
	public Orig_PurchaseUnit(Unit nu)
	{
		unit = nu;
	}
	public override void Execute(Game game)
	{
		game.State.CurrentPlayer.changeResourceStockpile(new Minerals(),-1);
		game.State.CurrentPlayer.changeResourceStockpile(new Grain(),-1);
		game.State.CurrentPlayer.changeResourceStockpile(new Oil(),-1);
		game.State.CurrentPlayer.Money -= 300;
	}
}

class Orig_BuildUnit : Command {
	Unit unit;
	Territory target;

	public Orig_BuildUnit(Unit aunit, Territory atarget) 
	{
		unit = aunit; target = atarget;
		undoable = true;
	}
	
	public override void Execute(Game game) 
	{
		target.addUnit(unit);   			
		game.State.CurrentPlayer.ActiveUnits.Add(unit);
		game.GUI.redrawTerritory(target);
	}
	public override void Unexecute(Game game) 
	{
		target.removeUnit(unit);   			
		game.State.CurrentPlayer.ActiveUnits.Remove(unit);
		game.GUI.redrawTerritory(target);
	}
}
}
