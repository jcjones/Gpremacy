namespace Gpremacy {
class Orig_Strat4CounterAttack : State {
	public Orig_Strat4CounterAttack (Game game_i) : base(game_i, 24, 3)
	{
	}
	
	public override string Name()
	{
		return "Strategic Step 4: Counter-Attack";
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
