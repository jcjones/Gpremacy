// created on 08/28/2005 at 09:31
using Gdk;
namespace Gpremacy {
class Unit {

	Player owner;
	bool movedThisTurn;
	
	public Unit(Player lord)
	{
		owner = lord;
		movedThisTurn = true;		
	}
	
	public Player Owner
	{
		get { return owner; }
	}
	
	public virtual void draw(Gdk.Window win, int offset)
	{
	}
	
	public virtual string toString()
	{
		string ret;
		ret = "Unknown Unit of " + owner.toString() + ".\n";
		return ret;
	}
	
	public bool MovedThisTurn
	{
		get { return movedThisTurn; }
		set { movedThisTurn = value; }
	}

}
}