using System;
using System.Collections;

namespace Gpremacy {
class Orig_Play6Prospect : State {
	public Orig_Play6Prospect (Game game_i) : base(game_i, 6, 1)
	{
	}
	
	public override string Name()
	{
		return "Phase 6: Buy and Prospect Resources";
	} 		
	
	public override void beginPlayer(Player aplayer)
	{
		Game.GUI.showMarketBuySell(0);
	}
	
}

[Serializable]
class Orig_AddResourceCard : Command
{
	ResourceCard card;
	Player player;
	public Orig_AddResourceCard(ResourceCard c, Player p) 
	{
		card = c;
		player = p;
	}
	
	public override void Execute(Game game) 
	{
		player.addResourceCard(card);
		game.RemoveResourceCard(card);
	}
}

[Serializable]
class Orig_Buy : Command 
{
	ArrayList stocks; // of Stock
	public Orig_Buy(ArrayList res)
	{
		stocks = res;
	}

	public override void Execute(Game game) 
	{
		foreach (Stock stuff in stocks) 
		{
			game.State.CurrentPlayer.changeResourceStockpile(stuff);
			game.State.CurrentPlayer.Money += -1*stuff.Number*game.Market.getCommodityCost(stuff.Good);
			game.Market.changeCommodityValue(stuff.Good, -1*stuff.Number);
		}
	}
}
}
