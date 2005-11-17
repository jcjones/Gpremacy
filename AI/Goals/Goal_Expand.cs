// created on 11/11/2005 at 18:26
using System.Collections;

namespace Gpremacy.AI {

/* This is the generic "defend against attacks" goal, which also 
 * arbeitrates some unit building. */
 
class Goal_Expand  : Goal {

	Gpremacy.Oil oil;

	public Goal_Expand(Processor bot) : base("Expand", bot)
	{
		oil = new Gpremacy.Oil();
	}
	

	public override void Arbitrate()
	{		
	}	
	
	public override bool HandleState(Gpremacy.State state)
	{
		if (state is Orig_Play4Move)
		{
			System.Console.WriteLine("Can expand.");
			acquireInactiveResources();
			return true;
		}
		if (state is Orig_Play6Prospect)
		{
			System.Console.WriteLine("Can prospect.");
			doProspectForResources();
			return true;
		}
		
		return false;
	}		
	
	private void acquireInactiveResources()
	{
		foreach(Gpremacy.ResourceCard rc in spinalCord.myPlayer.ResourceCards)
		{
			if (spinalCord.myPlayer.getStockpileAmount(oil) < 4)
				return;
				
			if (rc.HasBeenActive == false && rc.Place.occupiable(spinalCord.myPlayer))
			{
				/* We can go there, so pick a unit and go. */
				Gpremacy.TacticalUnit u; Gpremacy.Territory prev; ArrayList acost;				
				spinalCord.getIdleTacticalUnit(out u, out prev);
				
				if ( u == null || prev == null )
					return;
				
				acost = u.calculateMovementCost(rc.Place);

				/* Take care of walking */
				if (acost.Count > 1)
					acost.Remove(0);
								
				Command cmd = new Orig_MoveUnit(spinalCord.myPlayer, u, rc.Place, prev, acost);
				spinalCord.sendCommand(cmd);
			}			
		}
	}
	
	private void doProspectForResources()
	{
		if (spinalCord.myPlayer.Money < DeckDealer.GetInstance().CostPerCard * 4)
			return;
		
		if (spinalCord.myPlayer.getStockpileNumberFullSets() > 6)
			return;
			
		Gpremacy.Dictionary dict = new Gpremacy.Dictionary();
		foreach (Gpremacy.ResourceCard rc in spinalCord.myPlayer.ResourceCards)
		{
			dict.IncValue(rc.Good, rc.Good.Value);
		}
		
		Resource lowest = null;
		int lowN = 999;
		foreach (DictionaryEntry de in dict)
		{
			if ((int)de.Value < lowN)
			{
				lowN = (int)de.Value;
				lowest = (Resource)de.Key;
			}
		}
		
		if (lowest == null)
			return;
		
		System.Console.WriteLine(this + " is prospecting for " + lowest);
		
		Processor.theGame.ShuffleResourceCards();
		
		int cardsFlipped;
		
		for (cardsFlipped=1; cardsFlipped<4; ++cardsFlipped)
		{			
			if (Processor.theGame.JustShuffledResourceCards)
				Processor.theGame.JustShuffledResourceCards = false;
			else
				Processor.theGame.NextResourceCard();
				
			ResourceCard card = Processor.theGame.CurrentResourceCard;
			
			if (card == null)
				break;
			
			if (card.isResource() && (card.Good.Name == lowest.Name) )
			{
				System.Console.WriteLine(this + " has found " + card);
				/* Add this card to the player's hand, marked unopened */
				card.Active = false;
				Orig_AddResourceCard cmd4 = new Orig_AddResourceCard(card, spinalCord.myPlayer);
				spinalCord.sendCommand(cmd4);
				/* get this card, break */
				break;				
			}			
		}
		
		int totalCost = cardsFlipped*DeckDealer.GetInstance().CostPerCard;
		Orig_ChargeMoney cmd3 = new Orig_ChargeMoney(totalCost, spinalCord.myPlayer);
		spinalCord.sendCommand(cmd3);
								
		
	}
	
}
}
