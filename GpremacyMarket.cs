// created on 09/15/2005 at 08:26
using System.Collections;

namespace Gpremacy {
class GpremacyMarket {
	ArrayList commodities; // of Commodity
	
	public GpremacyMarket() 
	{
		commodities = new ArrayList();
	}
	
	public void initResource ( Resource a, int val )
	{
		commodities.Add( new Commodity(a, val) );
	}
	
	public string toString () 
	{
		string ret = "";
		foreach (Commodity c in commodities)
		{
			ret += c.Good.Name() + " market price at " + c.Value + "\n";
		}
		
		return ret;
	}

}

class Commodity {
	Resource good;
	int val;
	
	public Commodity(Resource i, int v)
	{
		good = i;
		val = v;
	}
	
	public Resource Good
	{	
		get { return good; }
	}
	
	public int Value
	{
		get { return val; }
		set { val = value; }
	} 	
}

}