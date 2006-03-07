// created on 03/06/2006 at 10:29
using System;
using System.Collections;
using Gpremacy;

namespace Gpremacy.Network {

class StateManager{
	Server server;
	
	ArrayList allPlayers; 			// of Player
	ArrayList playersThisPhase; 		// of Player
	ArrayList playersNextPhase;		// of Player
	ArrayList playersWaiting;		// of Player
	
	ArrayList stateList; 			// of State
	
	IEnumerator playersRandomized; 	// of Player
	IEnumerator currentPhase; 		// of State
	
	public StateManager(Server server, ArrayList stateList, ArrayList playerList) {
		this.server = server;
		
		this.stateList = (ArrayList)stateList.Clone();
		this.allPlayers = (ArrayList)playerList.Clone(); 
		
		playersThisPhase = new ArrayList();
		playersNextPhase = new ArrayList();
		
		playersRandomized = null;
						
		currentPhase = this.stateList.GetEnumerator();
		currentPhase.Reset();		
		currentPhase.MoveNext();

		/* Special case: Phase 1, upkeep */
		playersThisPhase = (ArrayList)allPlayers.Clone();
		
		/* Randomize players */			
		randomizePlayersForThisPhase();
		playersWaiting = (ArrayList)allPlayers.Clone();		
		
	}
	
	private void executeCommandEverywhere(Command cmd) {
		Game.GetInstance().State.NetworkExecuteQueue(cmd);
		server.sendCommand(cmd);
	}
	
	private void newTurn() {
		playersThisPhase.Clear();
	}
	
	private void gotoNextPhase() {
		/* Find the next next state. */
		int next = ((State)currentPhase.Current).NextOrder;	
		
		/* Check if we're done with the list */
		if (currentPhase.MoveNext() == false) 
		{
			/* Entirely new turn! Set appropriate stuff */
			newTurn();
		}
		
		/* Search for next state */		
		currentPhase.Reset();
		currentPhase.MoveNext();
		while ( ((State)currentPhase.Current).MyOrder != next )
		{
			//System.Console.WriteLine("Searching for " + next + " @ " + ((State)stateList.Current).MyOrder);
			currentPhase.MoveNext();
		}
		
		State nextState = (State)currentPhase.Current;
		
		System.Console.WriteLine("Next Phase is " + nextState.Name());
		
		Orig_NextState cmd = new Orig_NextState(nextState);
		executeCommandEverywhere(cmd);
		
	}
	
	private void allPlayersDoneThisPhase() {
		gotoNextPhase();
		
		/* Swap player lists */
		ArrayList t = playersThisPhase;
		playersThisPhase = playersNextPhase;
		playersNextPhase = t;
		playersNextPhase.Clear();
		
		/* Special case: Phase 1, upkeep */
		if ( ((State)currentPhase.Current).MyOrder == 1) {
			playersThisPhase = (ArrayList)allPlayers.Clone();
		}
		
		/* Randomize players */			
		randomizePlayersForThisPhase();
		
		playersWaiting = (ArrayList)allPlayers.Clone();
	}
	
	private void playerReady(Player readyPlayer) {
		playersWaiting.Remove(readyPlayer);
		
		bool allDone = true;
		
		foreach(Player player in playersWaiting) {
			if (player.Active) {
				allDone = false;
				break;
			}
		}
		
		System.Console.WriteLine("StateManager:PlayerReady: Player " + readyPlayer.Name + " is ready and AllDone = " + allDone + ".");

		if (allDone)
			allPlayersDoneThisPhase();
	}
	
	private void playerDone(Player player) {
		Player nextPlayer = null;
		
		if ((Player)playersRandomized.Current != player) {
			System.Console.WriteLine("Player done out of order");
			//return;
		}
		
		// If no players are active, this will be an infinite loop.
		for (;;) {
			/* Check if we're done with the list */
			if (playersRandomized.MoveNext() == false) 
			{
				System.Console.WriteLine("StateManager:playerDone: Done with random list, iterator over...");
				return;
			}
			
			nextPlayer = (Player)playersRandomized.Current;
			
			if (nextPlayer.Active)
				break;
				
			System.Console.WriteLine("StateManager:playerDone: Scanning for next active player");
		}
		
		
		System.Console.WriteLine("Next Player is " + nextPlayer.Name);
		Orig_NextPlayer cmd = new Orig_NextPlayer(nextPlayer);
		executeCommandEverywhere(cmd);
		
	}
	

	public bool parseCommand(CommandForServer cmd) {
		if (cmd is Orig_DoneWithPhase) {
			/* Nastiness to get a local player copy */
			Player player = 	Game.GetInstance().GetLocalCopyOfPlayer(((Orig_DoneWithPhase)cmd).player);
			
			System.Console.WriteLine("Player " + player.Name + " is done with phase...");
			
			playerDone(player);
			
			return true;
		}
		
		if (cmd is Orig_BlindBidIn) {			
			/* Nastiness to get a local player copy */
			Player player = 	Game.GetInstance().GetLocalCopyOfPlayer(((Orig_BlindBidIn)cmd).player);
			
			/* If they've chosen to bid into the next phase, add them to said phase */
			if (((Orig_BlindBidIn)cmd).hasBid) {
				playersNextPhase.Add(player);
				System.Console.WriteLine("Player " + player.Name + " has bid into the next phase");				
			}
			
			/* In either case, they're done with this phase  ... */
			playerReady(player);
			
			return true;			
		}
		
		return false;
	}
	
	private void randomizePlayersForThisPhase() {
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
		for (Index = playersThisPhase.Count - 1; Index > 0; Index--)
		{
			RandomIndex = r.Next() % Index;
			if (RandomIndex != Index)
			{
				TempPtr = playersThisPhase[Index];
				playersThisPhase[Index] = playersThisPhase[RandomIndex];
				playersThisPhase[RandomIndex] = TempPtr;
			}
		}
		
		int i=0;
		foreach(Player p in playersThisPhase) {
			Game.GetInstance().GUI.writeToLog("#"+(++i)+" is " + p.Name);
		}
		
		playersRandomized = playersThisPhase.GetEnumerator();
		playersRandomized.Reset();
		playersRandomized.MoveNext();	
	}	

}
}
