// created on 08/28/2005 at 09:31
using Gdk;
namespace Gpremacy {
class Unit {

	Player owner;
	
	public Unit(Player lord)
	{
		owner = lord;	
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
		ret = Name + " of " + owner.toString() + ".\n";
		return ret;
	}
	
	public virtual string Name
	{
		get { return "Generic Unit"; }
	}

}
}