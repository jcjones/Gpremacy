// created on 09/15/2005 at 08:26
using System.Collections;

namespace Gpremacy {
class GpremacyMarket {
	ArrayList commodities; // of Commodity
	int [] marketTable = {1,5,10,25,50,50,100,100,200,200,400,400,500,600,700,800,900,1000};
	
	public GpremacyMarket() 
	{
		commodities = new ArrayList();
	}
	
	public void initResource ( Resource a, int slot )
	{
		commodities.Add( new Commodity(a, slot, marketTable[slot]) );
	}
	
	public int getCommodityCost(Resource r)
	{
		foreach (Commodity com in commodities)
		{
			if (com.Good.Name == r.Name)
			{
				return com.Value;
			}
		}
		return 0;
	}	
	
	public void setCommodityCost(Resource r, int cost)
	{
		foreach (Commodity com in commodities)
		{
			if (com.Good.Name == r.Name)
			{
				com.Value = cost;
				return;
			}
		}
		return;
	}
	
	public int changeCommodityValue(Resource r, int change)
	{
		foreach (Commodity com in commodities)
		{
			if (com.Good.Name == r.Name)
			{
//				System.Console.WriteLine("Changing " + r.Name + " by " + change + " ");
				com.TablePosition -= change;
				if (com.TablePosition > marketTable.Length)
					com.TablePosition = marketTable.Length-1;
				if (com.TablePosition < 0)
					com.TablePosition = 0;
				return com.Value = marketTable[com.TablePosition];
			}
		}
		return 0;
	}	
	
	public string toString () 
	{
		string ret = "";
		foreach (Commodity c in commodities)
		{
			ret += c.Good.Name + " market price at $" + c.Value + "M\n";
		}
		
		return ret;
	}

}

class Commodity {
	Resource good;
	int val;
	int tablePosition;
	
	public Commodity(Resource i, int pos, int v)
	{
		good = i;
		tablePosition = pos;
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
	
	public int TablePosition
	{
		get { return tablePosition; }
		set { tablePosition = value; }		
	} 	
}

}