namespace Gpremacy {
class Orig_Conv1Supplies : State {
	public Orig_Conv1Supplies (Game game_i) : base(game_i,11,12)
	{
	}
	
	public override string Name()
	{
		return "Conventional Step 1: Delete Supplies";
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
