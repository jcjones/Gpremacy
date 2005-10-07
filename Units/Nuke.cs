// created on 08/28/2005 at 09:33
using System.Collections;

namespace Gpremacy {
class Nuke : StrategicUnit {
	public Nuke (Player lord) : base(lord)
	{
	}	
	public override string Name
	{
		get { return "Nuke"; }
	}
	
	public override int CostMoney
	{
		get { return 500; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -1));
			return r;
		}
	}
	
	public override Unit Clone(Player p)
	{
		return new Nuke(p);
	}	
		
}
}