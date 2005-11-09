// created on 09/19/2005 at 21:42
using System;
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
		if (Button != 2)
			return false;
			
		/* Test detonation ! */
		Orig_AttackStrategicDetonate cmd = new Orig_AttackStrategicDetonate(target);
		Game.GetInstance().State.Execute(cmd);
		/* Done testing detonation */ 
		return true;
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		return false;
	}
		
	public override void beginPlayer(Player player)
	{
		/* No charge on turn 1 */
		if (Game.State.TurnNumber == 1)
			return;
			
		Orig_Upkeep cmd = new Orig_Upkeep(player, player.ResourceCards, player.CalculateUpkeep());
		Game.State.Execute(cmd);			
	}	

}

[Serializable]
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
		Play = game.GetLocalCopyOfPlayer(Play);
		
		Play.Money -= Cost;
		foreach (ResourceCard card in ResourceCards)
		{
			if (card.Active)
				Play.changeResourceStockpile(card.Good, card.Good.Value);			
		}

	}
}
}
