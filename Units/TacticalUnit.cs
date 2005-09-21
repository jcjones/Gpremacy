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
		return true;
	}
	
	public bool moveTo ( Territory dest)
	{
		System.Console.WriteLine("trying to move to " + dest.toString());
		if (canMoveTo(dest)) 
		{
			CurrentLocation.removeUnit(this);
			dest.addUnit(this);
			
			CurrentLocation = dest;			
			System.Console.WriteLine("Moving to " + CurrentLocation.toString());
			return true;
		}
		return false;
	}
	
	public override string Name
	{
		get { return "Generic Tactical Unit"; }
	}

}
}