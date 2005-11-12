// created on 10/27/2005 at 18:29
using System;

namespace Gpremacy.Network {

[Serializable]
class DataPacket {
	public string identifier;
	public System.Object obj;
	public string endpoint;
	
	public DataPacket(string id, System.Object ob)
	{
		obj = ob;
		identifier = id;
	}
	
	public override string ToString() {
		if (endpoint != null)
			return identifier + " from " + endpoint.ToString();
		return identifier;
	}

}

}