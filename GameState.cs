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
	Queue networkCommands; // of Command	
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
		networkCommands = new Queue();
		
		states = new ArrayList();
		states.Add(new Orig_Play1Upkeep(game));
		states.Add(new Orig_Play2Sell(game));
		states.Add(new Orig_Play3Attack(game));
		states.Add(new Orig_Play4Move(game));
		states.Add(new Orig_Play5Build(game));
		states.Add(new Orig_Play6Prospect(game));

  		stateList = states.GetEnumerator();
  		stateList.MoveNext();
  		currentState = (Gpremacy.State)stateList.Current; 		

		turnNumber = 1;		
		//nextPlayer();
		//nextState();
		
		GLib.Timeout.Add (1000, new GLib.TimeoutHandler (NetworkExecuteRun));
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
		
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))
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
		
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))
			currentState.beginPlayer(currentPlayer);
		
		return currentPlayer;		
	}
	
	public int nextState ()
	{	
		int next = currentState.NextOrder;
		if (stateList.MoveNext() == false) 
		{
			/* New turn! */
			stateList.Reset();
			stateList.MoveNext();			
		}
		
		if ( ((State)stateList.Current).MyOrder == next )
		{
			/* Run action if this is a local player */
			if (game.LocalPlayers.Contains(currentPlayer))			
				currentState.donePlayer(currentPlayer); //TODO: See if these are needed.
				
			currentState.doneState();
			currentState = (Gpremacy.State)stateList.Current;
			currentState.beginState();
			
			/* Run action if this is a local player */
			if (game.LocalPlayers.Contains(currentPlayer))			
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
			
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))			
			currentState.donePlayer(currentPlayer); //TODO: See if these are needed.
			
		currentState.doneState();
		currentState = (Gpremacy.State)stateList.Current;
		currentState.beginState();
		
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))		
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
		System.Console.WriteLine("STATE: Registered click on " + target.Name);
			
		if (game.LocalPlayers.Contains(currentPlayer))
		{
			System.Console.WriteLine("STATE: passed on click " + target.Name);
			currentState.mouseClick(target, Button);
		}
		else
		{
			System.Console.WriteLine("Could not find " + currentPlayer.Name + " in localPlayers: " +((Player)game.LocalPlayers[0]).Name);
		} 
   							
   		if (Button == 3)
		{			
			game.GUI.writeToLog("=============================\n" +
						"Clicked " + target.toString() + "\n" +						
						"=============================");
		}			

	}
	
	public void BeginGame()
	{
		Game game = Game.GetInstance();
		
		game.GUI.GameSetupView.GUI.Hide();
		bool singlePlayer = false;
		
		/* Setup participants */
		if (game.gameLink != null)
		{
			/* Multi Player */
			foreach (GameParticipant gp in game.gameLink.participants)
			{
				if (gp.player != null)
				{
					Player them = game.PlayerByName(gp.player.Name);
					Console.WriteLine("Activating " + them.Name);
					them.Active = true;
				}
			}
			singlePlayer = false;
			
			if (game.gameLink is Server)
				game.GiveInitialUnits(); // Should not be in this file..
		} else {
			/* Single Player */
			singlePlayer = true;	
			Player self = game.PlayerByName(game.GUI.GameSetupView.whoAmI(true));
			Console.WriteLine("Local Game starting with player " + self.Name);
			self.Active = true; // This needs to be here, despite the us.Active below. 
			game.GiveInitialUnits(); // Should not be in this file..					
		}
		
		/* Current Player is the first one active */
		playerList.Reset();
		playerList.MoveNext();		
		do
		{
			if ( playerList.MoveNext() == false) 
			{
				playerList.Reset();
				playerList.MoveNext();
			}
			currentPlayer = (Gpremacy.Player)playerList.Current;
			Console.WriteLine("BeginGame: CurrentPlayer = " + currentPlayer.Name);
			
		} while (!currentPlayer.Active);
		
		Console.WriteLine("BeginGame: Selected player = " + currentPlayer.Name);
		
		/* Setup ourselves */
		Player us = game.PlayerByName(game.GUI.GameSetupView.whoAmI(singlePlayer));
		game.LocalPlayers.Add(us);
		us.Active = true;
				
		game.DistributeResourceCards(); // Should not be in this file..
		
		/* If we make an immediate statusbox update, we'll crash occasionally 
		 * because of a gtktextview bug. So we set a timer. */
		GLib.Timeout.Add (600, new GLib.TimeoutHandler (performBeginGameStatusUpdate));
	}
	
	private bool performBeginGameStatusUpdate() {
		Game game = Game.GetInstance();

		string str = "You are playing ";
		for (int i=0; i<game.LocalPlayers.Count; ++i)
		{
			str += ((Player)game.LocalPlayers[i]).Name;
			if (i+1 == game.LocalPlayers.Count)
				str += ".";
			else
				str += ", ";
		}
		
		game.GUI.updateGUIStatusBoxes();		
		game.GUI.writeToLog(str);
		return false;
	}

	public void Execute(Command cmd)
	{
		commandList.Add(cmd);
		cmd.Execute(game);
		
		/* Send to the network */
		if (game.gameLink != null)
			game.gameLink.sendCommand(cmd);
		
		/* Show results */		
		game.GUI.updateGUIStatusBoxes();
		//System.Console.WriteLine("Executed - CL:" + commandList.Count);
	}
	
	public void NetworkExecuteQueue(Command cmd)
	{
		networkCommands.Enqueue(cmd);	
	}
	
	/* This routine is called every 1000 ms by Glib.Timeout, setup in the constructor */
	public bool NetworkExecuteRun()
	{
		Command cmd;
		while (networkCommands.Count > 0)
		{
			cmd = (Command)networkCommands.Dequeue();
			NetworkExecute(cmd);	
		}
		
		/* Show results... makes sure the client stays up to date */		
		game.GUI.updateGUIStatusBoxes();
		return true; // Returning false would stop the timeout from ever running again
	}
	
	public void NetworkExecute(Command cmd)
	{		
		commandList.Add(cmd);
		
		System.Console.WriteLine("Network execution of " + cmd);
		cmd.Execute(game);
				
	}
	
	public bool Unexecute()
	{
		int pt = commandList.Count-1;
		if (pt < 0)
			return false;
		Command cmd = (Command)commandList[pt];
		
		if (cmd.Undoable) {
			/* Only if it's marked as undoable do we allow an undo */
			cmd.Unexecute(game);
			commandList.RemoveAt(pt);
		
			game.GUI.updateGUIStatusBoxes();
		}
		
		return (pt >= 1); // disable the menu item
	}

}

[Serializable]
class Orig_NextPlayer : Command {
	public Orig_NextPlayer() 
	{
	}
	
	public override void Execute(Game game)
	{
		game.State.nextPlayer();
		System.Console.WriteLine("Next Player: " + game.State.CurrentPlayer.Name + " in " + game.State.CurrentState.Name());
	}
}

}