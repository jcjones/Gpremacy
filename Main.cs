// project created on 07/05/2005 at 20:05
using System;
using Gtk;
using Glade;
using Pango;
using Gdk;

namespace Gpremacy {
class GpremacyGUI {
	GpremacyMap MapArea;

	[Glade.Widget] Gtk.Viewport MapViewport;
	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.ScrolledWindow MapScrolledWindow2;

	public static void Main(string[] args)
	{
		GpremacyGUI mainGUI = new GpremacyGUI();
		mainGUI.init();
	}
	
	public void init() {
		Application.Init ();
		MapArea = new GpremacyMap();
		System.Console.WriteLine("Got Maparea:" + MapArea + ".");		
		
		Glade.XML gxml = new Glade.XML ("/home/pug/src/gpremacy/gpremacy_gui/gpremacy_gui.glade", "MainWindow", null);
		gxml.Autoconnect (this);
		
		System.Console.WriteLine("Adding..." + MapArea + " to " + MapViewport + "!");
		MapViewport.Add(MapArea);
		
		System.Console.WriteLine("Added.");
		MainWindow.Resize(800,600);
		MapArea.ShowAll();
						
		MainWindow.ButtonPressEvent += OnButtonPress;
		
				
		Application.Run ();
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
   		double x = args.Event.X+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y+MapScrolledWindow2.Vadjustment.Value;	   	
	   	
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
	   	
   		System.Console.WriteLine("Clicked " + target.getName());
   		if (args.Event.Button == 3)
   		{
   			if (target.getMapTerritory().isLand)
   				target.addUnit(new Army(new Player(), target));
   			else
   				target.addUnit(new Navy(new Player(), target));   			
   		}
   		
   		/* Redraw that region */
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
	public void on_MainWindow_delete_event(System.Object o, DeleteEventArgs args)
	{
		Application.Quit();
		args.RetVal = true;
	}
	
}
}
