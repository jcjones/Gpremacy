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
	public Point[] borders;
	public Region region;
	Pango.Layout label;
	public int centerX, centerY;
	int labelX, labelY;

	public MapTerritory (String name, bool land_i, ArrayList borders_i, Pango.Context pango_context)
	{
		int count = borders_i.Count/2;
		isLand = land_i;
		borders = new Point[count];
		
	   	int x1,y1,x2=-1,y2=-1; 
	   	int j=0;
	   	for (int i=0; j < count*2; i++) 
		{
  			borders[i]=new Gdk.Point();
  			borders[i].X=(Int16)borders_i[j++];
  			borders[i].Y=(Int16)borders_i[j++];
  			//System.Console.WriteLine(i + ":"+ count +  " - " + borders[i].X + "," + borders[i].Y);
	   	}
	   	/* Make region */
	   	region = Gdk.Region.Polygon(borders, FillRule.WindingRule);
	   	
	   	/* Make label */
	   	
        label = new Pango.Layout (pango_context);
		label.Wrap = Pango.WrapMode.Word;
		label.FontDescription = FontDescription.FromString ("Tahoma 9");
		label.SetMarkup (name);
		
		/* Find the center of the border for label & overall center */
		int totalX = 0, totalY = 0;

		foreach (Point pt in borders) 
		{
			totalX += pt.X;
			totalY += pt.Y;
		}
		centerX = (totalX / count);
		centerY = (totalY / count);
		
		int szX, szY;
		label.GetPixelSize(out szX, out szY);
		labelX = centerX - (szX/2);
		labelY = centerY - (szY/2);						
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
	   	
	   	int minx = borders[0].X , miny = borders[0].Y, maxx = 0, maxy = 0, x, y;	   	
	   	Point[] translated_borders = new Point[borders.Length];
	   	
	   	/* Determine bounds */
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		if (minx > borders[i].X) minx = borders[i].X;
	   		if (miny > borders[i].Y) miny = borders[i].Y;	   		
	   		System.Console.WriteLine("(x,y)=("+borders[i].X+","+borders[i].Y+")");
	   	}

	   	/* Translate */ 	   	
	   	int tx = minx-ox, ty = miny-oy;
	   	System.Console.WriteLine("Tx, ty="+tx+","+ty);	 
	   		   	
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		x = translated_borders[i].X-tx;
	   		y = translated_borders[i].Y-ty;
	   			   		
	   		System.Console.WriteLine("(x,y)=("+x+","+y+")");
	   		translated_borders[i] = new Point(x,y);
	   		
	   		if (x > maxx) maxx = x;
	   		if (y > maxy) maxy = y;	   			   		   		
	   	}	   	

	   	/* Scale */
	   	double scaleX = (double)(szx)/maxx;
	   	double scaleY = (double)(szy)/maxy;
	   	System.Console.WriteLine("Sx, Sy="+scaleX+","+scaleY+" MaxX,MaxY="+maxx+","+maxy);	 
	   	
	   	for(int i=0; i<borders.Length; i++)
	   	{
	   		x = (int)(translated_borders[i].X*scaleX);
	   		y = (int)(translated_borders[i].Y*scaleY);
	   			   		
	   		System.Console.WriteLine("(x,y)=("+x+","+y+")");
	   		translated_borders[i] = new Point(x,y);

	   	}
	   	  	
		System.Console.WriteLine("Drawing: ");
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
	
}
}