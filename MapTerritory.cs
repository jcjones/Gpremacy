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
	
}
}