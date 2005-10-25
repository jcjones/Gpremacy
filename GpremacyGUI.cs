// project created on 07/05/2005 at 20:05
using System;
using System.Collections;
using System.Threading;
using Gtk;
using Glade;
using Pango;
using Gdk;

namespace Gpremacy {
class GpremacyGUI {
	GpremacyMap MapArea;
	Game game;
	Gdk.Region invalRegion;
	DeckDealer deckDealer;
	CombatView combatView;

	/* Main Window*/
	[Glade.Widget] Gtk.Viewport MapViewport;
	[Glade.Widget] Gtk.Window MainWindow;
	//[Glade.Widget] Gtk.ScrolledWindow MapScrolledWindow2;
		
	[Glade.Widget] Gtk.TextView OrderOfPlayTextBox;	
	[Glade.Widget] Gtk.TextView ResourcesTextBox;
	[Glade.Widget] Gtk.TextView WorldMarketTextBox;
	[Glade.Widget] Gtk.TextView LogTextBox;
//	[Glade.Widget] Gtk.TextView StatsTextBox;
//	[Glade.Widget] Gtk.TextView MiscTextBox;

	/* Naval Options */	
	[Glade.Widget] Gtk.Window LoadNavalOptions;
	//[Glade.Widget] Gtk.DrawingArea UnitLegendNaval;
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
	[Glade.Widget] Gtk.Button MarketBuySellProspect;
	
	/* Move Ground Options */
	[Glade.Widget] Gtk.Window MoveGroundOptions;
	[Glade.Widget] Gtk.Label MoveGroundLabel;
	[Glade.Widget] Gtk.Table MoveGroundTable;
	
	/* Build Forces and/or Weapons */
	[Glade.Widget] Gtk.Window UnitBuy;
	[Glade.Widget] Gtk.Table UnitBuyTable;
	[Glade.Widget] Gtk.Label UnitBuyCost;
	
	/* Prospect for Resources */
	[Glade.Widget] Gtk.Window ProspectSelection;
	[Glade.Widget] Gtk.Table ProspectSelectionTable;
	//[Glade.Widget] Gtk.Label ProspectSelectionLegend;
	
	/* Resource Card View */
	[Glade.Widget] Gtk.Window ResourceCardView;
	[Glade.Widget] Gtk.Table ResourceCardViewTable;
	//[Glade.Widget] Gtk.Label ResourceCardViewLegend;
			
	public GpremacyGUI(Game i)
	{
		game = i;
		
	}
	
	public void init() 
	{
		Application.Init ();
		
		// Initialize map
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

		// Init sub-widgets and windows
		updateGUIStatusBoxes();			
		deckDealer = DeckDealer.GetInstance();
		combatView = new CombatView();
		
		/* Fake Crap */
		/*Player Play = (Player)Game.GetInstance().LocalPlayers[0];
		Territory Hom = Map.getTerritoryByName("Russia");
		if (Play == null) Console.WriteLine("NULL RUSSIA");
		Orig_BuildUnit cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);
		cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);
		cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);
		
		Play = (Player)Game.GetInstance().LocalPlayers[1];
		Hom = Map.getTerritoryByName("Iraq");
		if (Play == null)  Console.WriteLine("NULL IRAQ");
		cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);							
		cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);							
		cmd1 = new Orig_BuildUnit(new Army(Play, Hom), Hom, Play);
		Game.GetInstance().State.Execute(cmd1);*/
		/* Done Fake Crap */							
		Application.Run ();
	}
	
	public GpremacyMap Map
	{
		get { return MapArea; }
	}
	
	public CombatView CombatView
	{
		get { return combatView; }
	}
	
	public void OnMotion (object o, MotionNotifyEventArgs args)
	{
      	double x = args.Event.X;//+MapScrolledWindow2.Hadjustment.Value;
   		double y = args.Event.Y;//+MapScrolledWindow2.Vadjustment.Value;

   		/* Find territory */
	   	Territory target = null;   	
	   	
	   	foreach (Territory here in MapArea.Territories)
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
	   	
	   	foreach (Territory here in MapArea.Territories)
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
		//MapArea.GdkWindow.InvalidateRegion(target.MapTerritory.region,true);

		MapArea.GdkWindow.InvalidateRegion(Map.Region, true);
	}
	
