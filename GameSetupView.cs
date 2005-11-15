// created on 10/27/2005 at 10:06
using System;
using System.Collections;
using System.Threading;
using Gtk;
using Pango;
using Gdk;
using Gpremacy.Network;

namespace Gpremacy {
class GameSetupView {
	Gtk.Window MainWindow;
	
	[Glade.Widget] Gtk.Window GameSetup;
	[Glade.Widget] Gtk.Button GameSetupSingleStart;
	[Glade.Widget] Gtk.VBox GameSetupSingleCountryVBox;
		
	[Glade.Widget] Gtk.Entry GameSetupEntryIP;
	[Glade.Widget] Gtk.SpinButton GameSetupSpinPort;
	[Glade.Widget] Gtk.Button GameSetupMultiStart;
	[Glade.Widget] Gtk.Button GameSetupConnectButton;
	[Glade.Widget] Gtk.RadioButton GameSetupRadioClient;
	[Glade.Widget] Gtk.RadioButton GameSetupRadioServer;

	[Glade.Widget] Gtk.Statusbar GameSetupStatusBar;
	
	[Glade.Widget] Gtk.VBox GameSetupCountryVBox;
	[Glade.Widget] Gtk.Table GameSetupPlayersTable;
	
	[Glade.Widget] Gtk.Label GameSetupPortLabel;
	
	Timer UpdateStatusTimer;

	public GameSetupView(Gtk.Window mainWindow)
	{
		MainWindow = mainWindow;
		init();
	}
	
	public void init()
	{
		/* Make Our Connection!*/
		Glade.XML gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "GameSetup", null);
		gxml.Autoconnect (this);
		
		GameSetup.Modal = true;
		GameSetup.TransientFor = MainWindow;

		GameSetup.DeleteEvent += on_GameSetup_delete_event;
		GameSetupSingleStart.Clicked += on_GameSetupSingleStart_clicked;
		GameSetupConnectButton.Clicked += on_GameSetupConnectButton_clicked;
		GameSetupMultiStart.Clicked += on_GameSetupMultiStart_clicked;
		
		GameSetupRadioClient.Toggled += on_MultiRadio_changed;
		GameSetupRadioServer.Toggled += on_MultiRadio_changed;
				
		GameSetupPortLabel.Text += "(normally " + Game.DefaultPort.ToString();
		
		GameSetupMultiStart.Sensitive = false;
				
		populatePlayers(GameSetupSingleCountryVBox);
		populatePlayers(GameSetupCountryVBox);
		
		on_MultiRadio_changed(null, null); /* Fake event */
	
    	// Create a timer that waits one second, then invokes every second.
	    UpdateStatusTimer = new Timer(new TimerCallback(updateStatus), null, 1000, 1000);
	    UpdateStatusTimer.ToString(); // Shut UP, you damn warning!

		GameSetupEntryIP.Text = "192.168.1.150";
	}

	public Gtk.Window GUI
	{
		get { return GameSetup; }
	}
	
	private static void updateStatus(System.Object state)
	{
		Game game = Game.GetInstance();
		GameSetupView gsv = game.GUI.GameSetupView;
		
		if (Game.GetInstance().gameLink != null)
		{
			/* Show list of players */
			gsv.showPeers(game.gameLink.participants);
			/* Disable chosen countries in the dropdown */
			gsv.disablePlayersInList(game.gameLink.participants);			
			/* Disable the start button */
			gsv.GameSetupMultiStart.Sensitive = ((game.gameLink.numPeers() > 0) && (game.gameLink is Server));
		}
		
	}
	
	public void showGameSetupUI()
	{	
		GameSetup.ShowAll();
	}
	
	public void addStatusText(string text)
	{
		GameSetupStatusBar.Push(1, text);
	}
	
	public void reenableConnectButton()
	{
		GameSetupConnectButton.Sensitive = true;
	}
	
	private void on_GameSetup_delete_event(System.Object obj, DeleteEventArgs e)
	{
		/* Stop Application */		
		Game.GetInstance().Quit();
		
		e.RetVal = true; // True == do not allow delete
	}

