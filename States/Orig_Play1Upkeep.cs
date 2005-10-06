// created on 09/19/2005 at 21:42

using System.Collections;

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
		
	public override void beginPlayer(Player player)
	{
		Orig_Upkeep cmd = new Orig_Upkeep(Game.State.CurrentPlayer, Game.State.CurrentPlayer.ResourceCards, calculateUpkeep());
		Game.State.Execute(cmd);			
	}	
	
	private int calculateUpkeep()
	{
		int amount = 0;
		foreach (Unit u in Game.State.CurrentPlayer.ActiveUnits)
		{
			amount += u.Upkeep;
		}
		foreach (ResourceCard c in Game.State.CurrentPlayer.ResourceCards)
		{
			amount += c.Upkeep;
		}
		return amount;
	}
	

}
class Orig_Upkeep : Command {
	ArrayList ResourceCards;
	Player Play;
	int Cost;

	public Orig_Upkeep(Player p, ArrayList rc, int ct) 
	{
		Play = p; ResourceCards = rc; Cost = ct;
		undoable = false;
	}
	
	public override void Execute(Game game) 
	{
		Play.Money -= Cost;
		foreach (ResourceCard card in ResourceCards)
		{
			Play.changeResourceStockpile(card.Good, card.Good.Value);			
		}

	}
}
}
