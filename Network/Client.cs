// created on 10/27/2005 at 19:29
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Gpremacy {
class Client : GameLink {
	Socket gameSocket;
	Connection gameConnection;
	IPEndPoint gameIP;
	
	
	public Client (string address, int port)
	{	
		try
		{
			gameIP = new IPEndPoint(IPAddress.Parse(address),port);
			gameSocket = new Socket(System.Net.Sockets.AddressFamily.InterNetwork,System.Net.Sockets.SocketType.Stream,System.Net.Sockets.ProtocolType.Tcp);
			gameSocket.Connect(gameIP);
			gameConnection = new Connection(gameSocket);			
		}
		catch (Exception e)
		{
			GameSetupView gsv = Game.GetInstance().GUI.GameSetupView;
			gsv.addStatusText("Could not connect to " + address + " on port " + port + ": " + e.Message);
			return;
		}
		
		this.sendCommand(new Command());
	}
	
	public override bool sendCommand(Command cmd)
	{
		DataPacket pkt = new DataPacket("Command", cmd);
		if (gameConnection != null)
		{
			gameConnection.sendObject(pkt);
			return true;
		}
		return false;
	}
	
}
}