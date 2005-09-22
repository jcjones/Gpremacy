namespace Gpremacy {
class Orig_Strat1Tactics : State {
	public Orig_Strat1Tactics (Game game_i) : base(game_i, 21, 22)
	{
	}
	
	public override string Name()
	{
		return "Strategic Step 1: Decide Tactics";
	} 		
	
	public override bool mouseClick(Territory target, uint Button)
	{
return false;
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
return false;
	}

}
}
