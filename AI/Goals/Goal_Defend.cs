// created on 11/11/2005 at 18:26
using System.Collections;

namespace Gpremacy.AI {

/* This is the generic "defend against attacks" goal, which also 
 * arbeitrates some unit building. */
 
class Goal_Defend  : Goal {

	public Goal_Defend(Processor bot) : base("Defend", bot)
	{
	}
	

	public override void Arbitrate()
	{		
	}	
	
	public override bool HandleState(Gpremacy.State state)
	{
		if (state is Orig_Play4Move)
		{
			System.Console.WriteLine("Can move.");
			return true;
		}
		if (state is Orig_Play5Build)
		{
			System.Console.WriteLine("Can build.");
			decideAndBuildUnit();
			return true;
		}
		
		return false;
	}		
	
	public override bool HandleCommand(Gpremacy.Command cmd)
	{
		/* See if we can handle this. */
		if (cmd is Gpremacy.Orig_AttackConventionalStart)
		{
			System.Console.WriteLine("Got a conv attack start");
			defendAgainstConventionalAttack((Gpremacy.Orig_AttackConventionalStart)cmd);
			return true;
		}
		
		if (cmd is Gpremacy.Orig_AttackStrategicStart)
		{
			System.Console.WriteLine("Got a strat attack start");
			defendAgainstStrategicAttack((Gpremacy.Orig_AttackStrategicStart)cmd);
			return true;
		}

		return false;
	}
	
	protected void decideAndBuildUnit()
	{
		Unit unitTest = new Army(spinalCord.myPlayer, null);
		
		if (spinalCord.myPlayer.getStockpileNumberFullSets() < 2)
			return;
		if (spinalCord.myPlayer.Money < unitTest.CostMoney);
			return;
			
		Command cmd = new Orig_PurchaseUnit(unitTest, spinalCord.myPlayer);
		spinalCord.sendCommand(cmd);
		
		Gpremacy.Dictionary dictionary = new Gpremacy.Dictionary();

		foreach (Gpremacy.Territory terr in Processor.theGame.GUI.Map.Territories)
		{
			if ( (terr.Owner.CountryID == terr.OriginalOwner) && 
				 (terr.Owner == spinalCord.myPlayer) )
			{
				dictionary.Add(terr, terr.Friendlies(spinalCord.myPlayer).Count);
			}
		}
			
		for (int i=0; i<3; ++i)
		{	
			DictionaryEntry weakest = new DictionaryEntry(); // Why can't this be null?
			Gpremacy.Territory terr = null;
			int weakN = 9999;
			foreach (DictionaryEntry de in dictionary)
			{
					System.Console.WriteLine(weakN + " " + de.Value);
					if (weakN > (int)de.Value)
					{
						weakest = de;
						weakN = (int)de.Value;
					}							
			}
			
			if (weakest.Key == null)
			{
				System.Console.WriteLine(this + " Weakest should never be null!");
				return;
			}
			
			terr = (Gpremacy.Territory)weakest.Key;

			System.Console.WriteLine(this + " buildunit in..." + terr.Name);
			
			dictionary.IncValue(weakest.Key);			
			cmd = new Orig_BuildUnit(new Army(spinalCord.myPlayer, terr), terr, spinalCord.myPlayer);
			spinalCord.sendCommand(cmd);
		}	
	}
	
	protected void defendAgainstConventionalAttack(Gpremacy.Orig_AttackConventionalStart cmd)
	{
		bool fd;
		const int lowResourceNumber = 3; // we have to have fewer than this # 
										 // resource sets to not fully defend 
		const int marginOfSuperiority = 3; // we have to outnumber them by this, also
		
		int sets = spinalCord.myPlayer.getStockpileNumberFullSets();
		
		/* Decide whether to defend fully */
		/* Defend full unless 1) we have no resources left or 
		 * 2) we have vast superiority and low resources */
		 
		fd = true;
		
		if (sets < 1)
			fd = false;
			
		if (sets <= lowResourceNumber)
		{
			// Check superiority in numbers
			Territory t = Processor.theGame.TerritoryByName(cmd.prevTerrName);
			int atkunits = t.Units.Count;
			
			t = Processor.theGame.TerritoryByName(cmd.curTerrName);			
			int defunits = t.Units.Count;
			
			if (defunits > atkunits + marginOfSuperiority)
				fd = false;
				
			System.Console.WriteLine("Low resources: " +atkunits +" / " +defunits +" = " + fd);
				
		}

		System.Console.WriteLine("Defending with choice " + fd);
				
		if (cmd.defender != spinalCord.myPlayer)
			return;
			
		System.Console.WriteLine("Defending sending reply with choice " + fd);
			
		Gpremacy.Orig_AttackConventionalDefenderReady reply = 
			new Gpremacy.Orig_AttackConventionalDefenderReady(cmd.attacker,
															cmd.defender, fd);

		spinalCord.sendCommand(reply);	
	}

	/*@TODO: finish this handler, since I don't want to rewrite it after I 
	 * retool the Strategic attacks */
	protected void defendAgainstStrategicAttack(Orig_AttackStrategicStart cmd)
	{
		int numLStars = spinalCord.myPlayer.getActiveUnitsOfType(new LSat(spinalCord.myPlayer)).Count;
		ArrayList targetsOfMine = new ArrayList();
		
		foreach (Gpremacy.NuclearTarget tgt in cmd.nuclearTargetList)
		{
			if (tgt.territory.Owner == spinalCord.myPlayer)
			{
				targetsOfMine.Add(tgt);
			} 
		} 
		
		// Sort targetsOfMine by # troops in the area
		
		// Allocate LStars
		
		foreach (Gpremacy.NuclearTarget tgt in targetsOfMine)
		{
			if (tgt.icbms < numLStars)
			{
				
			}				 
		}
		
		// Send "defense" packet
	}
}
}
