namespace Gpremacy {
class Orig_Strat2LStar : State {
	public Orig_Strat2LStar (Game game_i) : base(game_i, 22, 23)
	{
	}
	
	public override string Name()
	{
		return "Strategic Step 2: L-Star Screen";
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
