// created on 08/28/2005 at 09:34
using Gdk;
using System;
using System.Collections;

namespace Gpremacy {
[Serializable]
class LSat : StrategicUnit {
	bool firedThisTurn;
	public LSat (Player lord) : base(lord)
	{
		firedThisTurn = false;
	}	

	public override string Name
	{
		get { return "L-Sat"; }
	}
	
	public override int CostMoney
	{
		get { return 1000; }
	}
	
	public bool FiredThisTurn
	{
		get { return firedThisTurn; }
		set { firedThisTurn = value; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -2));
			return r;
		}
	}
	
	override public void draw(Gdk.Window win, int x, int y)
	{
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
		win.DrawPixbuf(context, store.LSat, 0, 0, x, y, store.LSat.Width, store.LSat.Height, RgbDither.Normal, 1, 1);
	}
	
	public override Unit Clone(Player p)
	{
		return new LSat(p);
	}

}
}