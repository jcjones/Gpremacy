// created on 08/28/2005 at 09:33
using Gdk;
using System;
using System.Collections;

namespace Gpremacy {
[Serializable]
class Navy : TacticalUnit {
	Territory hold; // for armiesAboard;

	public Navy (Player lord, Territory home) : base (lord, home)
	{
		hold = new Territory(-1, "Ship hold", lord.CountryID, lord, false, null, null);
	}	
	override public void draw(Gdk.Window win, int x, int y)
	{
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
		win.DrawPixbuf(context, store.Navy, 0, 0, x, y, store.Army.Width, store.Army.Height, RgbDither.Normal, 1, 1);
	}	
	
	public override string Name
	{
		get { return "Navy"; }
	}
	
	public override int CostMoney
	{
		get { return 300; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -1));
			r.Add(new Stock(new Oil(), -1));
			r.Add(new Stock(new Grain(), -1));
			return r;
		}
	}
	
	public override string Costs
	{
		get { return "per 3: $300 M and a set of supplies"; }
	}		
	
	public override string toString()
	{
		string ret;
		ret = Name + " of " + owner.toString() + " with " + UnitsAboardCount + " units aboard.\n";
		return ret;
	}
	
	public override bool canMoveTo ( Territory dest )
	{		
		/* Enforce distance */
		if (Game.GetInstance().GUI.Map.distanceBetween(CurrentLocation, dest) > 1)
			return false;
			
		/* If the territory is a deep sea, can move */
		if (!dest.IsLand && dest.IsDeep)
			return true;
		
		/* Only allow movement to unoccupied territories, then. */
		return dest.occupiable(owner);
	}
	
	public override bool CanHoldTroops
	{
		get { return true; }
	}
	
	public override int UnitsAboardCount
	{
		get { return hold.Units.Count; }
	}

	public override void DeleteUnitsAboard(int num)
	{
		if (num < 0 || num > UnitsAboardCount) 
			num = UnitsAboardCount;

		ArrayList oldUnits = (ArrayList)hold.Units.Clone();
		System.Console.Write("Hold removing [");
		
		for (int i=0; i<num; i++)
		{
			hold.removeUnit((Unit)oldUnits[i]);   			
			owner.ActiveUnits.Remove((Unit)oldUnits[i]);
			System.Console.Write(((Unit)oldUnits[i]).Name + " ");
		}
		
		System.Console.WriteLine("]");
	}
	
	public Territory Hold 
	{
		get { return hold; }
	}
	
	public override ArrayList UnitsAboard
	{
		get { return hold.Units; }
	}
	
	public void loadUnit(TacticalUnit joe)
	{
		hold.addUnit(joe);
	}
	
	public TacticalUnit unloadUnit(TacticalUnit joe)
	{
		hold.removeUnit(joe);
		return joe;
	}
	
	public override ArrayList calculateMovementCost(Territory b)
	{
		ArrayList r = new ArrayList(1);		
		r.Add(new Stock(new Oil(), -1));
		return r; 		
	}
	
	public override Unit Clone(Player p)
	{
		return new Navy(p, CurrentLocation);
	}

	
}
}