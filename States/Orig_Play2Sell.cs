using System;
using System.Collections;

namespace Gpremacy {
class Orig_Play2Sell : State {
	public Orig_Play2Sell (Game game_i) : base(game_i,2,3)
	{
	}
	
	public override string Name()
	{
		return "Phase 2: Sell Resources";
	} 		
	
	public override void beginPlayer(Player aplayer)
	{
		Game.GUI.showMarketBuySell(1);
	}

}

[Serializable]
class Orig_Sell : Command {
	ArrayList stocks; // of Stock
	Player curPlay;
	
	public Orig_Sell(ArrayList res, Player p)
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
