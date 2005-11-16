// created on 11/11/2005 at 01:01
using System.Threading;
using System.Collections;
using Gpremacy.Network;

namespace Gpremacy.AI {

/*	This is the workhorse of the AI, the part which manages incoming commands, etc. */
class Processor {

	int AI_ID;
	Thread AIThread;
	public Player myPlayer;
	AIManager myManager;
	Goal_Composite myBrain;
	public int mySkill;
	Queue incomingCommands; // of Command
	bool doneSomethingThisTurn;

	public Processor(int level, Player p, AIManager m, int id) 
	{
		AI_ID = id;
		myPlayer = p;
		mySkill = level;
		myManager = m;
		incomingCommands = new Queue();
		
		myBrain = new Goal_Cogitate(this);
		
		doneSomethingThisTurn = false;
		
		AIThread = new Thread(new ThreadStart(main));
		AIThread.Start();
	}
	
	public void stop()
	{
		AIThread.Abort();
	}
	
	public void sendCommand(Command cmd)
	{
		Monitor.Enter(myManager);
		myManager.deliverPacket(new DataPacket("Command", cmd), AI_ID);
		Monitor.Exit(myManager);
	}
	
	public void receiveCommand(Command cmd)
	{
		System.Console.WriteLine("AI Player " + myPlayer.Name + " got command " + cmd);
		Monitor.Enter(incomingCommands);
		incomingCommands.Enqueue(cmd);
		Monitor.Exit(incomingCommands);
	}
	
	/* Convenience function for the Goals */
	static public Game theGame
	{
		get {
			/* Do we need some sempahore locking? */
			return Game.GetInstance(); 
		}
	}
	
	/* Useful little functions */
	
	protected bool itsMyTurn()
	{
		/* @TODO Take into account processing delays... */
		return theGame.State.CurrentPlayer == myPlayer;
	}
	
	public void getIdleTacticalUnit(out TacticalUnit idleUnit, out Territory idleHome )
	{
		idleUnit = null;
		idleHome = null;
		
		foreach (Gpremacy.Territory terr in Processor.theGame.GUI.Map.Territories)
		{
			if ( terr.Owner == myPlayer ) 
			{
				ArrayList friends = terr.Friendlies(myPlayer);
				if (friends.Count > 0)
				{
					idleUnit = (TacticalUnit)friends[0];
					idleHome = terr;
					return;
				}
			}
		}
	}
	
	
	/* Thread */
	
	protected void main() {
		/* This routine basically pulls commands from the queue and 
		 * decides what to do with them. For example, begin defending
		 * if an attack comes through. */
		 
		while (true)
		{
			Monitor.Enter(incomingCommands);
			if (incomingCommands.Count > 0)
			{
				Command rcvdCommand = (Command)incomingCommands.Dequeue();
				
				/* Once we get a NextPlayer, we're allowed to
				 * do something again (when its our turn) */
				if (rcvdCommand is Orig_NextPlayer)
					doneSomethingThisTurn = false;
					
				/* Catch defense, etc. */
				if (rcvdCommand is Orig_AttackConventionalStart)
				{
					if ( ((Orig_AttackConventionalStart)rcvdCommand).defender == myPlayer )
					{
						/* Let the brain decide a conventional defense */
						myBrain.HandleCommand(rcvdCommand);
					}
				}
				
				if (rcvdCommand is Orig_AttackStrategicStart)
				{
					if ( ((Orig_AttackStrategicStart)rcvdCommand).defenders.Contains(myPlayer) )				
					{
						/* Let the brain decide a strategic defense */
						myBrain.HandleCommand(rcvdCommand);
					}
				}
			
				/* Now it's our turn... */
				if (itsMyTurn() && !doneSomethingThisTurn)
				{
					/* Let the brain decide what to do */
                                        myBrain.Arbitrate();
					myBrain.HandleState(theGame.State.CurrentState);
					
					/* End our turn */
					Orig_NextPlayer cmd = new Orig_NextPlayer();
					sendCommand(cmd);
				}
			}
	
			Monitor.Exit(incomingCommands);		
			Thread.Sleep(1000);
		}
	}

}
}
