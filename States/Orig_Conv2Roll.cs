namespace Gpremacy {
class Orig_Conv2Roll : State {
	public Orig_Conv2Roll (Game game_i) : base(game_i,12,13)
	{
	}
	
	public override string Name()
	{
		return "Conventional Step 2: Gather and Roll Dice";
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
