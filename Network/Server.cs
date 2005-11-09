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
		string str = "";
		if (!socket.Connected)
			str += "Disconnected: ";
			
		try {
			str += socket.RemoteEndPoint.ToString();
		} catch (Exception e)
		{
			str += "somewhere?";
		}
		
		if (player != null)
			str += " as Player: " + player.Name;
		
		return str;
	}	
	
}

class Server : GameLink {
	Socket gameSocket;
	ArrayList clients; // of ClientConnection
	ArrayList clientsToRemove;
	bool acceptingConnections;

	GameParticipant localhost;
	
	Thread listenJoins;
	Thread listenData;
		
	GameSetupView gsv;
	
	public Server (int port)
	{
		gsv = Game.GetInstance().GUI.GameSetupView;
		
		clients = new ArrayList();
		clientsToRemove= new ArrayList();
		
		acceptingConnections = true;
		
		gameSocket     = new Socket(System.Net.Sockets.AddressFamily.InterNetwork,System.Net.Sockets.SocketType.Stream,System.Net.Sockets.ProtocolType.Tcp);
		IPEndPoint ip1 = new IPEndPoint(IPAddress.Any,port);
		gameSocket.Bind(ip1);
		gameSocket.Listen(10);
						
		listenJoins = new Thread(new ThreadStart(listenForJoins));
		listenData = new Thread(new ThreadStart(listenForData));		
		
		listenJoins.Start();
		listenData.Start();
		
		participants = new ArrayList();
		localhost = new GameParticipant(null, null);
		
		Console.WriteLine("Listening on port " + port);
		gsv.addStatusText("Listening on port " + port);
		
        GLib.Timeout.Add (700, new GLib.TimeoutHandler (sendParticipantList));
		GLib.Timeout.Add (1000, new GLib.TimeoutHandler (cleanClients));
	}
	
	public override int numPeers()
	{
		return clients.Count;
	}
	
	public bool sendParticipantList()
	{
		participants.Clear();
		
		// Acquire mutex for clients
		Monitor.Enter(clients);
		foreach(ClientConnection c in clients)
		{
			if (!c.isConnected())
				continue;
			GameParticipant gp = new GameParticipant(c.player, c.socket.RemoteEndPoint);
			participants.Add(gp);
		}
		Monitor.Exit(clients);
		
		participants.Add(localhost);
		
		DataPacket pkt = new DataPacket("ParticipantList", participants);
		sendPacket(pkt);
		
		return acceptingConnections; // Return false when we're done accepting connections
	}
	
	public void updatePlayerCountryName(DataPacket pkt)
	{
		Monitor.Enter(clients);
		foreach(ClientConnection c in clients)
		{
			/* we're doing a string compare here, this is moronic. But it works... */
			if (c.socket.RemoteEndPoint.ToString() == pkt.endpoint.ToString()) 
			{
				c.player = Game.GetInstance().PlayerByName((string)pkt.obj);
				sendParticipantList();
				Monitor.Exit(clients);
				return;
			}
		}
		Monitor.Exit(clients);
	}
	
	public override void sendWhoIAm(string name)
	{
		/* Keep track of ourself ... */
		localhost.player = Game.GetInstance().PlayerByName(name);
		sendParticipantList();
	}
	
	public override bool playerChoicesValid() 
	{
		ArrayList seen = new ArrayList(); // of GameParticipant
		
		/* Send out the newest participant list which also ensures that
		 * participants is as up to date as possible before we validate it.
		 */
		 
		sendParticipantList();
		
		foreach (GameParticipant participant in participants)
		{
			if (participant.player == null)
				return false;
			if (seen.Contains(participant.player))
				return false;
			System.Console.WriteLine("Havent seen " + participant.player.Name + " before, adding...");
			seen.Add(participant.player);
		}
		System.Console.WriteLine("Good!");
		return true;	
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
			
		gameSocket.Close();
			
		Monitor.Enter(clients);
		foreach(ClientConnection c in clients)
			c.socket.Close();
		Monitor.Exit(clients);
		
		gsv.reenableConnectButton();
		gsv.addStatusText("Stopped server.");		
	}
	
	public bool cleanClients()
	{
		if (clientsToRemove.Count < 1)
			return true;
			
		/* Cleanup Disconnects */
		Monitor.Enter(clients);
		foreach (ClientConnection c in clientsToRemove)
		{
			if (clients.Contains(c)) 
			{
				System.Console.WriteLine("Disconnected from " + c);
				clients.Remove(c);
			}
		}
		Monitor.Exit(clients);
		
		Monitor.Enter(clientsToRemove);
		clientsToRemove.Clear();
		Monitor.Exit(clientsToRemove);
		
		return true;
	}
	
	public override bool sendCommand(Command cmd)
	{
		DataPacket pkt = new DataPacket("Command", cmd);
		sendPacket(pkt);		
		return true;
	}
	
	protected override bool sendPacket(DataPacket pkt)
	{
		Monitor.Enter(clients);
		foreach(ClientConnection c in clients)
		{
			if (!c.isConnected())
				continue;
			try {
				c.connection.sendObject(pkt);
			} catch (Exception e) {
				System.Console.WriteLine("Could not send packet " + pkt + " to " + c);
			}
		}
		Monitor.Exit(clients);
		return true;
	}
	
	public virtual bool propagateCommand(DataPacket pkt)
	{
		System.Console.Write("[Server] Propagating packet " + pkt.ToString());
		foreach(ClientConnection c in clients)
		{
			/* Do not send back to its incoming host. */
			if (pkt.endpoint.ToString() != c.socket.RemoteEndPoint.ToString()) {
				if (c.isConnected())
					c.connection.sendObject(pkt);
				System.Console.Write(" to " + c.ToString() + ",");
			}
		}
		System.Console.WriteLine();
		return true;
	}	

	public void listenForData()
	{
		DataPacket packet = null;
		while(true)
		{
			// Acquire mutex for clients
			Monitor.Enter(clients);
			foreach (ClientConnection c in clients)
			{				
				if (c.connection == null)
					continue;
				if (!c.isConnected())
				{
					clientsToRemove.Add(c);
					continue;
				}
					
				try {
					 packet = (DataPacket) c.connection.getObject();
				} catch (SocketException e) 
				{
					System.Console.WriteLine("Disconnecting " + c.ToString());
					clientsToRemove.Add(c);
					packet = null;
				} catch (Exception e)
				{
					clientsToRemove.Add(c);
					System.Console.WriteLine("NetworkServer Received Exception: " + e.Message);
					continue;
				}
				
				if (packet == null) 
					continue;
				
				packet.endpoint = c.socket.RemoteEndPoint;
					
				System.Console.WriteLine("Packet of ["+packet.identifier+"] from " + c.ToString());
				this.parsePacket(packet);
			}
			Monitor.Exit(clients);
			
			Thread.Sleep(10);
		}
	}

	public void listenForJoins()
	{
		while(acceptingConnections)
		{
			Socket tmp = gameSocket.Accept();
			ClientConnection client = new ClientConnection(tmp);
			
			Monitor.Enter(clients);
			clients.Add(client);
			Monitor.Exit(clients);

        	Gtk.Application.Invoke (delegate {
              	sendParticipantList();
        	});

			Thread.Sleep(10);			
		}
	}				
	

}
}