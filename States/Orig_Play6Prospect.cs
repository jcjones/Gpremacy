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
		player = game.GetLocalCopyOfPlayer(player);
		player.addResourceCard(card);
		game.RemoveResourceCard(card);
	}
}

[Serializable]
class Orig_Buy : Command 
{
	ArrayList stocks; // of Stock
	Player curPlay;
	public Orig_Buy(ArrayList res, Player p)
	{
		stocks = res;
		curPlay = p;
	}

	public override void Execute(Game game) 
	{
		curPlay = game.GetLocalCopyOfPlayer(curPlay);
		foreach (Stock stuff in stocks) 
		{
			curPlay.changeResourceStockpile(stuff);
			curPlay.Money += -1*stuff.Number*game.Market.getCommodityCost(stuff.Good);
			game.Market.changeCommodityValue(stuff.Good, -1*stuff.Number);
		}
	}
}
}
