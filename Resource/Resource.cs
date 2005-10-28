// created on 08/28/2005 at 15:05
using System;

namespace Gpremacy {
[Serializable]
class Resource {
	Player owner;
	int val;
	
	public Resource (Player lord, int i)
	{
		owner = lord;
		val = i;
	}
	
	public Player Owner
	{
		get { return owner; }
		set { owner = value; }
	}
	public int Value
	{
		get { return val; }
		set { val = value; }
	}
	
	virtual public string toString()
	{
		string ret = Value + " " + Name + "\n";
		return ret;
	}
	
	virtual public string Name
	{		
		get { return "Unknown"; }
	}
	
	override public int GetHashCode() {
		return Name.GetHashCode();
	}
	
}
}