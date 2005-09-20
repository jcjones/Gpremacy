// created on 09/19/2005 at 21:42
namespace Gpremacy {
class Orig_Play1Upkeep : State {
	public Orig_Play1Upkeep (Game game_i) : base(game_i,1,2)
	{
	}
	
	public override string Name()
	{
		return "Phase 1: Pay Upkeep and Reap Benefits";
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
