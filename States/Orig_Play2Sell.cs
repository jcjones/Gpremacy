namespace Gpremacy {
class Orig_Play2Sell : State {
	public Orig_Play2Sell (Game game_i) : base(game_i,2,3)
	{
	}
	
	public override string Name()
	{
		return "Phase 2: Sell Resources";
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
