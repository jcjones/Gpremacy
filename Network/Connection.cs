// created on 10/27/2005 at 10:04
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System;

namespace Gpremacy {

class Connection {
	private NetworkStream ns;
	
	public Connection(Socket socket)
	{
		ns = new NetworkStream(socket);		
	}
		
	public void sendObject(object o)
	{
		try
		{
			//StreamWriter sw = new StreamWriter(ns[n]);
			//StreamReader sr = new StreamReader(ns[n]);
			ns.Flush();
			IFormatter sender = new BinaryFormatter();
			sender.Serialize(ns,o);
		}
		catch(Exception er)
		{
			//MessageBox.Show("Problem ... while sending object");
			Game.GetInstance().GUI.ShowError("Problem while sending object: " + er.Message);
		}

	}
	
	public object getObject()
	{
		object data;
		
		IFormatter getter = new BinaryFormatter();
		if(ns.DataAvailable)
		{
			data = (object) getter.Deserialize(ns);
			ns.Flush();
			return data;
		}
		ns.Flush();
		return null;
	}
	
}
}