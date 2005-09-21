// created on 08/28/2005 at 15:05
namespace Gpremacy {
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
	
}
}