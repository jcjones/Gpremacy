namespace Gpremacy {
class Orig_Conv3Reinforce : State {
	public Orig_Conv3Reinforce (Game game_i) : base(game_i,13,14)
	{
	}
	
	public override string Name()
	{
		return "Conventional Step 3: Move Reinforcements";
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
