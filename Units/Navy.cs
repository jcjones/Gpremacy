// created on 08/28/2005 at 09:33
using Gdk;
using System.Collections;

namespace Gpremacy {
class Navy : TacticalUnit {
	Territory hold; // for armiesAboard;

	public Navy (Player lord, Territory home) : base (lord, home)
	{
		hold = new Territory(-1, "Ship hold", lord.CountryID, lord, false, null, null);
	}	
	override public void draw(Gdk.Window win, int offset)
	{	
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
	   	
		int X = CurrentLocation.MapTerritory.centerX+offset;
		int Y = CurrentLocation.MapTerritory.centerY+offset;
		
		win.DrawPixbuf(context, store.Navy, 0, 0, X, Y, store.Navy.Width, store.Navy.Height, RgbDither.Normal, 1, 1);
	}
	
	public override string Name
	{
		get { return "Navy"; }
	}
	
	public override string toString()
	{
		string ret;
		ret = Name + " of " + owner.toString() + " with " + UnitsAboardCount + " units aboard.\n";
		return ret;
	}
	
	public override bool canMoveTo ( Territory dest )
	{		
		return (1 == Game.GetInstance().GUI.Map.distanceBetween(CurrentLocation, dest));
	}
	
	public override bool CanHoldTroops
	{
		get { return true; }
	}
	
	public override int UnitsAboardCount
	{
		get { return hold.Units.Count; }
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
	
}
}