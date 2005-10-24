// created on 08/28/2005 at 09:31
using Gdk;
using System.Collections;

namespace Gpremacy {
class Unit {

	protected Player owner;
	
	public Unit(Player lord)
	{
		owner = lord;	
	}
	
	public Player Owner
	{
		get { return owner; }
	}
	
	public virtual Unit Clone(Player p)
	{
		return new Unit(p);
	}
	public Unit Clone()
	{
		return this.Clone(owner);
	}
		
	public virtual void draw(Gdk.Window win, int offset)
	{
	}
	
	public virtual void draw(Gdk.Window win, int x, int y)
	{
	}	
	
	public virtual string toString()
	{
		string ret;
		ret = Name + " of " + owner.toString() + ".\n";
		return ret;
	}
	
	public virtual string Name
	{
		get { return "Generic Unit"; }
	}
	
	public virtual int Upkeep
	{
		get { return 0; }
	}
	
	public virtual int CostMultiplicity
	{
		get { return 1; }
	}
	
	public virtual int CostMoney
	{
		get { return 0; }
	}
	
	public virtual ArrayList CostResources
	{
		get { return new ArrayList(); }
	}
	
	public virtual string Costs
	{
		get {
			string s = "$" + CostMoney + " M ";

			if (CostResources.Count > 0)
				s += "and ";
				
			foreach (Stock r in CostResources) {
				s += (-1*r.Number) + " " + r.Good.Name + " ";
		 	}
		 	return s; 
		}
	}
}
}