	/* Reset and start a new game */
	public void on_new1_activate(System.Object obj, EventArgs e) 
	{
			game.DistributeResourceCards(); // Should not be in this file..
			game.GiveInitialUnits(); // Should not be in this file..	
	}
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
	/* Requires GTK# 2.6 */
	/*
			string[] authors = new string[]{
				"J.C. \"Pug\" Jones <jcjones@ufl.edu>",
			};

			string[] documenters = new string[]{
				"J.C. \"Pug\" Jones <jcjones@ufl.edu>",
			};
			
			Gtk.AboutDialog dialog = new Gtk.AboutDialog();
			dialog.Name = "Gpremacy: The Game of the Superpowers";
			dialog.Comments = "An implementation of the 1984 board game Supremacy.";
			dialog.Authors = authors;
			dialog.Documenters = documenters;
			dialog.Copyright = "(C) 2005 J.C. \"Pug\" Jones";
			dialog.License = "Licensed under the GNU GPL version 2 or later.";
			dialog.Website = "http://gpremacy.nongnu.org/";
			dialog.WebsiteLabel = "Gpremacy on the Web";
			dialog.ShowAll();
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
		/* Hide All Windows. This is more of a "just in case", since all windows are modal. */
		foreach (Gtk.Window w in Gtk.Window.ListToplevels())
		{
			if (w != MainWindow)
			{
				Console.WriteLine("Toplevel window: " + w.Title);
				w.Hide();
			}
		}
		
		game.State.nextPlayer();
		updateGUIStatusBoxes();
	}
	
	public void on_strategic_attack1_activate(System.Object obj, EventArgs e)
	{
		State curState = Game.GetInstance().State.CurrentState;
		if (curState is Orig_Play3Attack)
			((Orig_Play3Attack)curState).strategicAttack();
	}
	
