namespace Gpremacy {
class Orig_Strat3Detonation : State {
	public Orig_Strat3Detonation (Game game_i) : base(game_i, 23, 24)
	{
	}
	
	public override string Name()
	{
		return "Strategic Step 3: Detonation";
	} 		
	
	public override bool mouseClick(Territory target, uint Button)
	{
return false;
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
