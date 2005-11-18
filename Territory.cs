// created on 08/27/2005 at 21:40
using Gtk;
using Glade;
using Pango;
using Gdk;
using System;
using System.Collections;

namespace Gpremacy{
[Serializable]
class Territory
{
	String name;
	Player owner;
	int id, originalOwner;
	ArrayList units;
	ArrayList resources;
	bool destroyed;	
	
	public Territory(int ID_i, String name_i, int originalOwner_i, Player lord) 
	{
		name = name_i;
		id = ID_i;
		originalOwner = originalOwner_i;
		owner = lord;

		destroyed = false;
		units = new ArrayList();
		resources = new ArrayList();
							
	}
	
	public ArrayList Units
	{
		get { return units; }
	}
	
	public ArrayList Resources
	{
		get { return resources; }
	}
	
	public Gdk.Color Color
	{
		get { 
			if (!MapTerritory.IsLand)
				return new Gdk.Color(18,100,186);
			else
				return Owner.Color;
		}
	}

	public ArrayList Friendlies(Player bob)
	{
		ArrayList friends = new ArrayList();
		foreach (TacticalUnit joe in units)
		{
			if (joe.Owner.Equals(bob))
			{
				Console.WriteLine("Added unit " + joe.Name + " to friendlies list.");
				friends.Add(joe);
			}
		}
		return friends;
	}
	
	public bool occupiedBy (Player IFF)
	{
		// Could just check owner and units.Count, but this is 
		// more generic, if we add allied occupation
		return (Friendlies(IFF).Count > 0);
	}
	
	public bool occupiable (Player IFF)
	{
		foreach (TacticalUnit joe in units)
		{
			if (!joe.Owner.Equals(IFF))
				return false;
		}
		System.Console.WriteLine("Appears to be occupiable.");
		return true;
	}
	
	public string toString() 
	{
		string ret;
		ret = "\nTerritory " + name + " [#"+ID+"], owned by " + owner.toString() + ".\n";
		if (IsLand)
			ret += "[Is Land] ";
		if (IsDeep)
			ret += "[Is Deep] ";
		ret += "Unit List:\n";
		foreach (Unit u in units)
		{
			ret +="\t" + u.toString() + "\n";
		}
		ret += "Resource List:\n";
		foreach (Resource r in resources)
		{
			ret +="\t" + r.toString() + "\n";		
		}
		ret += "\n";
		
		return ret;
	}
	
	public Player Owner
	{
		get { return owner; }
		set { owner = value; }
	}
	
	public bool Destroyed
	{
		get { return destroyed; }
		set { if (MapTerritory.IsLand) destroyed = value; }
		/* You can't destroy the ocean, man... whoa */
	}
	
	public int OriginalOwner
	{
		get { return originalOwner; }
	}
	
	/* Unique ID for this territory, if it's on the map. Naval units have ID -1 */
	public int ID
	{
		get { return id ; }
	}
	
	public MapTerritory MapTerritory
	{
		get { return Game.GetInstance().MapTerritoryByID(id); }
	}
	
	public String Name
	{
		get { return name; }
	}
	
	public void addUnit(Unit joe)
	{
		units.Add(joe);		
	}
	
	public void removeUnit(Unit joe)
	{
		if (!units.Contains(joe)) {
			System.Console.WriteLine("Attempt to remove unit " + joe.toString() + " has failed. Here is:");
			foreach(Unit u in units)
			{
				System.Console.WriteLine("U: " + u.toString());
			}
			
			throw new Exception("Removing unit which isn't here!");
		}
		units.Remove(joe);
	}
	
	public void addResource(Resource a)
	{
		resources.Add(a);
	}
	
	public bool IsLand
	{
		get { return MapTerritory.isLand; }
	}
	
	public bool IsDeep
	{
		get { return MapTerritory.deepSea; }
	}
			
	public void showNuke(Gdk.Window win, int frame)
	{
			Gdk.GC whocares = new Gdk.GC(win);
 			GraphicsStorage store = GraphicsStorage.GetInstance();
 			 			
 			Gdk.Pixbuf gfx = store.getFrame(store.Detonation, frame, store.DetonationFrames);

 			int x = MapTerritory.centerX - (gfx.Width/2);
 			int y = MapTerritory.centerY - (gfx.Height/2);
 			
			win.DrawPixbuf(whocares, gfx, 0, 0, x, y, gfx.Width, gfx.Height, RgbDither.Normal, 1, 1);	   		
	}
		
	public void draw(Gdk.Window win, Gdk.Color terr, Gdk.Color textcolor, Pango.Context pango_context)
	{
   		Gdk.GC textcoloring = new Gdk.GC(win);
   		textcoloring.RgbFgColor = textcolor;
	   	int carriedUnits = 0;
	   	string extraLabel = "";
		GraphicsStorage store = GraphicsStorage.GetInstance();	   	
		int cenTerrX = MapTerritory.centerX;
		int cenTerrY = MapTerritory.centerY;

	   	/* Show flag */
	   	Gdk.Pixbuf flag = store.AppropriateFlag(owner.CountryID);
	   	win.DrawPixbuf(textcoloring, flag, 0, 0, cenTerrX, cenTerrY, flag.Width, flag.Height, RgbDither.Normal, 1, 1);
	   	
		/* Draw the map */ 
	   	MapTerritory.draw(win, terr, textcolor);
	   	
	   	/* Show radiation if destroyed */
	   	if (destroyed)
			win.DrawPixbuf(textcoloring, store.Radiation, 0, 0, cenTerrX, cenTerrY, store.Radiation.Width, store.Radiation.Height, RgbDither.Normal, 1, 1);	   		
	   	
	   	/* Determine personnel carrier status */
	   	foreach(TacticalUnit joe in units)
	   		carriedUnits += joe.UnitsAboardCount;	   		
	   	if (carriedUnits > 0)
	   		extraLabel = "(" + carriedUnits + ")";
	   		   		
		/* Draw the first N units staggered, then use a label to show further #'s */
	   	for (int offset=0; offset < units.Count && offset < 3; offset++)
	   	{
	   		((TacticalUnit)units[offset]).draw(win, offset*5);
	   	}
	   	
	   	/* Label */
	   	if (units.Count > 1 || carriedUnits > 0)
	   	{	   		
   	        Pango.Layout label = new Pango.Layout (pango_context);
			label.Wrap = Pango.WrapMode.Word;
			label.FontDescription = FontDescription.FromString ("Tahoma 8");
			label.SetMarkup ( units.Count.ToString() + extraLabel );
			
			int szX, szY;
			label.GetPixelSize(out szX, out szY);

			/*Redraw*/			
//			win.InvalidateRect(new Gdk.Rectangle(MapTerritory.centerX, MapTerritory.centerY, szX, szY), true);
			
			win.DrawLayout (textcoloring, MapTerritory.centerX, MapTerritory.centerY, label);
	   	}

	}
	

}
}
