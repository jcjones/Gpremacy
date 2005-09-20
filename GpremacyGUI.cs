// project created on 07/05/2005 at 20:05
using System;
using Gtk;
using Glade;
using Pango;
using Gdk;

namespace Gpremacy {
class GpremacyGUI {
	GpremacyMap MapArea;
	Game game;
	Gdk.Region invalRegion;

	[Glade.Widget] Gtk.Viewport MapViewport;
	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.ScrolledWindow MapScrolledWindow2;
	
	[Glade.Widget] Gtk.TextView OrderOfPlayTextBox;	
	[Glade.Widget] Gtk.TextView ResourcesTextBox;
	[Glade.Widget] Gtk.TextView WorldMarketTextBox;
	[Glade.Widget] Gtk.TextView LogTextBox;
	[Glade.Widget] Gtk.TextView StatsTextBox;
	[Glade.Widget] Gtk.TextView MiscTextBox;
	
	public GpremacyGUI(Game i)
	{
		game = i;
		
	}
	
	public void init() 
	{
		Application.Init ();
		MapArea = new GpremacyMap(game);
		System.Console.WriteLine("Got Maparea:" + MapArea + ".");		
				
		Glade.XML gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "MainWindow", null);
		gxml.Autoconnect (this);
		
		System.Console.WriteLine("Adding..." + MapArea + " to " + MapViewport + "!");
		MapViewport.Add(MapArea);
		
		System.Console.WriteLine("Added.");
		MainWindow.Resize(800,600);
		MapArea.ShowAll();
						
		
		// Configure Mouse events
		MapArea.AddEvents((int)EventMask.PointerMotionMask);
		MapArea.AddEvents((int)EventMask.ButtonPressMask);
		
		MapArea.ButtonPressEvent += OnButtonPress;
		MapArea.MotionNotifyEvent += OnMotion;		

		updateGUIStatusBoxes();				
												
		Application.Run ();
	}
	
   public void OnMotion (object o, MotionNotifyEventArgs args)
   {
      	double x = args.Event.X;//+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y;//+MapScrolledWindow2.Vadjustment.Value;

   		/* Find territory */
	   	Territory target = null;   	
	   	
	   	foreach (Territory here in MapArea.getTerritories())
	   	{
	   		if ( here.getMapTerritory().checkClick(x,y) )
	   		{
	   			target = here;
	   			break;
	   		}
	   	}
	   	//System.Console.WriteLine("(X,Y)=" + x + "," + y + " = (x,y)+(xo,yo) = " + args.Event.X + "," + args.Event.Y + " + " + MapScrolledWindow2.Hadjustment.Value + ","  + MapScrolledWindow2.Vadjustment.Value);	   
   			   		   	
   		game.State.mouseMotion(x, y, target);   		
   }
   
   public void OnButtonPress (object o, ButtonPressEventArgs args)
   {
	   /* Print Coordinates in countries.csv form*/
	   /*
	   	if (args.Event.Button > 1) {
	   		System.Console.WriteLine("");
	   		System.Console.Write("X,0,false,");
	   	} else
	   		System.Console.Write((args.Event.X-4) + "," + (args.Event.Y-4) + ",");
	   */

	   	/* Find territory */
	   	Territory target = null;
      	double x = args.Event.X;//+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y;//+MapScrolledWindow2.Vadjustment.Value;
	   	
	   	foreach (Territory here in MapArea.getTerritories())
	   	{
	   		if ( here.getMapTerritory().checkClick(x,y) )
	   		{
	   			target = here;
	   			break;
	   		}
	   	}
	   	/* Do nothing if no target */
	   	if (target == null)
	   		return;
	   		
	   	game.State.mouseClick(target, args.Event.Button);
   		
   		/* Redraw that region */
	   	redrawTerritory(target);
	   	
	}
	
	public void redrawTerritory(Territory target)
	{
		MapArea.GdkWindow.InvalidateRegion(target.getMapTerritory().region,true);	
	}
	
