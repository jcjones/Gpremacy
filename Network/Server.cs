// created on 10/27/2005 at 09:20
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Gpremacy {

class ClientConnection {
	public Connection connection;
	public Player player;
	public Socket socket;
	
	public ClientConnection(Socket s)
	{
		socket = s;
		connection = new Connection(s);
		player = null;
		System.Console.WriteLine("Found new connection from " + s.RemoteEndPoint.ToString());
	}	

	public bool isConnected()
	{
		return socket.Connected;
	}
	
	public override string ToString()
	{
		if (socket.Connected)
			return socket.RemoteEndPoint.ToString();
		else
			return "Disconnected: " + socket.RemoteEndPoint.ToString(); 	
	}	
	
}

class Server : GameLink {
	Socket gameSocket;
	ArrayList clients; // of ClientConnection
	bool acceptingConnections;
	
	Thread listenJoins;
	Thread listenData;
	
	public Server (int port)
	{
		clients = new ArrayList();
		
		acceptingConnections = true;
		
		gameSocket     = new Socket(System.Net.Sockets.AddressFamily.InterNetwork,System.Net.Sockets.SocketType.Stream,System.Net.Sockets.ProtocolType.Tcp);
		IPEndPoint ip1 = new IPEndPoint(IPAddress.Any,port);
		gameSocket.Bind(ip1);
		gameSocket.Listen(10);
						
		listenJoins = new Thread(new ThreadStart(listenForJoins));
		listenData = new Thread(new ThreadStart(listenForData));		
		
		listenJoins.Start();
		listenData.Start();
	}
	
	public override int numPeers()
	{
		return clients.Count;
	}
	
	public override string listPeers()
	{
		string res = "";
		foreach(ClientConnection c in clients)
		{
			if (c.player != null)
				res += c.player.Name + " ";
			else
				res += "Unknown Player ";
				
			res += c.socket.RemoteEndPoint.ToString()+"\n";
		}
		
		return res;
	}

	public void sendBeginGame()
	{
		acceptingConnections = false;
		DataPacket pkt = new DataPacket("BeginGame", null);
		sendPacket(pkt);
		Game.GetInstance().State.BeginGame(); // Start it here		
	}
	
	public override void stop()
	{
		if (listenJoins != null)
			listenJoins.Abort();
		if (listenData != null)
			listenData.Abort();
	}
	
	public override bool sendCommand(Command cmd)
	{
		DataPacket pkt = new DataPacket("Command", cmd);
		sendPacket(pkt);		
		return true;
	}
	
	public void sendPacket(DataPacket pkt)
	{
		foreach(ClientConnection c in clients)
		{
			c.connection.sendObject(pkt);
		}
	}	

	public void listenForData()
	{
		ArrayList toRemove = new ArrayList();
		DataPacket packet;
		while(true)
		{
			toRemove.Clear();			
		
			foreach (ClientConnection c in clients)
			{
				if (c.connection == null)
					continue;
				if (!c.isConnected())
					toRemove.Add(c);
					
				try {
					 packet = (DataPacket) c.connection.getObject();
				} catch (Exception e)
				{
					continue;
				}
				
				if (packet == null) 
					continue;
					
				System.Console.WriteLine("Packet of ["+packet.identifier+"] from " + c.ToString());
				this.parsePacket(packet);
			}

			/* Cleanup Disconnects */
			foreach (ClientConnection c in toRemove)
			{
				System.Console.WriteLine("Disconnected from " + c.socket.RemoteEndPoint.ToString());
				clients.Remove(c);
			}
			
			Thread.Sleep(10);
		}
	}

	public void listenForJoins()
	{
		while(acceptingConnections)
		{
			Socket tmp = gameSocket.Accept();
			ClientConnection client = new ClientConnection(tmp);
			clients.Add(client);
			Thread.Sleep(10);			
		}
	}				
	

}
}