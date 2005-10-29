// created on 10/27/2005 at 15:47
using System;
using System.Net;
using System.Collections;

namespace Gpremacy {

[Serializable]
class GameParticipant{
	public Player player;
	public System.Net.EndPoint endpoint;
	
	public GameParticipant(Player p, System.Net.EndPoint e)
	{ player = p; endpoint = e; }
}

class GameLink {
	public ArrayList participants;
			
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
	
	public virtual string peerListToString()
	{
		string res = "";
		foreach(GameParticipant c in participants)
		{
			if (c.player != null)
				res += c.player.Name + " ";
			else
				res += "Unknown Player ";
			
			if (c.endpoint != null)
				res += c.endpoint.ToString()+"\n";
			else
				res += "Server\n";
		}
		
		return res;
	}
	
	public virtual void parseParticipantList(ArrayList list)
	{
		if ( (list.Count > 0) && (list[0] is GameParticipant) ) 
			participants = list;
		else
			System.Console.WriteLine("Received a bad participant list.");
	}
	
	public virtual int numPeers()
	{
		return participants.Count;
	}
	
	protected virtual bool sendPacket(DataPacket pkt)
	{
		return false;
	}
	
	public virtual void sendWhoIAm(string name)
	{
		DataPacket pkt = new DataPacket("CountryName", name);
		sendPacket(pkt);
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
		case "CountryName":
			if (this is Server)
				((Server)this).updatePlayerCountryName(pkt);
			break;
		case "ParticipantList":
			parseParticipantList((ArrayList)pkt.obj);
			break;
		}
		/* Release semaphore */
	}
	
	public virtual void stop()
	{
	} 
}
}