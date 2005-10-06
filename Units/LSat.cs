// created on 08/28/2005 at 09:34
using System.Collections;

namespace Gpremacy {
class LSat : StrategicUnit {
	bool FiredThisTurn;
	public LSat (Player lord) : base(lord)
	{
	}	

	public override string Name
	{
		get { return "L-Sat"; }
	}
	
	public override int CostMoney
	{
		get { return 1000; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -2));
			return r;
		}
	}

}
}