	public void on_new1_activate(System.Object obj, EventArgs e) 
	{}
	public void on_open1_activate(System.Object obj, EventArgs e) 
	{}
	public void on_save1_activate(System.Object obj, EventArgs e) 
	{}
	public void on_save_as1_activate(System.Object obj, EventArgs e) 
	{}
	public void on_quit1_activate(System.Object obj, EventArgs e) 
	{
		Application.Quit();
	}
	public void on_research1_activate(System.Object obj, EventArgs e) 
	{}
	public void on_about1_activate(System.Object obj, EventArgs e) 
	{
			string[] authors = new string[]{
				"J.C. \"Pug\" Jones <jcjones@ufl.edu>",
			};

			string[] documenters = new string[]{
				"J.C. \"Pug\" Jones <jcjones@ufl.edu>",
			};
			
/*			Gnome.About about = new Gnome.About ("GPremacy", VERSION,
					"(C) 2005 J.C. \"Pug\" Jones",
					"An implementation of the 1984 board game Supremacy.",
					authors, documenters, null,
					null);
			
			((Gtk.Window ) about).Icon = PixbufUtils.LoadFromAssembly ("about.png");
			about.Show ();
*/
	}
	public void on_always_march1_activate(System.Object obj, EventArgs e)
	{
	}
	public void on_MainWindow_delete_event(System.Object o, DeleteEventArgs args)
	{
		Application.Quit();
		args.RetVal = true;
	}
	
	public void on_endTurnButton_pressed(System.Object obj, EventArgs e)
	{
		//game.State.nextPlayer();
		game.State.nextState();
		updateGUIStatusBoxes();	
	}
	
	public void updateGUIStatusBoxes()
	{
		writeToOrderOfPlayTextBox("Current Player:\n" + game.State.CurrentPlayer.toString() + "\nCurrent State:\n" + game.State.StateIDName);								
		writeToResourcesTextBox(game.State.CurrentPlayer.toStringResources());
		writeToWorldMarketTextBox(game.Market.toString());	
	}
	
	public void writeToOrderOfPlayTextBox(String a) 
	{
		OrderOfPlayTextBox.Buffer.Text = a;
	}
	
	public void writeToResourcesTextBox(String a) 
	{
		ResourcesTextBox.Buffer.Text = a;
	}
	
	public void writeToWorldMarketTextBox(String a) 
	{
		WorldMarketTextBox.Buffer.Text = a;
	}
	
	public void writeToLog(String a) 
	{			
		// Write such that newest entries are on top
		LogTextBox.Buffer.Text = a + "\n" + LogTextBox.Buffer.Text;
	}	
	
	public void clearArrow()
	{
		if (invalRegion != null)
		{
			System.Console.WriteLine("Invalidating..." + invalRegion.Clipbox.X + " " + invalRegion.Clipbox.Y + " " + invalRegion.Clipbox.Height +  " " + invalRegion.Clipbox.Width);
    		MapArea.GdkWindow.InvalidateRegion(invalRegion,true);
    		invalRegion.Destroy();
    	}    		
	}
	
	public void drawArrow(Territory a, Territory b)
    {
		clearArrow();

		int x = a.getMapTerritory().centerX;
		int y = a.getMapTerritory().centerY;
		int w = b.getMapTerritory().centerX - x;
		int h = b.getMapTerritory().centerY - y;

		/* This point stuff is necessary since Gdk.Region.Rectangle() doesn't work! */
		Point[] arrow = new Point[4];
		arrow[0]=new Point(a.getMapTerritory().centerX, a.getMapTerritory().centerY);
		arrow[1]=new Point(a.getMapTerritory().centerX, b.getMapTerritory().centerY);
		arrow[2]=new Point(b.getMapTerritory().centerX, b.getMapTerritory().centerY);
		arrow[3]=new Point(b.getMapTerritory().centerX, a.getMapTerritory().centerY);

						
		/* Uncool arrow */						
       	MapArea.drawArrow(arrow[0], arrow[2]);
       	/* Cool arrow */
	       	
	   	invalRegion = Gdk.Region.Polygon(arrow, FillRule.WindingRule);
    }
	
}
}