	public void updateGUIStatusBoxes()
	{
		writeToOrderOfPlayTextBox("Turn Number: " + game.State.TurnNumber + "\nCurrent Player:\n" + game.State.CurrentPlayer.toString() + "\nCurrent State:\n" + game.State.StateIDName);								
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
    		//invalRegion.Destroy(); // Deprecated by GTK# 2.4
    	}    		
	}
	
	public void drawArrow(Territory a, Territory b)
    {
		clearArrow();

		//int x = a.MapTerritory.centerX;
		//int y = a.MapTerritory.centerY;
		//int w = b.MapTerritory.centerX - x;
		//int h = b.MapTerritory.centerY - y;

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
    
    public void showNuclearDetonationAnimation(Territory target)
	{	
    	Gdk.Window win = (Gdk.Window)Game.GetInstance().GUI.Map.GdkWindow;
    	
		int frames = GraphicsStorage.GetInstance().DetonationFrames;
		for (int i=0; i<frames; i++)
		{
			redrawTerritory(target);
			target.showNuke(win, i);			
			MapArea.GdkWindow.ProcessUpdates(true);
		    Thread.Sleep(20);
		}   
	}

    public void ShowError(string error)
    {       	
		MessageDialog dlg = new MessageDialog
     	(MainWindow, Gtk.DialogFlags.Modal,
     	Gtk.MessageType.Error, Gtk.ButtonsType.Ok,  GLib.Markup.EscapeText(error));
        dlg.Title = "Gpremacy Error!";     
        dlg.Run();
        dlg.Destroy();        
    }
    
    public void ShowWarning(string warning)
    {
		MessageDialog dlg = new MessageDialog
     	(MainWindow, Gtk.DialogFlags.Modal,
     	Gtk.MessageType.Warning, Gtk.ButtonsType.Ok,  GLib.Markup.EscapeText(warning));
     	dlg.Title = "Gpremacy Warning!";     
        dlg.Run();
        dlg.Destroy();
	}
    
    /* Market Buy/Sell Options */
    public void showMarketBuySell(int sell)
    {
    	MarketBuySellOkay.Sensitive = true;
    	MarketBuySellOkay.Clicked -= on_MarketBuyOkay_clicked;
    	MarketBuySellOkay.Clicked -= on_MarketSellOkay_clicked;
    	
   		MineralsScroll.ValueChanged -= MarketBuyProfitCalculation;
   		GrainScroll.ValueChanged -= MarketBuyProfitCalculation;
   		OilScroll.ValueChanged -= MarketBuyProfitCalculation;    		
   		MineralsScroll.ValueChanged -= MarketSellProfitCalculation;
   		GrainScroll.ValueChanged -= MarketSellProfitCalculation;
   		OilScroll.ValueChanged -= MarketSellProfitCalculation;
    	
    	if (sell > 0) { /* Sell Stage */
	    	MineralsScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Minerals()));
	    	OilScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Oil()));
	    	GrainScroll.SetRange(0.0, (double)game.State.CurrentPlayer.getStockpileAmount(new Grain()));
    		
    		MineralsScroll.ValueChanged += MarketSellProfitCalculation;
    		GrainScroll.ValueChanged += MarketSellProfitCalculation;
    		OilScroll.ValueChanged += MarketSellProfitCalculation;
    		
    		MarketSellProfitCalculation(null, null);
    		
    		MarketBuySellOkay.Clicked += on_MarketSellOkay_clicked;
    		
    		MarketBuySellProspect.Sensitive = false;
    		MarketBuySellProspect.Clicked -= on_MarketBuyProspect_clicked;
    	} else {  /* Buy Stage */
	    	MineralsScroll.SetRange(0.0, 99.0);
	    	OilScroll.SetRange(0.0, 99.0);
	    	GrainScroll.SetRange(0.0, 99.0);

    		MineralsScroll.ValueChanged += MarketBuyProfitCalculation;
    		GrainScroll.ValueChanged += MarketBuyProfitCalculation;
    		OilScroll.ValueChanged += MarketBuyProfitCalculation;
    		
    		MarketBuyProfitCalculation(null, null);
    		MarketBuySellOkay.Clicked += on_MarketBuyOkay_clicked;

    		MarketBuySellProspect.Sensitive = true;
    		MarketBuySellProspect.Clicked += on_MarketBuyProspect_clicked;
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
    	
    	if (cost > Game.GetInstance().State.CurrentPlayer.Money)
    	{
    		MarketBuySellOkay.Sensitive = false;
    	}
    	else
    	{
    		MarketBuySellOkay.Sensitive = true;
    	}
    	
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
		
		Console.WriteLine("ExeSell!");
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
		
		Console.WriteLine("ExeBuy!");
		Orig_Buy cmd = new Orig_Buy(ticket);
		game.State.Execute(cmd);			

		MarketBuySell.Hide();
	}
		
	public void on_MarketBuySell_delete_event(System.Object obj, EventArgs e)
	{	
		MarketBuySell.Hide();
	}
	
	public void on_MarketBuyProspect_clicked(System.Object obj, EventArgs e)
	{
		showProspectForResources();
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
				btn.Label = "Research " + u.Name;
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
		string str = ((Gtk.Button)obj).Label;
		str = str.Substring(str.IndexOf(" ")+1);		
		Console.WriteLine("Researching " + str);
		showDeckDealer(str);
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
		if (! Game.GetInstance().hasSufficientWeath(me, bill, costMoney) )
		{
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
	public void showDeckDealer(string searchString)
	{		
		foreach(Unit u in Game.GetInstance().AllUnits)
		{
			if (searchString.CompareTo(u.Name) == 0)
			{
				deckDealer.TargetUnit = u;
				deckDealer.show();
				return;
			}				
		}
		foreach(Resource r in Game.GetInstance().AllResources)
		{
			if (searchString.CompareTo(r.Name) == 0)
			{
				deckDealer.TargetResource = r;
				deckDealer.show();
				return;
			}				
		}
		writeToLog("ERROR: Attempting to Research/Prospect for ["+searchString+"], but that does not exist.");		
	}

	/* Prospect for Resources */
	/*[Glade.Widget] Gtk.Window ProspectSelection;
	[Glade.Widget] Gtk.Table ProspectSelectionTable;
	[Glade.Widget] Gtk.Label ProspectSelectionLegend;	
	*/
	
	public void showProspectForResources() 
	{
		foreach(Widget w in ProspectSelectionTable) 
			ProspectSelectionTable.Remove(w);

		uint row = 0;
		Gtk.RadioButton group = null;
		Gtk.RadioButton button = null;
		
		foreach(Resource r in Game.GetInstance().AllResources)
		{
			if (group == null) {
				button = new Gtk.RadioButton(r.Name);
				group = button;			
			} else {
				button = new Gtk.RadioButton(group, r.Name);
			}
			
			ProspectSelectionTable.Attach(button, 0, 1, row, row+1);
			//writeToLog("Labelling at row "+row+" a "+r.Name);
			row++;
		}

		ProspectSelection.ShowAll();		
	}
	public void on_ProspectSelectionOkay_clicked(System.Object obj, EventArgs e)
	{
		String result = "";
		Resource res = null;
		
		foreach (Gtk.Widget wid in ProspectSelectionTable)
		{
			if (wid is Gtk.RadioButton) {
				if (((Gtk.RadioButton)wid).Active) {
					result = ((Gtk.RadioButton)wid).Label;
					break;
				}
			}
		}
		writeToLog("Got result of ["+result+"].");
		
		foreach (Resource r in Game.GetInstance().AllResources)
		{
			if (r.Name == result) 
			{
				res = r;
				break;
			}
		}
		
		if (res != null) 
		{
			DeckDealer dd = DeckDealer.GetInstance();
			dd.TargetResource = res;
			dd.show();
		}
	}
	public void on_ProspectSelection_delete_event(System.Object obj, EventArgs e)
	{
		ProspectSelection.Hide();
	}	
	public void on_ProspectSelectionDone_clicked(System.Object obj, EventArgs e)
	{
		ProspectSelection.Hide();
	}
	
	/* Resource Card View */
	public void on_manage_resource_cards1_activate(System.Object obj, EventArgs e) 
	{
		Player curPlay = Game.GetInstance().State.CurrentPlayer;
		
		foreach(Gtk.Widget wid in ResourceCardViewTable)
		{
			ResourceCardViewTable.Remove(wid);
		}
		
		uint row = 0;
		foreach(ResourceCard card in curPlay.ResourceCards)
		{
			System.Console.WriteLine("Row " + row+ " has " + card.toString());
			Gtk.Label label = new Gtk.Label(card.toString());
			Gtk.ToggleButton button = new Gtk.ToggleButton("VOID");
			button.Active = card.Active;
			button.Toggled += ResourceCardViewTableButton_toggled;
			
			/* Toggle */
        	if (card.Active)
	        	button.Label = "Operating";        	
        	else
        		button.Label = "Idle";
        	if (card.Place.Owner != curPlay)
        		button.Sensitive = false;
			
			ResourceCardViewTable.Attach(label, 0, 1, row, row+1);
			ResourceCardViewTable.Attach(button, 1, 2, row, row+1);
			row++;
		}
	
		ResourceCardView.ShowAll();
	}
	
	void ResourceCardViewTableButton_toggled (object obj, EventArgs args)
    {
        Console.WriteLine ("Button Toggled");
        if (!(obj is Gtk.ToggleButton))
        	return;
        Gtk.ToggleButton btn = (Gtk.ToggleButton)obj;
        
        ArrayList ResourceCards = Game.GetInstance().State.CurrentPlayer.ResourceCards; 
        
		/* Determine correlation with ResourceCards */
        int cardIt = ResourceCards.Count;
		cardIt--;
		
		foreach(Widget wid in ResourceCardViewTable) 
		{
			if (wid is Gtk.ToggleButton) 
			{
				if (wid == obj)
					break;
				cardIt--;
			}			
		}

		/* We're dealing with cardIt */
		ResourceCard card = (ResourceCard)ResourceCards[cardIt];
		System.Console.WriteLine("Doing " +card.toString());
		
		/* Toggle */
		card.Active = btn.Active;
        if (btn.Active)
        	btn.Label = "Operating";        	
        else
        	btn.Label = "Idle";        	
        
        updateGUIStatusBoxes();
    }
	
	public void on_ResourceCardViewOkay_clicked(System.Object obj, EventArgs e)
	{
		ResourceCardView.Hide();
	}
		
	/* Conventional Battle Display */
	public void showConventionalBattle(Territory Target, Territory Stage, Player Attacker, Player Defender) 
	{
		combatView.BattleLocation = Target;
		combatView.StagingLocation = Stage;
		combatView.Attacker = Attacker;
		combatView.Defender = Defender;
		combatView.showConventionalBattle();
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
			
}
}


