// created on 08/28/2005 at 09:32
using System.Collections;

namespace Gpremacy {
class TacticalUnit : Unit {
	protected Territory CurrentLocation;

	public TacticalUnit (Player lord, Territory home) : base(lord)
	{
		CurrentLocation = home;
		System.Console.WriteLine("New unit created in " + CurrentLocation.toString());
	}	
	
	public bool canMoveTo ( Territory dest )
	{		
		return true;
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
	
	public virtual ArrayList UnitsAboard
	{
		get { return new ArrayList(); }
	}
	
	public override string Name
	{
		get { return "Generic Tactical Unit"; }
	}
	
	public override int Upkeep
	{
		get { return 10; }
	}
}
}