// created on 08/28/2005 at 09:33
using Gdk;
using System;
using System.Collections;

namespace Gpremacy {
[Serializable]
class Nuke : StrategicUnit {
	public Nuke (Player lord) : base(lord)
	{
	}	
	public override string Name
	{
		get { return "Nuke"; }
	}
	
	public override int CostMoney
	{
		get { return 500; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -1));
			return r;
		}
	}

	override public void draw(Gdk.Window win, int x, int y)
	{
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
		win.DrawPixbuf(context, store.ICBM, 0, 0, x, y, store.ICBM.Width, store.ICBM.Height, RgbDither.Normal, 1, 1);
	}
	
	public override Unit Clone(Player p)
	{
		return new Nuke(p);
	}	
		
}
}