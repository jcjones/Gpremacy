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
	public Orig_Sell(ArrayList res)
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
