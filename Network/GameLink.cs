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
		switch(pkt.identifier)
		{
		case "Command":
			game.State.NetworkExecute((Command)pkt.obj);
			break;
		case "BeginGame":
			game.State.BeginGame();
			break;
		}
	}
	
	public virtual void stop()
	{
	} 
}
}