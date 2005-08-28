// created on 08/28/2005 at 09:32
namespace Gpremacy {
class TacticalUnit : Unit {
	protected Territory CurrentLocation;

	public TacticalUnit (Player lord, Territory home) : base(lord)
	{
		CurrentLocation = home;
		System.Console.WriteLine("New unit created in " + CurrentLocation.getName());
	}	
	
	public bool canMoveTo ( Territory dest )
	{		
		return !this.getMovedThisTurn();
	}
	
	public bool moveTo ( Territory dest)
	{
		if (canMoveTo(dest)) 
		{
			CurrentLocation = dest;
			this.setMovedThisTurn(true);
			return true;
		}
		return false;
	}

}
}