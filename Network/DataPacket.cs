// created on 10/27/2005 at 18:29
using System;

namespace Gpremacy {

[Serializable]
class DataPacket {
	public string identifier;
	public System.Object obj;
	
	public DataPacket(string id, System.Object ob)
	{
		obj = ob;
		identifier = id;
	}
	
	public override string ToString() {
		return identifier;
	}

}

}