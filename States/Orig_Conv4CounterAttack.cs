namespace Gpremacy {
class Orig_Conv4CounterAttack : State {
	public Orig_Conv4CounterAttack (Game game_i) : base(game_i,14,3)
	{
	}
	
	public override string Name()
	{
		return "Conventional Step 4: Counter-Attack";
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
