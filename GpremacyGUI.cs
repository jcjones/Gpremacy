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
	[Glade.Widget] Gtk.Button NavalOkay;
		
	/* Buy/Sell Resources Options */
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
	
	/* Build Forces and/or Weapons */
	[Glade.Widget] Gtk.Window UnitBuy;
	[Glade.Widget] Gtk.Table UnitBuyTable;
	[Glade.Widget] Gtk.Label UnitBuyCost;
	
	/* Deck Dealer (Research / Prospecting) */
	[Glade.Widget] Gtk.Window DeckDealer;
	[Glade.Widget] Gtk.Label DeckDealerLegend;
	[Glade.Widget] Gtk.Label DeckDealerStatus;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawLeft;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawRight;
	
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

		foreach (Gtk.Widget wid in LoadNavalOptionsTable)
			if (wid is Gtk.SpinButton)
				if (resultOnShore < 1)
					((Gtk.SpinButton)wid).SetRange(0.0, ((Gtk.SpinButton)wid).Value);
				else
					((Gtk.SpinButton)wid).SetRange(0.0, 4.0);
		
		LoadNavalOptionsCounter.Text = "Friendlies on shore: " + resultOnShore;
		
		NavalOkay.Sensitive = (resultOnShore >= 0);
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
	
	/* Build Forces And/Or Weapons Options */
	
	public void showUnitBuy (){
	/*[Glade.Widget] Gtk.Window UnitBuy;
	[Glade.Widget] Gtk.Table UnitBuyTable;*/
		foreach(Widget wid in UnitBuyTable)
			UnitBuyTable.Remove(wid);

		uint row = 0;
		foreach(Unit u in Game.GetInstance().AllUnits) {
			Gtk.Label lbl = new Gtk.Label(u.Name);
			Gtk.Label cost = new Gtk.Label(u.Costs);
			UnitBuyTable.Attach(lbl, 0, 1, row, row+1);
			UnitBuyTable.Attach(cost, 1, 2, row, row+1);
			
			if (Game.GetInstance().State.CurrentPlayer.canBuild(u)) {
				Gtk.SpinButton spin = new Gtk.SpinButton(0.0, 100.0, 1.0);
				spin.ValueChanged += displayUnitBuyCost;
				UnitBuyTable.Attach(spin, 2, 3, row, row+1);
			} else {
				Gtk.Button btn = new Gtk.Button();
				btn.Label = "Research";
				btn.Clicked += on_UnitBuy_research_click;
				UnitBuyTable.Attach(btn, 2, 3, row, row+1);
			}			
			
			row++;
		}
		
		displayUnitBuyCost(null, null);
		UnitBuy.ShowAll();
	}
	
	public void displayUnitBuyCost(System.Object obj, EventArgs e)
	{
		ArrayList purchasedUnits; // of Unit
		Dictionary bill;
		int costMoney;

		UnitBuyCost.Text = calculateUnitBuyCost(out costMoney, out bill, out purchasedUnits);
	}
	
	public string calculateUnitBuyCost(out int costMoney, out Dictionary bill, out ArrayList purchasedUnits)
	{
		string str = "";		

		purchasedUnits = new ArrayList(); // of Unit
		bill = new Dictionary();
		costMoney = 0;
		
		int i = 0;
		
		int numTripleTacticalUnits = 0; // The number of Armies and Navies bought
		foreach(Widget wid in UnitBuyTable)
		{
			
			if (! (wid is Gtk.SpinButton) && ! (wid is Gtk.Button) )
				continue;
			/* Take into account things which are "Research" */
			if (wid is Gtk.Button) 
			{
				i++; continue;
			}
			
			int index = Game.GetInstance().AllUnits.Count-1-i;
			if (index < 0 || index > Game.GetInstance().AllUnits.Count) 
			{
				throw (new Exception("calculateUnitBuyCost widget index is nuts: " + index));
			}
			
			Unit curUnit = (Unit)Game.GetInstance().AllUnits[index];
			int amt = (int)((Gtk.SpinButton)wid).Value;
			//System.Console.WriteLine("Sel " + curUnit.Name + " of " + amt);
			 
			for(int j=0; j<amt; j++)
				purchasedUnits.Add(curUnit);

			if (curUnit.CostMultiplicity == 3)
			{
				numTripleTacticalUnits += amt;
			} else {			
				foreach (Stock r in curUnit.CostResources) {
					bill.IncValue(r.Good.Name, amt*r.Number);
				}
				costMoney += curUnit.CostMoney*amt;
			}
			i++;			
		}
		int tripleAmt = (int)Math.Ceiling((double)numTripleTacticalUnits/3);
		System.Console.WriteLine("TripleAmt: " + tripleAmt + " NumTriple:" + numTripleTacticalUnits);
		costMoney += tripleAmt*300;
		bill.IncValue(new Oil(), -1*tripleAmt);
		bill.IncValue(new Minerals(), -1*tripleAmt);
		bill.IncValue(new Grain(), -1*tripleAmt);
		
		str += "Costs: $" + costMoney.ToString() + "M,";
		foreach(DictionaryEntry d in bill.Data) 
		{
			if ((Int32)d.Value < 0)
				str += " " + (-1*(Int32)d.Value) + " " + ((Resource)d.Key).Name + ","; 
		}
		
		string warning = "";
		if (tripleAmt*3 > numTripleTacticalUnits)
		{
			warning = ".\nYou can buy more Armies and Navies for the same cost!";
		}
		
		return str.TrimEnd(new char[] {' ', ','})+warning;
	}

	public void on_UnitBuy_research_click(System.Object obj, EventArgs e)
	{
		Console.WriteLine("Researching " + ((Gtk.Button)obj).Label);
		showDeckDealer();
	}
	public void on_UnitBuy_delete_event(System.Object obj, EventArgs e)
	{
		UnitBuy.Hide();
	}	
	public void  on_UnitBuyOkay_clicked (System.Object obj, EventArgs e)
	{
		ArrayList purchasedUnits; // of Unit
		Dictionary bill;
		int costMoney;
		Player me = Game.GetInstance().State.CurrentPlayer;

		/* Build bill */
		calculateUnitBuyCost(out costMoney, out bill, out purchasedUnits);
		
		/* Check funds */
		foreach (DictionaryEntry d in bill.Data) 
		{
			if (me.getStockpileAmount((Resource)d.Key) < (Int32)d.Value)
				 {
				 	writeToLog("You do not have enough " + ((Resource)d.Key).Name +".");
				 	return;
				 }
		}
		if (me.Money < costMoney)
		{
		 	writeToLog("You do not have enough money.");
		 	return;
		}

		/* Purchase units */
		int MultiplicityCounter = 0;
		foreach (Unit u in purchasedUnits)
		{
			if (u.CostMultiplicity == 3)
			{
				MultiplicityCounter++;
				if (1 != MultiplicityCounter%3)
					continue;
			}	
			Orig_PurchaseUnit cmd = new Orig_PurchaseUnit(u, me);
			Game.GetInstance().State.Execute(cmd);
		}
		
		/* Build Strategic Units */
		ArrayList delayedUnits = new ArrayList();
		foreach (Unit u in purchasedUnits) {
			if (u is TacticalUnit)
			{
				delayedUnits.Add(u);
				continue;
			} else if (u is StrategicUnit) {
				Orig_BuildUnit cmd2 = new Orig_BuildUnit(u, null, me);
				Game.GetInstance().State.Execute(cmd2);
			}							
		}
		
		/* Queue the rest of the order and hide the window */
		((Orig_Play5Build)Game.GetInstance().State.CurrentState).UnitsToBuild.AddRange(delayedUnits);
		
		UnitBuy.Hide();
	}
	
	/* Deck Dealer */
	/*[Glade.Widget] Gtk.Window DeckDealer;
	[Glade.Widget] Gtk.Label DeckDealerLegend;
	[Glade.Widget] Gtk.Label DeckDealerStatus;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawLeft;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawRight;*/
	
	public void showDeckDealer()
	{	
		/* This window is modal */
		
		DeckDealer.Modal = true;
		DeckDealer.TransientFor = MainWindow;
		DeckDealer.SetSizeRequest(500,300);
		DeckDealerLegend.Text = "No Legend";
		DeckDealerStatus.Text = "No Status";
		
		if (DeckDealer == null)
		{
			System.Console.WriteLine("NULLNESS");
		}
		
		/*Gdk.GC cards = DeckDealer.Style.BlackGC;
		Gdk.GC text = DeckDealer.Style.BlackGC;
		*/
				
		DeckDealer.ExposeEvent += on_DeckDealer_exposed;
		
		DeckDealer.ShowAll();
		
	}
	public void on_DeckDealer_exposed (object o, ExposeEventArgs args)
	{
		Gdk.GC text = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		Gdk.GC cards = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		
		text.Foreground = new Gdk.Color(0,0,0);
		text.Background = new Gdk.Color(225,25,225);
				
		cards.Foreground = new Gdk.Color(255,255,255);
		cards.Background = new Gdk.Color(25,25,255);
	
		System.Console.WriteLine("Exposed DeckDealer");
		drawCardFace(DeckDealerDrawLeft.GdkWindow, cards, "");
		drawCardFace(DeckDealerDrawRight.GdkWindow, text, "Testing!!!\nMeee");							
	}
	public void drawCardFace(Gdk.Window win, Gdk.GC gc, string text)	
	{
		int top = 10, bottom = 100, left = 10, right = 200, corner = 5;
		win.DrawLine(gc, top+corner, left+corner, top+corner, right-corner);
		win.DrawLine(gc, bottom-corner, left+corner, bottom-corner, right+corner);
		win.DrawLine(gc, top+corner, left+corner, bottom-corner, left+corner);
		win.DrawLine(gc, top+corner, right-corner, bottom-corner, right-corner);
		win.DrawArc(gc, true, top, left, corner, corner, 90 * 64, 180 * 64);
	}	
	public void on_DeckDealerOkay_clicked(System.Object obj, EventArgs e)
	{
		DeckDealer.Hide();
	}
	public void on_DeckDealer_delete_event(System.Object obj, EventArgs e)
	{
		DeckDealer.Hide();
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


