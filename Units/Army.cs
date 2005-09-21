// created on 08/28/2005 at 09:33
using Gdk;

namespace Gpremacy {
class Army : TacticalUnit {
	public Army (Player lord, Territory home) : base (lord, home)
	{
	}	

	override public void draw(Gdk.Window win, int offset)
	{	
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
	   	
		int X = CurrentLocation.getMapTerritory().centerX+offset;
		int Y = CurrentLocation.getMapTerritory().centerY+offset;
		
		win.DrawPixbuf(context, store.Army, 0, 0, X, Y, store.Army.Width, store.Army.Height, RgbDither.Normal, 1, 1);
	}
	
	public override string Name
	{
		get { return "Army"; }
	}
		
}
}