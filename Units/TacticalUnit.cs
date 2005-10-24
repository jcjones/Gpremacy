// created on 08/28/2005 at 09:32
using System.Collections;

namespace Gpremacy {
class TacticalUnit : Unit {
	protected Territory CurrentLocation;

	public TacticalUnit (Player lord, Territory home) : base(lord)
	{
		CurrentLocation = home;
	}	
	
	public virtual bool canMoveTo ( Territory dest )
	{
		return true;
	}

	override public void draw(Gdk.Window win, int offset)
	{	
		int X = CurrentLocation.MapTerritory.centerX+offset;
		int Y = CurrentLocation.MapTerritory.centerY+offset;
		this.draw(win, X, Y);
	}	
	
	public bool moveTo ( Territory dest )
	{
		CurrentLocation = dest;
		return true;
	}
	
	public virtual bool CanHoldTroops
	{
		get { return false; }
	}
	
	public virtual int UnitsAboardCount
	{
		get { return 0; }
	}
	
	public virtual void DeleteUnitsAboard(int num)
	{		
	}
	
	public virtual ArrayList UnitsAboard
	{
		get { return new ArrayList(); }
	}
	
	public override string Name
	{
		get { return "Generic Tactical Unit"; }
	}
	
	public override int CostMultiplicity
	{
		get { return 3; }
	}
		
	public virtual ArrayList calculateMovementCost(Territory b)
	{
		return new ArrayList();
	}
	
	public override int Upkeep
	{
		get { return 10; }
	}
}
}