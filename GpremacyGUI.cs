// project created on 07/05/2005 at 20:05
using System;
using System.Collections;
using Gtk;
using Glade;
using Pango;
using Gdk;

namespace Gpremacy {
class GpremacyGUI {
	GpremacyMap MapArea;
	Game game;
	Gdk.Region invalRegion;

	/* Main Window*/
	[Glade.Widget] Gtk.Viewport MapViewport;
	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.ScrolledWindow MapScrolledWindow2;
		
	[Glade.Widget] Gtk.TextView OrderOfPlayTextBox;	
	[Glade.Widget] Gtk.TextView ResourcesTextBox;
	[Glade.Widget] Gtk.TextView WorldMarketTextBox;
	[Glade.Widget] Gtk.TextView LogTextBox;
	[Glade.Widget] Gtk.TextView StatsTextBox;
	[Glade.Widget] Gtk.TextView MiscTextBox;

	/* Naval Options */	
	[Glade.Widget] Gtk.Window LoadNavalOptions;
	[Glade.Widget] Gtk.DrawingArea UnitLegendNaval;
	[Glade.Widget] Gtk.Label LoadNavalOptionsCounter;
	[Glade.Widget] Gtk.Label LoadNavalOptionsTitle;
	[Glade.Widget] Gtk.Table LoadNavalOptionsTable;
		
	/* Buy/Sell Options */
	[Glade.Widget] Gtk.Window MarketBuySell;
	[Glade.Widget] Gtk.Label MineralsStock;
	[Glade.Widget] Gtk.Label MineralsCost;
	[Glade.Widget] Gtk.Label OilStock;
	[Glade.Widget] Gtk.Label OilCost;
	[Glade.Widget] Gtk.Label GrainStock;
	[Glade.Widget] Gtk.Label GrainCost;
	[Glade.Widget] Gtk.Label MarketTotalBox;
	[Glade.Widget] Gtk.HScrollbar MineralsScroll;
	[Glade.Widget] Gtk.HScrollbar OilScroll;
	[Glade.Widget] Gtk.HScrollbar GrainScroll;
	[Glade.Widget] Gtk.Button MarketBuySellOkay;
	
	/* Move Ground Options */
	[Glade.Widget] Gtk.Window MoveGroundOptions;
	[Glade.Widget] Gtk.Label MoveGroundLabel;
	[Glade.Widget] Gtk.Table MoveGroundTable;
	
	public GpremacyGUI(Game i)
	{
		game = i;
		
	}
	
	public void init() 
	{
		Application.Init ();
		MapArea = new GpremacyMap(game);
		System.Console.WriteLine("Got Maparea:" + MapArea + ".");		
				
		Glade.XML gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), null, null);
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
	
	public GpremacyMap Map
	{
		get { return MapArea; }
	}
	
