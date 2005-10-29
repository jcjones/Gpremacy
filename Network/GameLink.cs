// created on 10/27/2005 at 15:47
namespace Gpremacy {
class GameLink {		
	public virtual bool sendCommand(Command cmd)
	{
		return false;
	}
	
	public virtual Command getNextCommand()
	{
		return null;
	}
	
	public virtual bool isCommandWaiting()
	{
		return false;
	}
	
	public virtual string listPeers()
	{
		return "";
	}
	
	public virtual int numPeers()
	{
		return 0;
	}
	
	public virtual void parsePacket(DataPacket pkt)
	{
		Game game = Game.GetInstance();
		
		System.Console.WriteLine("Parsing packet " + pkt.identifier + ":");
		/* Acquire semaphore, we're about to mess with the game state. */
		
		switch(pkt.identifier)
		{
		case "Command":
			game.State.NetworkExecuteQueue((Command)pkt.obj);
			break;
		case "BeginGame":
			game.State.BeginGame();
			break;
		}
		/* Release semaphore */
	}
	
	public virtual void stop()
	{
	} 
}
}