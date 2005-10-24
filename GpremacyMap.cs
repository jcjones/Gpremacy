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
        ArrayList territories;
        Gdk.Pixbuf bgimagePixbuf;
        GraphicsStorage store;
        Game game;
        Gdk.Region region;
 
        public GpremacyMap (Game igame)
        {
        	game = igame;
			territories = new ArrayList();
			this.Realized += OnRealized;
			this.ExposeEvent += OnExposed;
			LoadCountryBoundaries();
			game.LoadResourceCards(territories);

            store = GraphicsStorage.GetInstance();    

			Gdk.Point[] borders = new Point[4];
			borders[0]=new Point(0,0);
			borders[1]=new Point(store.Map.Width,0);
			borders[2]=new Point(store.Map.Width,store.Map.Height);
			borders[3]=new Point(0,store.Map.Height);
			region = Gdk.Region.Polygon(borders, FillRule.WindingRule);                        

			this.SetSizeRequest(store.Map.Width, store.Map.Height);
			

        }
   
        void OnExposed (object o, ExposeEventArgs args)
        {
			//System.Console.WriteLine("In GpremacyMap:OnExposed");        

			this.GdkWindow.DrawPixbuf(this.Style.WhiteGC, store.Map, 0, 0, 0, 0, store.Map.Width, store.Map.Height, RgbDither.Normal, 1, 1);
			//this.GdkWindow.DrawLayout(this.Style.TextGC (StateType.Normal), 100, 150, layout);                
			
			
			foreach(Territory item in territories)
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
        	Player owner;
        	int ownerid, x, y, id = 0;
        	ArrayList points = new ArrayList(); // of Gdk.Point
        	ArrayList graphConnections = new ArrayList(); // of ArrayList of String.
        	ArrayList subGraph; // of String
        	bool isLand;
        	Territory lastTerritory = null;
        	
        	try {
	        	System.IO.StreamReader input = new System.IO.StreamReader(SupportFileLoader.locateGameFile("countries.csv"));
		       	do {
		       		line = input.ReadLine();
	       			if ((line == null) || (line.Length > 0 && line[0]=='#')) continue;
		       		
	       			// Format: Name,   Owner#, [x, y]*
	       			//         string  int      int,int
	       			// Format: \t%ConnectingName, ConnectingName*
					//		      string          string
	       			if (line.IndexOf(",") <= 0)
	       				continue;
	       			if (line.IndexOf("%") <= 0) 
					{
	       				String[] parts;
		       			parts = line.Split(',');
	       				/*for (int i=0;i<parts.Length;i++) 
	       					System.Console.Write("[" + parts[i] + "],");
	       				System.Console.WriteLine(); */
	       				
		       			if (parts.Length > 2) 
	       				{
		       				// Extract data
	       					name = parts[0].Trim('"');
	       					ownerid = Int16.Parse(parts[1]);
		        			if (ownerid > 0 && ownerid <= game.Players.Count)
		        				owner = (Player)(game.Players[ownerid-1]);
		        			else
		        				owner = game.PlayerNobody;
		        			isLand = Boolean.Parse(parts[2]);
		        			// Get points
		        			for (int i=3; i<parts.Length-1; i=i+2)
		        			{
		        				x = Int16.Parse(parts[i]); 
		        				y = Int16.Parse(parts[i+1]); 
		        				points.Add(new Gdk.Point(x,y));
		        			} 
							lastTerritory = new Territory(id++, name, ownerid, owner, isLand, points, this.PangoContext);
							territories.Add(lastTerritory);
                        	points.Clear();
	       				}
	       			} else {
	       				/* Read in Graph*/	       				
	       				String[] parts;
		       			parts = line.Substring(line.IndexOf("%")+1).Split(',');
		       			subGraph = new ArrayList();
		       			subGraph.Add(lastTerritory);
		       			for (int i=0; i<parts.Length; i++)
		       				subGraph.Add(parts[i]);
		       			graphConnections.Add(subGraph);
	       			}
	       		} while (input.Peek() > -1);
	       		input.Close();
	       		
       		} catch ( System.IO.FileNotFoundException e ) {
        		game.HaltGame("Couldn't open countries.csv. Game halts.");    		       			
       		}
       		
       		/* Take care of connection graphs */       		
       		Territory home, targetTerritory;
       		String target;
       		bool noLand;
       		foreach (ArrayList graph in graphConnections) {
       			noLand = true;
       			home = (Territory)graph[0];
       			
       			for (int i=1; i<graph.Count; i++) {
       				target=(String)graph[i];
       				targetTerritory = getTerritoryByName(target);
       				if (targetTerritory.IsLand)
       					noLand = false;
       				home.MapTerritory.addConnection(targetTerritory);
       			}
       			
       			if (noLand)
       				home.MapTerritory.deepSea = true;
       		}
        }
        
        public Gdk.Region Region
        {
        	get { return region; }
        }
        
        public Territory getTerritoryByName(String name)
        {
        	foreach (Territory t in territories)
        		if (t.Name == name)
        			return t;
        	throw new Exception("Could not find the territory by name of " + name +". Check your countries.csv file; maybe you mispelled it?");
        }        
        
        public ArrayList Territories
        {
        	get { return territories; }
        }

        /* Graph Functions */
        /* This is the Bellman-Ford All-Destinations Shortest Path Algorithm */
       	public void shortestPaths(Territory startPoint, 
       								out int[] connectionDistances, 
							       	out int[] connectionWayPoints)
		{
			connectionDistances = new int[territories.Count];
			connectionWayPoints = new int[territories.Count];
			Territory current;
			
			/* Initialize Graph */
			foreach(Territory t in territories)
			{
				if (t == startPoint)
					connectionDistances[t.ID] = 0;
				else
					connectionDistances[t.ID] = Int32.MaxValue-100;
				connectionWayPoints[t.ID] = -1;
			}
/*			System.Console.WriteLine("PRE- Distances From " + startPoint.Name + " -");
			foreach(Territory edge in startPoint.MapTerritory.Connectedterritories)
				if (edge.ID > 0)
					System.Console.WriteLine("To " + edge.Name + " is " +  connectionDistances[edge.ID] +" through " + ((connectionWayPoints[edge.ID]>=0)?((Territory)territories[connectionWayPoints[edge.ID]]).Name:"Nowhere"));
*/							
/*  
   // Step 2: relax edges repeatedly
   for i from 1 to size(vertices):       
       for each edge uv in edges:
           u := uv.source
           v := uv.destination             // uv is the edge from u to v
           if v.distance > u.distance + uv.weight
               v.distance := u.distance + uv.weight
               v.predecessor := u
*/
			for (int i=0; i<territories.Count; i++)
			{
				//Console.WriteLine("Checking Terr " + ((Territory)territories[i]).Name);
				foreach(Territory edge in ((Territory)territories[i]).MapTerritory.ConnectedTerritories)
				{
					//Console.WriteLine("  Checking Edge " + edge.Name);
					if (connectionDistances[edge.ID] > connectionDistances[i]+1)
					{
						//Console.WriteLine("    Changing Edge " + edge.Name + " to come from " + ((Territory)territories[i]).Name);
						connectionDistances[edge.ID] = connectionDistances[i]+1;
						connectionWayPoints[edge.ID] = i;
						//Console.WriteLine("    UEDONE");
					}
					//Console.WriteLine("  EDONE");
				}
				//Console.WriteLine("TDONE");
			}		
			/* Every distance of a ConnectedTerritory must be 1, so uncomment this to error check */
			/*
			System.Console.WriteLine("- Distances From " + startPoint.Name + " -");
			foreach(Territory edge in startPoint.MapTerritory.Connectedterritories)
				if (edge.ID > 0)
					System.Console.WriteLine("To " + edge.Name + " is " +  connectionDistances[edge.ID] +" through " + ((connectionWayPoints[edge.ID]>=0)?((Territory)territories[connectionWayPoints[edge.ID]]).Name:"Nowhere"));			
			*/
		}
		
		public void updatePathsFrom(Territory A)
		{
			int[] dist, way;
			shortestPaths(A, out dist, out way);
			A.MapTerritory.ConnectionDistances = dist; 
			A.MapTerritory.ConnectionWayPoints = way;
		}		

        public int distanceBetween(Territory a, Territory b)
        {
        	if (a.MapTerritory.ConnectionDistances == null)
				updatePathsFrom(a);
        	return a.MapTerritory.ConnectionDistances[b.ID];
        }
        
        public int distanceFromClosestHomeTerritory(Territory a, Player p)
        {
        	int shortest = Int32.MaxValue;
        	foreach(Territory terr in territories)
        	{
        		if (terr.OriginalOwner != p.CountryID || terr == a)
        			continue;
        			
        		int tmp = distanceBetween(a, terr);
        		System.Console.WriteLine("Dist btwn " + terr.Name + " and " + a.Name + " = " + tmp);
        		
        		if (tmp < shortest)
        			shortest = tmp;
        	}
        	System.Console.WriteLine("Closest terr to " + a.Name + " is " + shortest);
        	return shortest;
        }
        
        /* Pretty Stuff */
        
        public void drawArrow(Point a, Point b)
        {
        	this.GdkWindow.DrawLine (this.Style.BlackGC, a.X, a.Y, b.X, b.Y);
        }
}
}
