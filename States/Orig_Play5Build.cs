namespace Gpremacy {
class Orig_Play5Build : State {
	public Orig_Play5Build (Game game_i) : base(game_i,5,6)
	{
	}
	
	public override string Name()
	{
		return "Phase 5: Build Forces and Weapons";
	} 		
	
	public override bool mouseClick(Territory target, uint Button)
	{
		Unit nu;
		if (target.getMapTerritory().isLand)
			nu = new Army(Game.State.CurrentPlayer, target);
		else
			nu = new Navy(Game.State.CurrentPlayer, target);
		
		target.addUnit(nu);   			
		Game.State.CurrentPlayer.ActiveUnits.Add(nu);
		return true;
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
	return false;
	}
	
	public override void done()
	{
	}

}
}
