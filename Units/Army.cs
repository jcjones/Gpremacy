// created on 08/28/2005 at 09:33
using Gdk;
using System;
using System.Collections;

namespace Gpremacy {
[Serializable]
class Army : TacticalUnit {
	public Army (Player lord, Territory home) : base (lord, home)
	{
	}	
	
	override public void draw(Gdk.Window win, int x, int y)
	{
		Gdk.GC context = new Gdk.GC(win);
 		GraphicsStorage store = GraphicsStorage.GetInstance();
		win.DrawPixbuf(context, store.Army, 0, 0, x, y, store.Army.Width, store.Army.Height, RgbDither.Normal, 1, 1);
	}
	
	public override string Name
	{
		get { return "Army"; }
	}
	
	public override int CostMoney
	{
		get { return 300; }
	}
	
	public override ArrayList CostResources
	{
		get { 
			ArrayList r = new ArrayList(); 
			r.Add(new Stock(new Minerals(), -1));
			r.Add(new Stock(new Oil(), -1));
			r.Add(new Stock(new Grain(), -1));
			return r;
		}
	}
	
	public override string Costs
	{
		get { return "per 3: $300 M and a set of supplies"; }
	}				
		
	public override bool canMoveTo ( Territory dest )
	{		
		return (dest.occupiable(owner)); 
	}

	public override ArrayList calculateMovementCost(Territory b)
	{
		ArrayList r = new ArrayList();

		/*@TODO: Do distance calculations between here CurrentLocation and b. */
		int dist = Game.GetInstance().GUI.Map.distanceBetween(CurrentLocation, b);
		System.Console.WriteLine("Distance: " + dist);
		
		if (dist < 2) {
			/* If the distance is 1, we can march */
			r.Add(new Stock(new Grain(), -1));
			if (dist < 1) // checking minimal move for Always March
				return r;
		}

		/* Can always airlift */
		r.Add(new Stock(new Oil(), -2));
				
		return r;  		
	}

	public override Unit Clone(Player p)
	{
		return new Army(p, CurrentLocation);
	}
		
}
}