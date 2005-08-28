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
	String Name;
	Player Owner;
	int ID;
	ArrayList Units;
	ArrayList Resources;
	bool Destroyed;	
	MapTerritory ourMapTerritory;
	
	public Territory(String name_i, int ID_i, bool land_i, ArrayList borders_i, Pango.Context pango_context) 
	{
		Name = name_i;
		ID = ID_i;
		ourMapTerritory = new MapTerritory(name_i, land_i, borders_i, pango_context);
		Destroyed = false;
		Units = new ArrayList();
		Resources = new ArrayList();		
	}
	
	public void setOwner(Player inPlayer)
	{
		Owner = inPlayer;
	}

	public void setDestroyed(bool stat)
	{
		Destroyed = stat;
	}
	
	public Player getOwner()
	{
		return Owner;
	}
	
	public bool getDestroyed()
	{
		return Destroyed;
	}
	
	public MapTerritory getMapTerritory()
	{
		return ourMapTerritory;
	}
	
	public int getID()
	{
		return ID;
	}
	
	public String getName()
	{
		return Name;
	}
	
	public void addUnit(Unit joe)
	{
		Units.Add(joe);		
	}
	
	public void removeUnit(Unit joe)
	{
		Units.Remove(joe);
	}
	
	public void draw(Gdk.Window win, Gdk.Color terr, Gdk.Color textcolor, Pango.Context pango_context)
	{
	   	ourMapTerritory.draw(win, terr, textcolor);

	   	for (int offset=0; offset < Units.Count && offset < 3; offset++)
	   	{
	   		((TacticalUnit)Units[offset]).draw(win, offset*5);
	   	}
	   	if (Units.Count > 1)
	   	{
	   		Gdk.GC textcoloring = new Gdk.GC(win);
	   		textcoloring.RgbFgColor = textcolor;
	   		
   	        Pango.Layout label = new Pango.Layout (pango_context);
			label.Wrap = Pango.WrapMode.Word;
			label.FontDescription = FontDescription.FromString ("Tahoma 8");
			label.SetMarkup ( Units.Count.ToString() );
			
			int szX, szY;
			label.GetPixelSize(out szX, out szY);

			/*Redraw*/			
//			win.InvalidateRect(new Gdk.Rectangle(ourMapTerritory.centerX, ourMapTerritory.centerY, szX, szY), true);
			
			win.DrawLayout (textcoloring, ourMapTerritory.centerX, ourMapTerritory.centerY, label);
	   	}

	}
	

}
}
