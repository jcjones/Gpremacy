namespace Gpremacy {
class Orig_Play3Attack : State {
	public Orig_Play3Attack (Game game_i) : base(game_i,3,4)
	{
	}
	
	public override string Name()
	{
		return "Phase 3: Attack";
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