   public void OnMotion (object o, MotionNotifyEventArgs args)
   {
      	double x = args.Event.X;//+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y;//+MapScrolledWindow2.Vadjustment.Value;

   		/* Find territory */
	   	Territory target = null;   	
	   	
	   	foreach (Territory here in MapArea.getTerritories())
	   	{
	   		if ( here.MapTerritory.checkClick(x,y) )
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

		/* Double-clicks are meaningless to Gpremacy */
	   	if ((args.Event.Type == EventType.TwoButtonPress) || (args.Event.Type == EventType.ThreeButtonPress))
	   		return;
	   		   	
	   	/* Find territory */	   	
	   	Territory target = null;
      	double x = args.Event.X;//+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y;//+MapScrolledWindow2.Vadjustment.Value;
	   	
	   	foreach (Territory here in MapArea.getTerritories())
	   	{
	   		if ( here.MapTerritory.checkClick(x,y) )
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
		MapArea.GdkWindow.InvalidateRegion(target.MapTerritory.region,true);	
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
	public void on_undo_activate(System.Object obj, EventArgs e) 
	{
		if (!game.State.Unexecute())
		{
			// TODO: Disable the menu item
			// figure out how to renable it first, doofus!
		}
	}
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

		int x = a.MapTerritory.centerX;
		int y = a.MapTerritory.centerY;
		int w = b.MapTerritory.centerX - x;
		int h = b.MapTerritory.centerY - y;

		/* This point stuff is necessary since Gdk.Region.Rectangle() doesn't work! */
		Point[] arrow = new Point[4];
		arrow[0]=new Point(a.MapTerritory.centerX, a.MapTerritory.centerY);
		arrow[1]=new Point(a.MapTerritory.centerX, b.MapTerritory.centerY);
		arrow[2]=new Point(b.MapTerritory.centerX, b.MapTerritory.centerY);
		arrow[3]=new Point(b.MapTerritory.centerX, a.MapTerritory.centerY);

						
		/* Uncool arrow */						
       	MapArea.drawArrow(arrow[0], arrow[2]);
       	/* Cool arrow */
       	//Art.Bpath topPath = new Art.Bpath();
       	//topPath.X1 = x;
       	//topPath.Y1 = y;
       	
	       	
	   	invalRegion = Gdk.Region.Polygon(arrow, FillRule.WindingRule);
    }

    public void ShowError(string error)
    {    
		MessageDialog dlg = new MessageDialog
     	(MainWindow, Gtk.DialogFlags.Modal,
     	Gtk.MessageType.Error, Gtk.ButtonsType.Ok,  error);     
        dlg.Run();
        dlg.Destroy();
    }
    
    /* Market Buy/Sell Options */
    public void showMarketBuySell(int sell)
    {
    	if (sell > 0) { /* Sell Stage */
	    	MineralsScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Minerals()));
	    	OilScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Oil()));
	    	GrainScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Grain()));
    		
    		MineralsScroll.ValueChanged -= MarketBuyProfitCalculation;
    		GrainScroll.ValueChanged -= MarketBuyProfitCalculation;
    		OilScroll.ValueChanged -= MarketBuyProfitCalculation;    		
    		MineralsScroll.ValueChanged += MarketSellProfitCalculation;
    		GrainScroll.ValueChanged += MarketSellProfitCalculation;
    		OilScroll.ValueChanged += MarketSellProfitCalculation;
    		
    		MarketSellProfitCalculation(null, null);
    		
    		MarketBuySellOkay.Clicked -= on_MarketBuyOkay_clicked;
    		MarketBuySellOkay.Clicked += on_MarketSellOkay_clicked;
    	} else {  /* Buy Stage */
	    	MineralsScroll.SetRange(0.0, 99.0);
	    	OilScroll.SetRange(0.0, 99.0);
	    	GrainScroll.SetRange(0.0, 99.0);

    		MineralsScroll.ValueChanged -= MarketSellProfitCalculation;
    		GrainScroll.ValueChanged -= MarketSellProfitCalculation;
    		OilScroll.ValueChanged -= MarketSellProfitCalculation;
    		MineralsScroll.ValueChanged += MarketBuyProfitCalculation;
    		GrainScroll.ValueChanged += MarketBuyProfitCalculation;
    		OilScroll.ValueChanged += MarketBuyProfitCalculation;
    		
    		MarketBuyProfitCalculation(null, null);
    		MarketBuySellOkay.Clicked -= on_MarketSellOkay_clicked;
    		MarketBuySellOkay.Clicked += on_MarketBuyOkay_clicked;
    	}
    	
    	MineralsScroll.Value = 0;
   		OilScroll.Value = 0;
   		GrainScroll.Value = 0;
    		
    	MineralsCost.Text = "@ $"+game.Market.getCommodityCost(new Minerals()).ToString()+" M";
    	OilCost.Text = "@ $"+game.Market.getCommodityCost(new Oil()).ToString()+" M";
    	GrainCost.Text =  "@ $"+game.Market.getCommodityCost(new Grain()).ToString()+" M";
    	
    	MarketBuySell.ShowAll();
    }
    
    public void MarketSellProfitCalculation(System.Object obj, EventArgs e)
    {
    	int profit = 0;
    	profit += ((int)OilScroll.Value)*game.Market.getCommodityCost(new Oil());
    	profit += ((int)GrainScroll.Value)*game.Market.getCommodityCost(new Grain());
    	profit += ((int)MineralsScroll.Value)*game.Market.getCommodityCost(new Minerals());
    	MarketTotalBox.Text = "Profit: $"+profit.ToString()+" M";
    	
    	MineralsStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Minerals())-((int)MineralsScroll.Value)).ToString();
    	OilStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Oil())-((int)OilScroll.Value)).ToString();
    	GrainStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Grain())-((int)GrainScroll.Value)).ToString();    	
    }
    
    public void MarketBuyProfitCalculation(System.Object obj, EventArgs e)
    {
    	int cost = 0;
    	cost += ((int)OilScroll.Value)*game.Market.getCommodityCost(new Oil());
    	cost += ((int)GrainScroll.Value)*game.Market.getCommodityCost(new Grain());
    	cost += ((int)MineralsScroll.Value)*game.Market.getCommodityCost(new Minerals());
    	MarketTotalBox.Text = "Cost: $"+cost.ToString()+" M";
    	
    	MineralsStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Minerals())+((int)MineralsScroll.Value)).ToString();
    	OilStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Oil())+((int)OilScroll.Value)).ToString();
    	GrainStock.Text = (game.State.CurrentPlayer.getStockpileAmount(new Grain())+((int)GrainScroll.Value)).ToString();    	
    }    
    
    public void on_MarketSellOkay_clicked(System.Object obj, EventArgs e)
	{
		ArrayList forSale = new ArrayList(); // of Stock
							
		forSale.Add(new Stock(new Oil(), -1*((int)OilScroll.Value)));
		forSale.Add(new Stock(new Minerals(), -1*((int)MineralsScroll.Value)));
		forSale.Add(new Stock(new Grain(), -1*((int)GrainScroll.Value)));
		
		Orig_Sell cmd = new Orig_Sell(forSale);
		game.State.Execute(cmd);			

		MarketBuySell.Hide();
	}
	
    public void on_MarketBuyOkay_clicked(System.Object obj, EventArgs e)
	{
		ArrayList ticket = new ArrayList(); // of Stock
							
		ticket.Add(new Stock(new Oil(), ((int)OilScroll.Value)));
		ticket.Add(new Stock(new Minerals(), ((int)MineralsScroll.Value)));
		ticket.Add(new Stock(new Grain(), ((int)GrainScroll.Value)));
		
		Orig_Buy cmd = new Orig_Buy(ticket);
		game.State.Execute(cmd);			

		MarketBuySell.Hide();
	}
		
	public void on_MarketBuySell_delete_event(System.Object obj, EventArgs e)
	{	
		MarketBuySell.Hide();
	}
	
	/* Naval Options */

	public void showLoadNavalOptions(Territory land, Territory sea)
	{
		uint n = 1;
		LoadNavalOptionsTitle.Text = "Land: " + land.Name + " Sea: " + sea.Name;
		LoadNavalOptionsCounter.Text = "Friendlies on shore: " + land.Friendlies(game.State.CurrentPlayer).Count.ToString();

		foreach (Gtk.Widget wid in LoadNavalOptionsTable)
		{
			/* Clean house */
			LoadNavalOptionsTable.Remove(wid);
		}
		
		foreach (Navy ship in sea.Friendlies(game.State.CurrentPlayer))
		{
			if (!ship.CanHoldTroops) continue;
			Gtk.Label label = new Gtk.Label("Ship " + n.ToString());
			Gtk.SpinButton spinbox = new Gtk.SpinButton(0.0,4.0,1.0);
			spinbox.SetIncrements(1.0,1.0);
			spinbox.Value = (double)ship.UnitsAboardCount;
			spinbox.ValueChanged += on_NavalSpinbuttonSpun;
			
			LoadNavalOptionsTable.Attach(label, 0, 1, n, n+1);
			LoadNavalOptionsTable.Attach(spinbox, 1, 2, n, n+1);
			n++;
		}
		
		LoadNavalOptions.ShowAll();
	}
	public void on_NavalSpinbuttonSpun(System.Object obj, EventArgs e)
	{
		ArrayList data = (ArrayList)game.State.CurrentState.Data;
		Territory port = ((Territory)data[0]);
		Territory sea = ((Territory)data[1]);		
				
		int sumOfUnitsAboard = 0;
		int sumOfSpinBoxes = 0;
		int unitsOnLand = port.Units.Count;
		int resultOnShore = 0;		
		
		foreach (Navy ship in sea.Units)
		{
			sumOfUnitsAboard += ship.UnitsAboardCount;
		}
		foreach (Gtk.Widget wid in LoadNavalOptionsTable)
		{
			if (wid is Gtk.SpinButton)
			{
				sumOfSpinBoxes += ((int)((Gtk.SpinButton)wid).Value);
				System.Console.WriteLine("SpinNow: " + sumOfSpinBoxes + " wid is " + wid);
			}
		}
		resultOnShore = (unitsOnLand + sumOfUnitsAboard) - sumOfSpinBoxes;
		System.Console.WriteLine("resOnShore:"+resultOnShore+" unOnLand:"+unitsOnLand+" sumAboard:"+sumOfUnitsAboard+" sumSpin:"+sumOfSpinBoxes);
		
		LoadNavalOptionsCounter.Text = "Friendlies on shore: " + resultOnShore;
	}	
	public void on_NavalOkay_clicked(System.Object obj, EventArgs e)
	{
		/* Resolve the reorderings needed to end
 		 * up with the appropriate number of 
		 * units on each ship and in port. */		 
		LoadNavalOptions.Hide();
			
		ArrayList troops = new ArrayList();
		ArrayList data = (ArrayList)game.State.CurrentState.Data;		
		Territory port = ((Territory)data[0]);
		Territory sea = ((Territory)data[1]);		
		ArrayList landUnits;
		ArrayList seaUnits = sea.Units;

		IEnumerator shipIt = seaUnits.GetEnumerator();
		shipIt.MoveNext();
		Navy ship = (Navy)shipIt.Current;
		
		int diff;		
		
		foreach (Widget wid in LoadNavalOptionsTable)
		{		
			if (! (wid is Gtk.SpinButton)) continue;
			/* Reload ground unit list */
			landUnits = port.Units;
			troops.Clear();

			while (!ship.CanHoldTroops) 
			{
				if (!shipIt.MoveNext())
					break; // should not happen, we'd be running out of ships
				ship = (Navy)shipIt.Current;
			}

			/* Determine what we're doing to this particular ship */			
			diff = ((int)((Gtk.SpinButton)wid).Value) - ship.UnitsAboardCount;
			
			/* Move units onto ship in accordance with this spinButton*/
			if (diff > 0)
			{
				System.Console.WriteLine("Moving " + diff + " units aboard from col of " + landUnits.Count);
				for(int i=0; i<diff; i++)
					troops.Add(landUnits[i]);
				Orig_LoadUnits loadcmd = new Orig_LoadUnits(ship, troops, port, sea);
				game.State.Execute(loadcmd);
			} else if (diff < 0) {
			 	System.Console.WriteLine("Moving " + (-1*diff) + " units ashore from col of " + ship.UnitsAboard.Count);
			 	/* Walk through each ship, moving appropriate people to the troops structure for
			 	 * execution. */
			 	 
			 	for(int i=0; i<(-1*diff); i++)
					troops.Add(ship.UnitsAboard[i]);
			 	Orig_UnloadUnits unloadcmd = new Orig_UnloadUnits(ship, troops, port, sea);
			 	game.State.Execute(unloadcmd);
			}			
			
			if (!shipIt.MoveNext())
				break; // will happen at the end of our ship list
			ship = (Navy)shipIt.Current;
		}
		
	}
	public void on_NavalCancel_clicked(System.Object obj, EventArgs e)
	{
		LoadNavalOptions.Hide();	
	}
	public void on_LoadNavalOptions_delete_event(System.Object obj, EventArgs e)
	{	
		LoadNavalOptions.Hide();	
	}	
	
	/* Ground Movement Options */
	public void showMoveOptions(Territory previous, Territory next)
	{
		ArrayList data = (ArrayList)game.State.CurrentState.Data;
		ArrayList moveCost = (ArrayList)data[0];
	
		MoveGroundLabel.Text = "Moving from " + previous.Name + " to " + next.Name;
		foreach(Widget wid in MoveGroundTable)
			MoveGroundTable.Remove(wid);
		
		RadioButton btn = null;
		uint row = 1;
		foreach(Stock s in moveCost) 
		{
			String label = "Use " + (-1*s.Number) + " " + s.Good.Name;
			if (btn == null) {
				btn = new RadioButton(label);
				data.Insert(2, btn); /* Initialize this value */
			} else
				btn = new RadioButton(btn, label);
			btn.Toggled += on_MoveGroundRadio;
			MoveGroundTable.Attach(btn, 0, 1, row, row+1);
			row++;
		}
		
		MoveGroundOptions.ShowAll();		
	}

	public void on_MoveAccept_clicked(System.Object obj, EventArgs e)
	{
		ArrayList data = (ArrayList)game.State.CurrentState.Data;
		ArrayList moveCost = (ArrayList)data[0];
		Orig_MoveUnit cmd = (Orig_MoveUnit)data[1];
		Gtk.RadioButton sel = (Gtk.RadioButton)data[2];
		
		int i=0;
		foreach(Gtk.Widget wid in MoveGroundTable) {
			if (wid == sel)
				break;
			if (! (wid is Gtk.RadioButton) )
				continue;
			i++;
		}
		
		/* i is the REVERSE number in the arraylist, since 
		 * we read from the table in reverse order to how
		 * we put things in, so we need to swap a bit */
		cmd.moveCost = moveCost.GetRange((moveCost.Count-1-i),1);
		game.State.Execute(cmd);
		MoveGroundOptions.Hide();
	}
	public void on_MoveGroundRadio(System.Object obj, EventArgs e)
	{
		((ArrayList)game.State.CurrentState.Data).Insert(2, obj);
	}
	public void on_MoveCancel_clicked(System.Object obj, EventArgs e)
	{
		MoveGroundOptions.Hide();
	}
	public void on_MoveGroundOptions_delete_event(System.Object obj, EventArgs e)
	{
		MoveGroundOptions.Hide();
	}	
	
	/* Strategic Target Selection Options */
	public void on_StrategicTargetSelection_delete_event(System.Object obj, EventArgs e)
	{
	}	
	public void on_StratTargetOkay_clicked(System.Object obj, EventArgs e)
	{
	}
	public void on_StratTargetCancel_clicked(System.Object obj, EventArgs e)
	{
	}
	
	/* Strategic Battle Selection Options */
	public void on_StrategicBattle_delete_event(System.Object obj, EventArgs e)
	{
	}	
	public void on_StratBattle_clicked(System.Object obj, EventArgs e)
	{
	}
	
	/* Conventional Battle Selection Options*/
	public void on_ConventionalBattle_delete_event(System.Object obj, EventArgs e)
	{
	}	
	public void on_ConvBattleOkay_clicked(System.Object obj, EventArgs e)
	{
	}	
	public void on_ConvBattleCancel_clicked(System.Object obj, EventArgs e)
	{
	}	
	
			
}
}

