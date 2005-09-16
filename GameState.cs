// created on 09/14/2005 at 22:04

/* 
The Order of Play
Stage 1: Pay Salaries and Loans to the Bank
	$10m / army or navy && $50m / company
Stage 1: Transfer Production Units to Supply Center
Stage 2: Sell All, Some or None of the Supply Units
Stage 3: Attack
Stage 4: Move armies and/or navies
Stage 5: Build Forces and Weapons
Stage 6: Buy or Prospect for Resources
	
Battle Drill

Conventional:
	A: Indentify Theatre of War
	B: Delete Supplies and Deploy Forces
	C: Get the Dice -- Attacker gets 1, Def gets 2, most units gets +1, most lstars gets +1
	D: Roll Dice and Count Losses - 1 kill / 3 points on dice
	E: Move reinforcements - First defender moves, then attacker
	F: Defender may counterattack - Go to step A for either conventional or strategic forces
	
Strategic:
	A: Identify Target of Nuclear Attack
	B: Declare Tactics (Solo or Cluster/MIRV)
	C: Launch Nukes
	D: L-Star Screen - Defender rolls 1d / L-Star. A 1-5 destroys a nuke, a 6 misses
	E: Defender may counterattack - Go to step A for either conventional or strategic forces
*/

using System;
using System.Collections;
 
enum stateNames : int {
	Base,
	Play1Upkeep,
	Play2Sell,
	Play3Attack,
	Play4Move,
	Play5Build,
	Play6Prospect,
	Conv1Supplies,
	Conv2Roll,
	Conv3Reinforce,
	Conv4CounterAttack,
	Strat1Tactics,
	Strat2LStar,
	Strat3Detonation,
	Strat4CounterAttack		
}
 
namespace Gpremacy {
class GameState {
	Game game;
	stateNames stateID;
	IEnumerator playerList;
	Player currentPlayer;
	Territory previousTerritory;
	
	public GameState (Game game_i)
	{
		game = game_i;
		playerList = game.Players.GetEnumerator();
		
		previousTerritory = null;
		
		//nextPlayer();
		nextState();
	}
	
	public int StateID
	{
		get { return (int)stateID; }
	}
	
	public Player CurrentPlayer
	{
		get { return currentPlayer; }
	}
	
	public Player nextPlayer() 
	{
		if ( playerList.MoveNext() == false) 
		{
			playerList.Reset();
			playerList.MoveNext();
			// We've hit the end of our player list, so next state...
			nextState();
		}
		
		currentPlayer = (Gpremacy.Player)playerList.Current;
		
		if (game.GUI != null) 
		{
			game.GUI.writeToOrderOfPlayTextBox("Current Player:\n" + currentPlayer.toString() + "\nCurrent State:\n" + stateID);								
			game.GUI.writeToResourcesTextBox(currentPlayer.toStringResources());			
		}		
		
		return currentPlayer;		
	}
	
	public int nextState ()
	{
		if (game.GUI != null)
			game.GUI.writeToWorldMarketTextBox(game.Market.toString());
	
		if (stateID == stateNames.Play6Prospect) {
			return (int)(stateID = stateNames.Play1Upkeep);
		}
		if (stateID == stateNames.Conv4CounterAttack) {
			return (int)(stateID = stateNames.Play3Attack);
		}		
		if (stateID == stateNames.Strat4CounterAttack) {
			return (int)(stateID = stateNames.Play3Attack);
		}
		return (int)stateID++;				
	}
	
	public void mouseClick(Territory target, uint Button)
	{
		
		if (Button == 3)
   		{
   			if (stateID == stateNames.Play4Move) {
   				if (previousTerritory == null)
   				{
   					previousTerritory = target;
   				} else {
   					// move units from previousTerritory to target.
   					foreach (TacticalUnit unit in previousTerritory.Units)
   					{
   						// Print diagnostics
   						unit.moveTo(target);
   					}
   					previousTerritory = null;   					
   				}
   				
   			} else if ( stateID == stateNames.Play5Build) {
   				Unit nu;
   				if (target.getMapTerritory().isLand)
	   				nu = new Army(CurrentPlayer, target);
   				else
	   				nu = new Navy(CurrentPlayer, target);
   				
   				target.addUnit(nu);   			
   				CurrentPlayer.ActiveUnits.Add(nu);
   			}
   			
   							
   		}
		else
		{			
			game.GUI.writeToLog("=============================\n" +
						"Clicked " + target.toString() + "\n" +						
						"=============================");
		}			
		
	}


}
}