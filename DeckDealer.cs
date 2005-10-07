// created on 10/06/2005 at 14:26
using System;
using Gtk;
using Pango;
using Gdk;

namespace Gpremacy {
class DeckDealer {

	/* Deck Dealer (Research / Prospecting) */
	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.Window DeckDealerWindow;
	[Glade.Widget] Gtk.Label DeckDealerLegend;
	[Glade.Widget] Gtk.Label DeckDealerStatus;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawLeft;
	[Glade.Widget] Gtk.DrawingArea DeckDealerDrawRight;
	[Glade.Widget] Gtk.Button DeckDealerOkay;
	[Glade.Widget] Gtk.Button DeckDealerFlip;
	
	int cardsFlipped;
	int costPerCard;
	Resource targetResource;
	Unit targetUnit;

	private static DeckDealer instance;
	private static int numOfReference;	
	
	private DeckDealer()
	{
		numOfReference = 0;		
	}
	
	public static DeckDealer GetInstance()
	{
		if(instance == null)
		{
			instance = new DeckDealer();
			instance.init();
		}
		numOfReference++;
		return instance;
	}           

	public static int Reference
	{
		get { return numOfReference; }
	} 	

	
	public void init()
	{
		/* Make Our Connection!*/
		Glade.XML gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "DeckDealerWindow", null);
		gxml.Autoconnect (this);
		cardsFlipped = 0;
		costPerCard = 200; 
		targetResource = null;
		targetUnit = null;
	}
	
	public int CostPerCard
	{
		get { return costPerCard; }
		set { costPerCard = value; }
	}
	
	public int CardsFlipped
	{
		get { return cardsFlipped; }
		set { cardsFlipped = value; }
	}
	
	public Resource TargetResource 
	{
		get { return targetResource; }
		set { targetResource = value; }
	}

	public Unit TargetUnit 
	{
		get { return targetUnit; }
		set { targetUnit = value; }
	}
	
	public bool targetIsResource() 
	{
		return (targetResource != null);
	}
		
	public void show()
	{
		cardsFlipped = 0;		
		Game.GetInstance().ShuffleResourceCards();
		
		/* This window is modal */		
		DeckDealerWindow.Modal = true;
		DeckDealerWindow.TransientFor = MainWindow;
		
		DeckDealerWindow.SetSizeRequest(400,300);
		
		DeckDealerFlip.Sensitive = true; // default on
		
		DeckDealerWindow.DeleteEvent += on_DeckDealer_delete_event;
		DeckDealerWindow.ExposeEvent += on_DeckDealer_exposed;
		
		DeckDealerOkay.Clicked += on_DeckDealerOkay_clicked;
		DeckDealerFlip.Clicked += on_DeckDealerFlip_clicked;
		
		DeckDealerWindow.ShowAll();
		updateStatusBoxes();		
	}
	
	private void updateStatusBoxes()
	{
		DeckDealerLegend.Text = "Searching For ";
		if (targetIsResource())
			DeckDealerLegend.Text += targetResource.Name; 
		else
			DeckDealerLegend.Text += targetUnit.Name;
					
		if (checkForSufficientWealth())
		{
			DeckDealerStatus.Text = "Cost So Far: $" + cardsFlipped*costPerCard + "M";
		} else {
			DeckDealerFlip.Sensitive = false;
			DeckDealerStatus.Text = "You do not have enough wealth to continue.";
			if (!targetIsResource())
				DeckDealerStatus.Text += "\nYou must have enough resources and wealth to"
									  + "\npurchase this unit immediately after your research"
									  + "\nis complete."; 
		}
	}
	
	private bool checkForSufficientWealth()
	{
		Player p = Game.GetInstance().State.CurrentPlayer;
		int totalMoney = 0;
		if (!targetIsResource())
		{
		/* Check resources */
			foreach (Stock s in targetUnit.CostResources) 
			{
				if (p.getStockpileAmount(s.Good) < s.Number)
			 	{
			 		System.Console.WriteLine("You do not have enough " + s.Good.Name +".");
				 	return false;
			 	}
			}
			totalMoney += targetUnit.CostMoney;
		}
		totalMoney += (cardsFlipped+1)*costPerCard;
		
		/* Check funds */
		if (p.Money < totalMoney)
			return false;
			 
		return true;
	}
	
