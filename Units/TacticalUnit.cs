// created on 08/28/2005 at 09:32
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
		return !this.MovedThisTurn;
	}
	
	public bool moveTo ( Territory dest)
	{
		if (canMoveTo(dest)) 
		{
			CurrentLocation.removeUnit(this);
			CurrentLocation = dest;
			CurrentLocation.addUnit(this);
			this.MovedThisTurn = true;
			return true;
		}
		return false;
	}
	
	public override string Name()
	{
		return "Generic Tactical Unit";
	}

}
}