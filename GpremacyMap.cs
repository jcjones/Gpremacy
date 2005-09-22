using System;
using Gtk;
using Glade;
using Pango;
using Gdk;
using System.Collections;

namespace Gpremacy{

class GpremacyMap : DrawingArea
{
        Pango.Layout layout;
        ArrayList Territories;
        Gdk.Pixbuf bgimagePixbuf;
        GraphicsStorage store;
        Game game;
 
        public GpremacyMap (Game igame)
        {
        	game = igame;
			Territories = new ArrayList();
			this.Realized += OnRealized;
			this.ExposeEvent += OnExposed;
			LoadCountryBoundaries();
			game.LoadResourceCards(Territories);
			game.DistributeResourceCards(); // Should not be in this file..      
            
            store = GraphicsStorage.GetInstance();    
                        

			this.SetSizeRequest(store.Map.Width, store.Map.Height);
			

        }
   
        void OnExposed (object o, ExposeEventArgs args)
        {
			//System.Console.WriteLine("In GpremacyMap:OnExposed");        

			this.GdkWindow.DrawPixbuf(this.Style.WhiteGC, store.Map, 0, 0, 0, 0, store.Map.Width, store.Map.Height, RgbDither.Normal, 1, 1);
			//this.GdkWindow.DrawLayout(this.Style.TextGC (StateType.Normal), 100, 150, layout);                
			
			
			foreach(Territory item in Territories)
			{
				Gdk.Color owner = new Gdk.Color ((byte)255, (byte)64, (byte)64);
				Gdk.Color text;
				
				if (item.MapTerritory.isLand)
					text = new Gdk.Color ((byte)255, (byte)255, (byte)255);
				else
					text = new Gdk.Color ((byte)32, (byte)32, (byte)32);
					
				item.draw(this.GdkWindow, owner, text, this.PangoContext);
			}
           
        }
 
        void OnRealized (object o, EventArgs args)
        {
        		System.Console.WriteLine("In GpremacyMap:OnRealized");
        		/* Setup Text */        
                layout = new Pango.Layout (this.PangoContext);
                layout.Wrap = Pango.WrapMode.Word;
                layout.FontDescription = FontDescription.FromString ("Tahoma 16");
                layout.SetMarkup ("Hello Pango.Layout");
                /* */
                
        }
 
        void OnWinDelete (object o, DeleteEventArgs args)
        {
                Application.Quit ();
        }
        
        void LoadCountryBoundaries ()
        {
        	String line;
        	String name;
        	bool land;
        	int id, cur, next, i, n = 10;
        	ArrayList points = new ArrayList();
        	Player owner;
        	
        	try {
        		
	       		System.IO.StreamReader input = new System.IO.StreamReader(SupportFileLoader.locateGameFile("countries.csv"));
	        	do {
	        		n=n+30;
	        		line = input.ReadLine();
	        		if ((line == null) || (line.Length > 0 && line[0]=='#')) continue;
	        		
	        		// Format: Name, Owner#, [x, y]*
	        		//System.Console.WriteLine("got [" + line + "] l=" + line.Length);
	        		if (line.IndexOf(",") > 0) {
	        			cur = 0; i=0;
	        			next = line.IndexOf(",");
	        			//System.Console.WriteLine("Now..." + cur);
	        			name = line.Substring(cur, (next-cur));
	        			
        				cur = next+1;
        				next = line.IndexOf(",", cur);
        				id = Int16.Parse(line.Substring(cur, (next-cur)));
        				
        				cur = next+1;
        				next = line.IndexOf(",", cur);
        				land = Boolean.Parse(line.Substring(cur, (next-cur)));
        				
	        				        			
	        			//System.Console.WriteLine("Name: " + name + "ID: " + id + "Land: " + land);
	        			
	        			while (next < line.Length ) {
	        				cur = next+1;
	        				next = line.IndexOf(",", cur);
	        				if (next < 0) next = line.Length;
	        				
	        				points.Add(Int16.Parse(line.Substring(cur, (next-cur))));
	        				i++;
	        			}

	        			if (id > 0 && id <= game.Players.Count)
	        				owner = (Player)(game.Players[id-1]);
	        			else
	        				owner = game.PlayerNobody;
	        				
	        			//System.Console.WriteLine("id: " + id + " " + game.Players.Count + " " + owner.Name);
	        				
	        			Territories.Add(new Territory(name, id, owner, land, points, this.PangoContext));
                        points.Clear();
	        		}
	        		        		
	       		} while (input.Peek() > -1);
	       		input.Close();
	       		
       		} catch ( System.IO.FileNotFoundException e ) {
        		System.Console.WriteLine("Couldn't open countries.csv.");        		       			
       		}
        }
        
        public ArrayList getTerritories()
        {
        	return Territories;
        }
        
        public void drawArrow(Point a, Point b)
        {
        	this.GdkWindow.DrawLine (this.Style.BlackGC, a.X, a.Y, b.X, b.Y);
        }
}
}
