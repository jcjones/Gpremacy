// created on 11/10/2005 at 23:23
using System.Collections;
using Gpremacy.Network;

namespace Gpremacy.AI {

/* This class acts as the intermediary between the AI processes and the GameLink. 
 * It's primary goal in life is to spawn AI processes, hand them data and send
 * out their commands. This class is a variation on the Proxy design template.
 */
class AIManager {

	GameLink linkToServer;
	ArrayList AI_Clients; // of Processor
	int numAI;

	public AIManager(GameLink gl)
	{
		linkToServer = gl;
 		AI_Clients = new ArrayList();
 		numAI = 0;
	}
	
	public void stop()
	{
		foreach(Processor p in AI_Clients)
		{
			p.stop();
		}
	}
	
	public void deliverPacket(DataPacket pkt, int id)
	{
		pkt.endpoint = "AI"+id.ToString();
		linkToServer.parsePacket(pkt);
	}
	
	public void receiveCommand(Command cmd)
	{		
		foreach (Processor p in AI_Clients)
		{
			p.receiveCommand(cmd);
		}
	}
	
	public bool startAI(int level, Player p) 
	{
		++numAI;
		
		DataPacket pkt = new DataPacket("Connected", null);
		deliverPacket(pkt, numAI);
		
		AI_Clients.Add(new Processor(level, p, this, numAI));		
		return true;
	}

}
}