// created on 10/27/2005 at 15:47
using System;
using System.Net;
using System.Collections;
using System.Threading;
using Gpremacy.AI;

namespace Gpremacy.Network {

[Serializable]
class GameParticipant{
	public Player player;
	public string endpoint;
	
	public GameParticipant(Player p, string e)
	{ player = p; endpoint = e; }
}

class GameLink {
	public ArrayList participants; // of GameParticipant
	public AIManager BrainManager;
	
	public GameLink() 
	{
	}
			
	public virtual bool sendCommand(Command cmd)
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
			
			if (c.endpoint != "")
				res += c.endpoint+"\n";
			else
				res += "Server\n";
		}
		
		return res;
	}
	
	public virtual void parseParticipantList(ArrayList list)
	{
		if ( (list != null) && (list.Count > 0) && (list[0] is GameParticipant) )
		{
			Monitor.Enter(participants); 
			participants = list;
			Monitor.Exit(participants);
		} else {
			System.Console.WriteLine("Received a bad participant list.");
		}
	}
	
	public virtual int numPeers()
	{
		if (participants == null)
			return 0;

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
		
		switch(pkt.identifier)
		{
		case "Command":
			if (this is Server) {
				if (pkt.obj is CommandForServer)
					((Server)this).serverGameManager((CommandForServer)pkt.obj);
				else
					((Server)this).propagateCommand(pkt);
			}
				
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
	}
	
	public virtual void stop()
	{
		if (BrainManager != null)
			BrainManager.stop();	
	} 
	
	public virtual bool playerChoicesValid() 
	{
		return false;
	}
}
}