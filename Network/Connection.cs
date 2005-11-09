// created on 10/27/2005 at 10:04
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
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
		Monitor.Enter(ns);

		try
		{
			ns.Flush();
			IFormatter sender = new BinaryFormatter();
			sender.Serialize(ns,o);
		}		
		catch(Exception er)
		{
			Game.GetInstance().GUI.ShowError("Problem while sending object: " + er.Message + " to " + ns.ToString());
		}
		finally
		{
			Monitor.Exit(ns);
		}

	}
	
	public object getObject()
	{
		object data;
		
		IFormatter getter = new BinaryFormatter();
		
		Monitor.Enter(ns);		
		if(ns.DataAvailable)
		{
			data = (object) getter.Deserialize(ns);
			ns.Flush();
			
			Monitor.Exit(ns);
			return data;
		}
		ns.Flush();
		Monitor.Exit(ns);
		
		return null;
	}
	
}
}