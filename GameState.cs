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
 
namespace Gpremacy {
class GameState {
	Game game;
	ArrayList states; // of State
	IEnumerator stateList; // of State
	IEnumerator playerList;
	State currentState;
	Player currentPlayer;
	ArrayList commandList; // of Command, for undo/redo
	int turnNumber;
	
	public GameState (Game game_i)
	{
		game = game_i;
		playerList = game.Players.GetEnumerator();
		
		playerList.MoveNext();
		currentPlayer = (Gpremacy.Player)playerList.Current;
		
		//previousTerritory = null;
		//arrowOn = false;
		
		commandList = new ArrayList();
		
		states = new ArrayList();
		states.Add(new Orig_Play1Upkeep(game));
		states.Add(new Orig_Play2Sell(game));
		states.Add(new Orig_Play3Attack(game));
		states.Add(new Orig_Play4Move(game));
		states.Add(new Orig_Play5Build(game));
		states.Add(new Orig_Play6Prospect(game));
		states.Add(new Orig_Conv1Supplies(game));
		states.Add(new Orig_Conv2Roll(game));
		states.Add(new Orig_Conv3Reinforce(game));
		states.Add(new Orig_Conv4CounterAttack(game));
		states.Add(new Orig_Strat1Tactics(game));
		states.Add(new Orig_Strat2LStar(game));
		states.Add(new Orig_Strat3Detonation(game));
		states.Add(new Orig_Strat4CounterAttack(game));

  		stateList = states.GetEnumerator();
  		stateList.MoveNext();
  		currentState = (Gpremacy.State)stateList.Current; 		

		turnNumber = 1;		
		//nextPlayer();
		//nextState();
	}
	
	public string StateIDName
	{
		get { return currentState.Name(); }
	}
	
	public Player CurrentPlayer
	{
		get { return currentPlayer; }
	}
	
	public State CurrentState
	{
		get { return currentState; }
	}	
	
	public ArrayList CommandList
	{
		get { return commandList; }
	}
	
	public int TurnNumber 
	{
		get { return turnNumber; }
	}
	
	public Player nextPlayer() 
	{
		/* Walk forward to find the next Active player, 
		   but if we hit the end of the list
		   in the process, go to the next state.*/
		//arrowOn = false;
		game.GUI.clearArrow();
		
		currentState.donePlayer(currentPlayer);
		
		do
		{
			if ( playerList.MoveNext() == false) 
			{
				playerList.Reset();
				playerList.MoveNext();
				// We've hit the end of our player list, so next state...
				nextState();
			}
			currentPlayer = (Gpremacy.Player)playerList.Current;
			
		} while (!currentPlayer.Active);
		
		currentState.beginPlayer(currentPlayer);
		
		return currentPlayer;		
	}
	
	public int nextState ()
	{	
		int next = currentState.NextOrder;
		stateList.MoveNext();
		if ( ((State)stateList.Current).MyOrder == next )
		{
			currentState.donePlayer(currentPlayer); //TODO: See if these are needed.
			currentState.doneState();
			currentState = (Gpremacy.State)stateList.Current;
			currentState.beginState();
			currentState.beginPlayer(currentPlayer); //TODO: See if these are needed.
			return next;
		}
		/* Next state in list is not the next state, so search for it. */
		
		stateList.Reset();
		stateList.MoveNext();
		while ( ((State)stateList.Current).MyOrder != next )
		{
			System.Console.WriteLine("Searching for " + next + " @ " + ((State)stateList.Current).MyOrder);
			stateList.MoveNext();
		}
			
		currentState.donePlayer(currentPlayer); //TODO: See if these are needed.
		currentState.doneState();
		currentState = (Gpremacy.State)stateList.Current;
		currentState.beginState();
		currentState.beginPlayer(currentPlayer); //TODO: See if these are needed.
		
		/* Count Turn */
		if (currentState.MyOrder == 1) turnNumber++;
		
		return next;						
	}
	
	public void mouseMotion(double x, double y, Territory curTerr)
	{		
		uint button = 0;
		currentState.mouseMotion(x, y, curTerr, button);
	}
	
	public void mouseClick(Territory target, uint Button)
	{
		currentState.mouseClick(target, Button);
   							
   		if (Button == 3)
		{			
			game.GUI.writeToLog("=============================\n" +
						"Clicked " + target.toString() + "\n" +						
						"=============================");
		}			

	}

	public void Execute(Command cmd)
	{
		if (cmd.Undoable)
			commandList.Add(cmd);
		cmd.Execute(game);
		game.GUI.updateGUIStatusBoxes();
		//System.Console.WriteLine("Executed - CL:" + commandList.Count);
	}
	
	public bool Unexecute()
	{
		int pt = commandList.Count-1;
		if (pt < 0)
			return false;
			
		((Command)commandList[pt]).Unexecute(game);
		commandList.RemoveAt(pt);
		
		game.GUI.updateGUIStatusBoxes();
		
		return (pt >= 1); // disable the menu item
	}

}
}