	private void on_DeckDealer_exposed (object o, ExposeEventArgs args)
	{
		Gdk.GC textcolor = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		Gdk.GC cardoutline = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		Gdk.GC cardntback = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		Gdk.GC cardback = new Gdk.GC(DeckDealerDrawLeft.GdkWindow);
		
		textcolor.RgbFgColor = new Gdk.Color(0,0,0);				
		cardback.RgbFgColor = new Gdk.Color(225,225,225);
		cardoutline.RgbFgColor = new Gdk.Color(0,0,50);
		cardntback.RgbFgColor = new Gdk.Color(120,120,255);
	
		drawCardFace(DeckDealerDrawLeft.GdkWindow, cardoutline, cardntback, 10, 10, 150, 210);
		drawCardFace(DeckDealerDrawRight.GdkWindow, cardoutline, cardback, 10, 10, 150, 210);
		
        Pango.Layout layout = DeckDealerDrawLeft.CreatePangoLayout("");
        layout.Width = 145000; // I don't have a clue the units of this number, but it works
        layout.Wrap = Pango.WrapMode.Word;
        layout.Alignment = Pango.Alignment.Center;
        
       	layout.SetText("\n\nGpremacy Resource Card");       	
        DeckDealerDrawLeft.GdkWindow.DrawLayout(textcolor, 15, 20, layout);
                
        layout.SetText(Game.GetInstance().CurrentResourceCard.toString());
        DeckDealerDrawRight.GdkWindow.DrawLayout(textcolor, 15, 20, layout);
        								
	}
	public void drawCardFace(Gdk.Window win, Gdk.GC outline, Gdk.GC fill, int x, int y, int w, int h)	
	{
		int top = y, bottom = h+y, left = x, right = w+x, corner = 25;
		int hcorner = (int)Math.Ceiling((double)corner/2); // half corner
		/* Outline of Arcs */
		win.DrawArc(outline, true, left-1, top-1, corner, corner, 90 * 64, 90 * 64);
		win.DrawArc(outline, true, left-1, bottom-corner+1, corner, corner, 180 * 64, 90 * 64);
		win.DrawArc(outline, true, right-corner+1, bottom-corner+1, corner, corner, 270 * 64, 90 * 64);
		win.DrawArc(outline, true, right-corner+1, top-1, corner, corner, 360 * 64, 90 * 64);
		/* Fill of Arcs */		
		win.DrawArc(fill, true, left, top, corner, corner, 90 * 64, 90 * 64);
		win.DrawArc(fill, true, left, bottom-corner, corner, corner, 180 * 64, 90 * 64);
		win.DrawArc(fill, true, right-corner, bottom-corner, corner, corner, 270 * 64, 90 * 64);
		win.DrawArc(fill, true, right-corner, top, corner, corner, 360 * 64, 90 * 64);
		/* Fill of Card */
		win.DrawRectangle(fill, true, left+hcorner, top, right-left-corner, bottom-top);
		win.DrawRectangle(fill, true, left, top+hcorner, right-left, bottom-top-corner);
		/* Outline of Card */
		win.DrawLine(outline, left+hcorner, top, right-hcorner, top);
		win.DrawLine(outline, left+hcorner, bottom, right-hcorner, bottom);
		win.DrawLine(outline, left, top+hcorner, left, bottom-hcorner);
		win.DrawLine(outline, right, top+hcorner, right, bottom-hcorner);
	}
	private void on_DeckDealerFlip_clicked(System.Object obj, EventArgs e)
	{		
		cardsFlipped++;
		Game.GetInstance().NextResourceCard();
		
		ResourceCard card = Game.GetInstance().CurrentResourceCard;
		if (card.isResource() && targetIsResource())
		{
			Console.WriteLine("Both Are Resources");
			if (card.Good.Name == targetResource.Name)
			{
				Console.WriteLine("Found Target Resource");
				DeckDealerFlip.Sensitive = false;
			}
		} else if (!card.isResource() && !targetIsResource()) {
			Console.WriteLine("Both Are Units");
			if (card.Knowledge.Name == targetUnit.Name)
			{
				Unit nu = targetUnit.Clone(Game.GetInstance().State.CurrentPlayer); // copy that, rogue two.
				
				Console.WriteLine("Found Target Unit");
				/* Set available */
				Game.GetInstance().State.CurrentPlayer.makeUnitAvailable(nu);
				
				/* Make an immediate purchase */
				Orig_PurchaseUnit cmd = new Orig_PurchaseUnit(nu, Game.GetInstance().State.CurrentPlayer);
				Game.GetInstance().State.Execute(cmd);
				Orig_BuildUnit cmd2 = new Orig_BuildUnit(nu, null, Game.GetInstance().State.CurrentPlayer);
				Game.GetInstance().State.Execute(cmd2);
								
				DeckDealerFlip.Sensitive = false;
			}
		}		
				
		on_DeckDealer_exposed(null,null); // evil! Update screen
		updateStatusBoxes();
		//DeckDealer.SendExpose(null); // less evil, but crashes
	}	
	private void on_DeckDealerOkay_clicked(System.Object obj, EventArgs e)
	{
		if (cardsFlipped > 0) {
			Orig_ChargeMoney cmd = new Orig_ChargeMoney(cardsFlipped*costPerCard, Game.GetInstance().State.CurrentPlayer);
			Game.GetInstance().State.Execute(cmd);
		}
		DeckDealerWindow.Hide();
	}
	private void on_DeckDealer_delete_event(System.Object obj, EventArgs e)
	{
		if (cardsFlipped > 0) {
			Orig_ChargeMoney cmd = new Orig_ChargeMoney(cardsFlipped*costPerCard, Game.GetInstance().State.CurrentPlayer);
			Game.GetInstance().State.Execute(cmd);
		}
		DeckDealerWindow.Hide();
	}
}
}