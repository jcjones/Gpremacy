// created on 08/27/2005 at 21:40
using System;
using Gtk;
using Glade;
using Pango;
using Gdk;
using System.Collections;

namespace Gpremacy{

class Territory
{
	String name;
	Player owner;
	int id;
	ArrayList units;
	ArrayList resources;
	bool destroyed;	
	MapTerritory ourMapTerritory;
	
	public Territory(String name_i, int ID_i, Player lord, bool land_i, ArrayList borders_i, Pango.Context pango_context) 
	{
		name = name_i;
		id = ID_i;
		owner = lord;
		ourMapTerritory = new MapTerritory(name_i, land_i, borders_i, pango_context);
		destroyed = false;
		units = new ArrayList();
		resources = new ArrayList();		
	}
	
	public string toString() 
	{
		string ret;
		ret = "\nTerritory " + name + ", owned by " + owner.toString() + ".\n";
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
		set { destroyed = value; }
	}
	
	public int ID
	{
		get { return id ; }
	}
	
	public MapTerritory getMapTerritory()
	{
		return ourMapTerritory;
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
		units.Remove(joe);
	}
	
	public void addResource(Resource a)
	{
		resources.Add(a);
	}
	
	public void draw(Gdk.Window win, Gdk.Color terr, Gdk.Color textcolor, Pango.Context pango_context)
	{
	   	ourMapTerritory.draw(win, terr, textcolor);

	   	for (int offset=0; offset < units.Count && offset < 3; offset++)
	   	{
	   		((TacticalUnit)units[offset]).draw(win, offset*5);
	   	}
	   	if (units.Count > 1)
	   	{
	   		Gdk.GC textcoloring = new Gdk.GC(win);
	   		textcoloring.RgbFgColor = textcolor;
	   		
   	        Pango.Layout label = new Pango.Layout (pango_context);
			label.Wrap = Pango.WrapMode.Word;
			label.FontDescription = FontDescription.FromString ("Tahoma 8");
			label.SetMarkup ( units.Count.ToString() );
			
			int szX, szY;
			label.GetPixelSize(out szX, out szY);

			/*Redraw*/			
//			win.InvalidateRect(new Gdk.Rectangle(ourMapTerritory.centerX, ourMapTerritory.centerY, szX, szY), true);
			
			win.DrawLayout (textcoloring, ourMapTerritory.centerX, ourMapTerritory.centerY, label);
	   	}

	}
	

}
}
