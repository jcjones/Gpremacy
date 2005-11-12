// created on 11/10/2005 at 23:31
using Gpremacy.AI;
using System.Collections;

namespace Gpremacy.Network {

	/* This class is a local with bots-only game connection. */

class LocalLink : GameLink {

	int AIcounter;

	public LocalLink() 
	{
		AIcounter = 0;
		BrainManager = new AIManager(this);
 		participants = new ArrayList();	// of GameParticipant				
	}
	
	public void startAIPlayer(int difficulty, Player who)
	{
		participants.Add(new GameParticipant(who, "AI"+(AIcounter++).ToString() ) );
		BrainManager.startAI(difficulty, who);		
	}
	
	protected override bool sendPacket(DataPacket pkt)
	{
		if (pkt.identifier == "Command")
			BrainManager.receiveCommand((Command)pkt.obj);
		return true;
	}	
	
	public override bool sendCommand(Command cmd)
	{
		BrainManager.receiveCommand(cmd);
		return false;
	}

}
}