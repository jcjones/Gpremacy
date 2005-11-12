// created on 10/27/2005 at 19:29
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Gpremacy;

namespace Gpremacy.Network {
class Client : GameLink {
	Socket gameSocket;
	Connection gameConnection;
	IPEndPoint gameIP;
	Thread listenData;
	GameSetupView gsv;
	
	public Client (string address, int port)
	{	
		gsv = Game.GetInstance().GUI.GameSetupView;

		Console.WriteLine("Connecting to " + address + " at " + port);
		gsv.addStatusText("Connecting to " + address + " at " + port);
		 
		try 
		{
			gameIP = new IPEndPoint(IPAddress.Parse(address),port);
		} 
		catch (System.FormatException e) 
		{
			gsv.addStatusText("Looking up " + address + " ...");
			try {
				gameIP = new IPEndPoint(System.Net.Dns.GetHostByName(address).AddressList[0], port);
			}
			catch (Exception ex)
			{
				gsv.reenableConnectButton();
				gsv.addStatusText("Could not lookup " + address + " on port " + port + ": " + ex.Message);
				return;
			}
			
		} catch (Exception e) {
			gsv.reenableConnectButton();
			gsv.addStatusText("Unknown error parsing address " + address + " on port " + port + ": " + e.Message);
			return;
		}

		try
		{
			gameSocket = new Socket(System.Net.Sockets.AddressFamily.InterNetwork,System.Net.Sockets.SocketType.Stream,System.Net.Sockets.ProtocolType.Tcp);
			gameSocket.Connect(gameIP);
			gameConnection = new Connection(gameSocket);			
		}
		catch (Exception e)
		{
			gsv.reenableConnectButton();
			gsv.addStatusText("Could not connect to " + address + " on port " + port + ": " + e.Message);
			return;
		}
		
		listenData = new Thread(new ThreadStart(listenForData));
		listenData.Start();

		gsv.addStatusText("Connected.");
		
		DataPacket pkt = new DataPacket("Connected", null);
		gameConnection.sendObject(pkt);
		
		participants = new ArrayList();
	}
	
	public override void stop()
	{
		if (listenData != null)
			listenData.Abort();
		gameSocket.Close();
		gsv.reenableConnectButton();
	}
	
	protected override bool sendPacket(DataPacket pkt)
	{
		if (gameConnection != null)
		{
			gameConnection.sendObject(pkt);
			return true;
		}
		return false;
	}
	
	public override bool sendCommand(Command cmd)
	{
		DataPacket pkt = new DataPacket("Command", cmd);
		return sendPacket(pkt);
	}
	
	public void listenForData()
	{
		DataPacket packet;
		while(gameSocket.Connected)
		{
			Thread.Sleep(10);
		
			if (gameConnection == null)
				continue;
					
			try {
				 packet = (DataPacket) gameConnection.getObject();
			} catch (Exception e)
			{
				System.Console.WriteLine("NetworkClient Received Exception: " + e.Message);
				continue;
			}
				
			if (packet == null) 
				continue;
					
			System.Console.WriteLine("Packet of ["+packet.identifier+"] ");
			
			this.parsePacket(packet);			
		}
		System.Console.WriteLine("Disconnected.");
        Gtk.Application.Invoke (delegate {
           	Game.GetInstance().GUI.ShowError("Connection to server lost.");
        });		
	}
	
}
}