	private void on_MultiRadio_changed (System.Object obj, EventArgs e)
	{
		if (GameSetupRadioClient.Active)
		{
			/* We're a client, disable server options, enable client ones. */
			//GameSetupEntryPort.Sensitive = true;
			GameSetupEntryIP.Sensitive = true;
			GameSetupConnectButton.Label = "Connect";			
		} else {
			/* We're a server, disable client options, enable server ones. */
			//GameSetupEntryPort.Sensitive = false;
			GameSetupEntryIP.Sensitive = false;
			GameSetupConnectButton.Label = "Start Hosting";			
		}		
	}		
	private void on_GameSetupSingleStart_clicked (System.Object obj, EventArgs e)
	{
		LocalLink link = new LocalLink();
		Game.GetInstance().gameLink = link;
		
		Player p = Game.GetInstance().PlayerByName("Confederacy of South America");
		//link.startAIPlayer(1, p); 

		p = Game.GetInstance().PlayerByName("People's Republic of China");
		link.startAIPlayer(1, p); 
		
		Game.GetInstance().State.BeginGame();
	}
	private void on_GameSetupConnectButton_clicked (System.Object obj, EventArgs e)
	{
		string IP = GameSetupEntryIP.Text;
		int port = Int32.Parse(GameSetupSpinPort.Text);

		GameSetupConnectButton.Sensitive = false;
		
		if (GameSetupRadioClient.Active)
		{
			/* We're a client, connect */
			Game.GetInstance().gameLink = new Client(IP, port);
		} else {
			/* We're a host, listen */
			Game.GetInstance().gameLink = new Server(port);
		}
	}
	private void on_GameSetupMultiStart_clicked (System.Object obj, EventArgs e)
	{
		/* Validate that clients and players make a one to one mapping */
		if ((Game.GetInstance().gameLink != null) && (Game.GetInstance().gameLink.playerChoicesValid()) )
			((Server)Game.GetInstance().gameLink).sendBeginGame();		
	}
	
	private void populatePlayers(Gtk.VBox box)
	{
		//combo.Clear();
		Gtk.RadioButton btn = null;
		Gtk.RadioButton firstbtn = null;
		
		ArrayList players = Game.GetInstance().Players;
		foreach (Player player in players)
		{
			if (firstbtn == null)
			{
				firstbtn = new Gtk.RadioButton(null, player.Name);
				firstbtn.Active = false;
				btn = firstbtn;
			} else {
				btn = new Gtk.RadioButton(firstbtn, player.Name);
			}
			
			box.Add(btn);
	
			btn.Toggled += on_Player_toggled;
						
		}
	}
	
	private void on_Player_toggled(System.Object obj, EventArgs e)
	{
		//((Server)Game.GetInstance().gameLink);
		string playerName = whoAmI(GameSetupCountryVBox);
		
		if (Game.GetInstance().gameLink != null)
			Game.GetInstance().gameLink.sendWhoIAm(playerName);		
	}
	
	public string whoAmI(bool isSinglePlayer)
	{
		if (isSinglePlayer)
			return whoAmI(GameSetupSingleCountryVBox);
			
		return whoAmI(GameSetupCountryVBox);
	}
	
	private string whoAmI(Gtk.VBox box)
	{	
		foreach (Gtk.Widget wid in box)
		{
			if (! (wid is Gtk.RadioButton) )
				continue;
			if ( ((Gtk.RadioButton)wid).Active )
			{
				return ((Gtk.RadioButton)wid).Label;
			}
		}

		return "Unknown";
	}
	
	private void disablePlayersInList(ArrayList participants)
	{
		if (participants == null)
			return;
	
		foreach (Gtk.Widget wid in GameSetupCountryVBox) {
			if ( ! ( wid is Gtk.RadioButton) )
				continue;
			string btnText = ((Gtk.RadioButton)wid).Label;
			bool isUsed = false;			
			
			Monitor.Enter(participants);
			foreach (GameParticipant gp in participants)
			{
				if ( (gp.player != null) && (gp.player.Name == btnText) )
				{
					isUsed = true;
					break;
				}
			}
			Monitor.Exit(participants);
			
			wid.Sensitive = !isUsed;		
		}
	}
	
	private void showPeers(ArrayList participants)
	{
		foreach (Gtk.Widget wid in GameSetupPlayersTable)
		{
			GameSetupPlayersTable.Remove(wid);
		}
		uint row = 0;
		
		if (participants == null)
			return;
		
		string names, addrs;
		Monitor.Enter(participants);
		foreach (GameParticipant gp in participants)
		{
			if (gp.player == null)
				names = "Unchosen";
			else
				names = gp.player.Name;
			
			if (gp.endpoint == null)
				addrs = "Server";
			else
				addrs = gp.endpoint.ToString();

			Gtk.Label name = new Gtk.Label(names);
			Gtk.Label addr = new Gtk.Label(addrs);
			
			name.Visible = true;
			addr.Visible = true;
			
			GameSetupPlayersTable.Attach(name, 0, 1, row, row+1);
			GameSetupPlayersTable.Attach(addr, 1, 2, row, row+1);
			
			row++;
		}
		Monitor.Exit(participants);
	}
	
}
}