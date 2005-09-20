namespace Gpremacy {
class Orig_Play6Prospect : State {
	public Orig_Play6Prospect (Game game_i) : base(game_i, 6, 1)
	{
	}
	
	public override string Name()
	{
		return "Phase 6: Buy and Prospect Resources";
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
