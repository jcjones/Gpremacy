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
using Gpremacy.Network;
 
namespace Gpremacy {
class GameState {
	Game game;
	ArrayList states; // of State
	IEnumerator stateList; // of State
	IEnumerator playerList; // of Player
	
	/* These two objects are to implement randomized gameplay / turn */
	ArrayList playersThisState; // of Player
	IEnumerator playersRandomized; // of Player
	
	State currentState;
	State nextState;
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
		
		// Set up randomization for players
		playersThisState = (ArrayList)game.Players.Clone();
		playersRandomized = playersThisState.GetEnumerator();
		
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
  		stateList.MoveNext();
  		nextState = (Gpremacy.State)stateList.Current;

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
	
	public State NextState
	{
		get { return nextState; }
	}
	
	public ArrayList StateList
	{
		get { return states; }
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
		game.GUI.clearArrow();
		
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))
			currentState.donePlayer(currentPlayer);
		
		do
		{
			if ( playersRandomized.MoveNext() == false) 
			{
				// We've hit the end of our player list, so next state and return.
				playersRandomized.Reset();
				playersRandomized.MoveNext();
				changeToNextState();				
				return currentPlayer = (Gpremacy.Player)playersRandomized.Current; 
			}
			currentPlayer = (Gpremacy.Player)playersRandomized.Current;
			
		} while (!currentPlayer.Active);
		
		/* Run action if this is a local player */
		if (game.LocalPlayers.Contains(currentPlayer))
			currentState.beginPlayer(currentPlayer);
		
		return currentPlayer;		
	}
	
	public int changeToNextState ()
	{	
		int next = nextState.NextOrder;

		/* Begin the next state ... */
		currentState.doneState();
		currentState = nextState;
		currentState.beginState();
		
		/* Assemble list of blind-bidded players */
		playersThisState.Clear();
		foreach(Player p in Game.GetInstance().Players)
		{
			/* Players play if they're active and have bid, or if it's Phase 1.*/
			if (p.Active && p.BlindBidsLeft > 0 && (p.BidIntoNextRound || currentState.MyOrder == 1) )
				playersThisState.Add(p);
		}
				
		if (currentState.MyOrder == 1) turnNumber++;
		
		/* Find the next next state. */
		if (stateList.MoveNext() == false) 
		{
			/* New turn! */
			stateList.Reset();
			stateList.MoveNext();			
		}
		
		/* Next state in list is not the next state, so search for it. */		
		stateList.Reset();
		stateList.MoveNext();
		while ( ((State)stateList.Current).MyOrder != next )
		{
			//System.Console.WriteLine("Searching for " + next + " @ " + ((State)stateList.Current).MyOrder);
			stateList.MoveNext();
		}
		
		/* Set next state */		
		nextState = (Gpremacy.State)stateList.Current;		
				
		/* If we're here, someone wants in. Randomize them. */
		RandomizePlayersForThisState();
				
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
		if (game.gameLink == null)
			throw new Exception("GameLink is null. This should never happen.");

		/* Multi Player */
		foreach (GameParticipant gp in game.gameLink.participants)
		{
			if (gp.player != null)
			{
				Player them = game.PlayerByName(gp.player.Name);
				Console.WriteLine("Activating " + them.Name + " for " + gp.endpoint);
				them.Active = true;
			}
		}
		
		if (game.gameLink is Server)
			game.GiveInitialUnits(); // Should not be in this file..
				 
		if (game.gameLink is LocalLink) 
		{
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
	
	public void RandomizePlayersForThisState() {
		Random r = new Random();

		/* Algorithm: 
  		RandomInx : integer;
  		TempPtr   : pointer;

  		for Inx := aList.Count - 1 downto 1 do begin
	    	RandomInx := Random(Inx + 1);
    		if (RandomInx <> Inx) then begin
	      		TempPtr := aList[Inx];
	      		aList[Inx] := aList[RandomInx];
	      		aList[RandomInx] := TempPtr;
	    	end;
		*/
		
		object TempPtr;
		int RandomIndex;
		int Index;
		for (Index = playersThisState.Count - 1; Index > 0; Index--)
		{
			RandomIndex = r.Next() % Index;
			if (RandomIndex != Index)
			{
				TempPtr = playersThisState[Index];
				playersThisState[Index] = playersThisState[RandomIndex];
				playersThisState[RandomIndex] = TempPtr;
			}
		}
		
		int i=0;
		foreach(Player p in playersThisState) {
			Game.GetInstance().GUI.writeToLog("#"+(++i)+" is " + p.Name);
		}
		
		playersRandomized = playersThisState.GetEnumerator();
		playersRandomized.Reset();
		playersRandomized.MoveNext();	
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

[Serializable]
class Orig_BlindBidIn : Command {
	Player a;
	bool hasBid;
	public Orig_BlindBidIn(Player p, bool b)
	{
		a = p;
		hasBid = b;
	}
	
	public override void Execute(Game game)
	{
		a = game.GetLocalCopyOfPlayer(a);
		a.BidIntoNextRound = hasBid;
		if (hasBid)
			a.BlindBidsLeft--;
	}
}

}