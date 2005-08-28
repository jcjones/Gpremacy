// created on 08/28/2005 at 09:31
using Gdk;
namespace Gpremacy {
class Unit {

	Player Owner;
	bool MovedThisTurn;
	
	public Unit(Player lord)
	{
		Owner = lord;
		MovedThisTurn = true;		
	}
	
	public virtual void draw(Gdk.Window win, int offset)
	{
	}
	
	public bool getMovedThisTurn()
	{
		return MovedThisTurn;
	}
	
	public void setMovedThisTurn(bool ans)
	{
		MovedThisTurn = ans;
	} 

}
}