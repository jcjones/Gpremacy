// created on 08/27/2005 at 09:37
using System;
using Gtk;
using Glade;
using Pango;
using Gdk;
using System.Collections;

namespace Gpremacy{

class MapTerritory
{
	public bool isLand;
	public bool deepSea;
	public Point[] borders;
	public Region region;
	Pango.Layout label;
	public int centerX, centerY;
	//int labelX, labelY;
	ArrayList connectedTerritories;
	int[] connectionDistances;
	int[] connectionWayPoints;

	public MapTerritory () 
	{
	}

	public MapTerritory (String name, bool land_i, ArrayList borders_i, Pango.Context pango_context)
	{
		deepSea = false;
		isLand = land_i;
		borders = new Point[borders_i.Count];
		connectedTerritories = new ArrayList();
		
	   	//int x1,y1,x2=-1,y2=-1; 
		int totalX = 0, totalY = 0;
	   	
	   	for (int i=0; i < borders_i.Count; i++) 
		{
  			borders[i]=(Gdk.Point)borders_i[i];
	   		totalX += borders[i].X;
			totalY += borders[i].Y;
  			//System.Console.WriteLine(i + ":"+ borders_i.Count +  " - " + borders[i].X + "," + borders[i].Y + "RT:("+totalX+","+totalY+")");
		}
		
		/* Find the center of the border */		
		centerX = (totalX / borders_i.Count);
		centerY = (totalY / borders_i.Count);
		//System.Console.WriteLine("Center of " + name + " is " + centerX+","+centerY);
		
	   	/* Make region */
	   	region = Gdk.Region.Polygon(borders, FillRule.WindingRule);
	   	
	   	/* Make label */	   	
        label = new Pango.Layout (pango_context);
		label.Wrap = Pango.WrapMode.Word;
		label.Alignment = Pango.Alignment.Center;
		label.FontDescription = FontDescription.FromString ("Tahoma 9");
		label.SetMarkup (name);		
				
		//int szX, szY;
		//label.GetPixelSize(out szX, out szY);
		//labelX = centerX - (szX/2);
		//labelY = centerY - (szY/2);						
	}
		
	public void draw(Gdk.Window win, int ox, int oy, int szx, int szy, Gdk.Color terr)
	{
		/* This function allows drawing the territory at a
		 * predefined x,y coordinate, which requires we
		 * translate the entirety of the polygon to begin at
		 * that coordinate, an O(3n) operation (could be O(2n)) 
		*/
	   	Gdk.GC field = new Gdk.GC(win);
	   	field.RgbFgColor = terr;
	   	
	   	int minx = borders[0].X , miny = borders[0].Y, maxx = 0, maxy = 0, x = 0 , y = 0;	   	
	   	Point[] translated_borders = new Point[borders.Length];
	   	
	   	/* Determine bounds */
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		if (minx > borders[i].X) minx = borders[i].X;
	   		if (miny > borders[i].Y) miny = borders[i].Y;
	   		
	   		if (borders[i].X > maxx) maxx = borders[i].X;
	   		if (borders[i].Y > maxy) maxy = borders[i].Y;	   
	   		//System.Console.WriteLine("(x,y)=("+borders[i].X+","+borders[i].Y+")");
	   	}

	   	/* Translate */ 	   	
	   	int tx = minx-ox, ty = miny-oy;
	   	System.Console.WriteLine("Tx, ty="+tx+","+ty+" MinX,MinY="+minx+","+miny+" MaxX,MaxY="+maxx+","+maxy);	 
	   		   	
	   	maxx = maxy = 0;
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		x = borders[i].X-tx;
	   		y = borders[i].Y-ty;
	   			   		
	   		if (x > maxx) maxx = x;
	   		if (y > maxy) maxy = y;	   			   		   		

	   		translated_borders[i] = new Point(x,y);
	   	}	   	

	   	/* Scale uniformly */
	   	double scaleX = (double)(szx)/maxx;
	   	double scaleY = (double)(szy)/maxy;

	   	System.Console.WriteLine("Sx, Sy="+scaleX+","+scaleY+" MaxX,MaxY="+maxx+","+maxy);	 
	   	
	   	if (scaleX > scaleY)
	   		scaleX = scaleY;
		else
			scaleY = scaleX;			   
	   	
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		x = (int)(translated_borders[i].X*scaleX);
	   		y = (int)(translated_borders[i].Y*scaleY);

			if (true)	   			   		
	   			System.Console.WriteLine(i+":(x,y)=("+x+","+y+")");
	   		translated_borders[i] = new Point(x,y);

	   	}
	   	  	
		System.Console.WriteLine("End Sample x: " + x+ " y:" + y);
		
		/* Draw */
		win.DrawPolygon(field, true, translated_borders);
	}	
	
	public void draw(Gdk.Window win, Gdk.Color terr, Gdk.Color textcolor) 
	{   
		
	   	Gdk.GC field = new Gdk.GC(win);
	   	Gdk.GC text = new Gdk.GC(win);
	   	field.RgbFgColor = terr;
	   	// Add transparency, somehow...
	   	text.RgbFgColor = textcolor;
	   	
		//win.DrawPolygon(field, false, borders);
		win.DrawLayout (text, centerX, centerY, label);
	}
	
	public bool checkClick(double x, double y) 
	{		
		return region.PointIn((int)x,(int)y);
	}
	
	public bool IsLand
	{
		get { return isLand; }
	}
	
	public void addConnection(Territory a)
	{
		connectedTerritories.Add(a);
	}
	
	public int[] ConnectionDistances
	{ 
		get { return connectionDistances; }
		set { connectionDistances = value; }
	}
	
	public int[] ConnectionWayPoints
	{ 
		get { return connectionWayPoints; }
		set { connectionWayPoints = value; }
	}	
	
	public ArrayList ConnectedTerritories
	{
		get { return connectedTerritories; }
	}

	
}
}