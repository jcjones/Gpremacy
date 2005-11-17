using System;
using Gtk;
using Glade;
using Pango;
using Gdk;
using System.Collections;
using System.Drawing.Drawing2D;

namespace Gpremacy{

class GpremacyMap : DrawingArea
{
        Pango.Layout layout;
        ArrayList territories;
        GraphicsStorage store;
        Gdk.Region region;
        
        public bool ArrowOn;
        public Territory ArrowFromTerritory;
        public Territory ArrowToTerritory;
 
        public GpremacyMap (ArrayList terrs)
        {
			territories = terrs; // loaded by Game
			
			this.Realized += OnRealized;
			this.ExposeEvent += OnExposed;

            store = GraphicsStorage.GetInstance();    

			Gdk.Point[] borders = new Gdk.Point[4];
			borders[0]=new Gdk.Point(0,0);
			borders[1]=new Gdk.Point(store.Map.Width,0);
			borders[2]=new Gdk.Point(store.Map.Width,store.Map.Height);
			borders[3]=new Gdk.Point(0,store.Map.Height);
			region = Gdk.Region.Polygon(borders, Gdk.FillRule.WindingRule);                        

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
			
			if (ArrowOn)
			{
				System.Console.WriteLine("Arrow On from " + ArrowFromTerritory.Name + " " + ArrowToTerritory.Name);
				this.drawArrow(ArrowFromTerritory, ArrowToTerritory);
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
        
        public void ForceRedraw()
        {
        	System.Console.WriteLine("Clearscreen");
        	this.GdkWindow.InvalidateRegion(Region, true);
        }
         
        public Gdk.Region Region
        {
        	get { return region; }
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
			//Territory current;
			
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
        	Territory localTerr = Game.GetInstance().TerritoryByName(a.Name);        	
        	if (localTerr.MapTerritory.ConnectionDistances == null)
				updatePathsFrom(localTerr);
        	return localTerr.MapTerritory.ConnectionDistances[b.ID];
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
        
        public void drawArrow(Territory start, Territory end)
        {
			/* in the upward orientation
			 * ox+10, oy
			 * ox+10, ey+35
			 * ox+35, ey+35
			 * ox, ey
			 * ox-35, ey+35
			 * ox-10, ey+35
			 * ox-10, oy
			 */			
			if (start == end)
				return;
				
			int aX = start.MapTerritory.centerX;
			int aY = start.MapTerritory.centerY;
			int bX = end.MapTerritory.centerX;
			int bY = end.MapTerritory.centerY;
			 
			double RAD_TO_DEG = 57.29578;
        	float ox, oy;
 			Matrix X = new Matrix();
			Gdk.GC ArrowFill = new Gdk.GC(this.GdkWindow);
		   	ArrowFill.RgbFgColor = Game.GetInstance().State.CurrentPlayer.Color;
		   				        	
        	ox = aX; oy = aY; 
        	double dist = System.Math.Sqrt( ( aX-bX )*( aX-bX ) + ( aY-bY )*( aY-bY ) );
        	
        	if (aY-bY == 0)
        		aY++;
        		
        	double angle = System.Math.Atan2( ( aY-bY ) , ( aX-bX ) ) * RAD_TO_DEG;
			X.RotateAt((float)angle+90.0f, new System.Drawing.PointF(ox, oy));
			X.Translate(ox, oy);
        	X.Scale(1.0f, (float)(dist/100.0));
			
        	System.Drawing.Point[] points = new System.Drawing.Point[8];
        	        	
       		points[0] = new System.Drawing.Point(10, 0);
       		points[1] = new System.Drawing.Point(10, 100-35);
       		points[2] = new System.Drawing.Point(35, 100-35);
       		points[3] = new System.Drawing.Point(0, 100);
       		points[4] = new System.Drawing.Point(-35, 100-35);
       		points[5] = new System.Drawing.Point(-10, 100-35);
       		points[6] = new System.Drawing.Point(-10, 0);
       		points[7] = new System.Drawing.Point(10, 0);
        	
        	X.TransformPoints(points);
        	
        	Gdk.Point[] gpoints = new Gdk.Point[points.Length];
        	for(int i=0; i<points.Length; i++)
        		gpoints[i] = new Gdk.Point(points[i].X,points[i].Y); 
        	
        	this.GdkWindow.DrawPolygon (ArrowFill, true, gpoints);
        	this.GdkWindow.DrawPolygon (this.Style.BlackGC, false, gpoints);        	
        }
        
}
}
