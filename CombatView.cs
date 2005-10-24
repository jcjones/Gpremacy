// created on 10/20/2005 at 11:39
using System;
using System.Collections;
using Gtk;
using Pango;
using Gdk;

namespace Gpremacy {
class CombatView {

	/* Conventional Battle */
	[Glade.Widget] Gtk.Window MainWindow;
	[Glade.Widget] Gtk.Window ConventionalBattle;
	[Glade.Widget] Gtk.DrawingArea ConvBattleUnitLegend;
	[Glade.Widget] Gtk.DrawingArea ConvBattleAttackerUnits;
	[Glade.Widget] Gtk.DrawingArea ConvBattleDefenderUnits;
	[Glade.Widget] Gtk.Label ConvBattleLabel;
	[Glade.Widget] Gtk.Label ConvBattleAttackerDice;
	[Glade.Widget] Gtk.Label ConvBattleDefenderDice;
	[Glade.Widget] Gtk.Button ConvBattleOkay;
	[Glade.Widget] Gtk.Button ConvBattleCancel;
	
	Territory stageLocation;
	Territory battleLocation;
	Player attacker;
	Player defender;
	int attackerDice;
	int defenderDice;
	int attackerKills;
	int defenderKills;
	
	ArrayList attackingUnits;
	ArrayList defendingUnits;
	Dictionary attackingUnitsDictionary;
	Dictionary defendingUnitsDictionary;
	
	bool battleResolved;
	bool fullDefense;
	
	public CombatView() {
		init();
	}
	
	public void init()
	{
		/* Make Our Connection!*/
		Glade.XML gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "ConventionalBattle", null);
		gxml.Autoconnect (this);
		attacker = defender = null;
		battleLocation = stageLocation = null;
		
		battleResolved = false;
		fullDefense = true;

		/* This window is modal */		
		ConventionalBattle.Modal = true;
		ConventionalBattle.TransientFor = MainWindow;
		
		ConventionalBattle.SetSizeRequest(400,300);
		
		ConvBattleOkay.Clicked += on_ConvBattleOkay_clicked;
		ConvBattleCancel.Clicked += on_ConvBattleCancel_clicked;
		
		ConventionalBattle.DeleteEvent += on_ConventionalBattle_delete_event;
		ConventionalBattle.ExposeEvent += on_CombatView_exposed;		
	}
	
	public Player Attacker
	{
		get { return attacker; }
		set { attacker = value; }
	}
	public Player Defender
	{
		get { return defender; }
		set { defender = value; }
	}
	public Territory BattleLocation
	{
		get { return battleLocation; }
		set { battleLocation = value; }
	}
	public Territory StagingLocation
	{
		get { return stageLocation; }
		set { stageLocation = value; }
	}	
	
	private void setup()
	{
		LSat lsat = new LSat(attacker); // Doesn't matter whose it is...
		
		battleResolved = false;
		attackingUnits = stageLocation.Friendlies(attacker);
		attackingUnits.AddRange(attacker.getActiveUnitsOfType(lsat));
		
		defendingUnits = battleLocation.Friendlies(defender);
		defendingUnits.AddRange(defender.getActiveUnitsOfType(lsat));
		
		attackingUnitsDictionary = new Dictionary();
		defendingUnitsDictionary = new Dictionary();
				
		foreach (Unit unit in attackingUnits)
			attackingUnitsDictionary.IncValue(unit.Name);
		
		foreach (Unit unit in defendingUnits)
			defendingUnitsDictionary.IncValue(unit.Name);
			
	}

	private void resetBattleAndHide()
	{	
		attackingUnits = defendingUnits = null; // Invalidate lists
		ConventionalBattle.Hide();
	}
	
	private void decideDice() 
	{
		/* Define dice 
		 * Attacker gets 1, Def gets 2, most units gets +1, most lstars gets +1
		 */
		attackerDice = 1;
		defenderDice = 2;

		Console.WriteLine("1Attacker gets " + attackerDice + " dice, defender gets " + defenderDice);
				
		if (attackingUnits.Count > defendingUnits.Count)
			attackerDice++;
		else if (attackingUnits.Count < defendingUnits.Count)
			defenderDice++;

		Console.WriteLine("2Attacker gets " + attackerDice + " dice, defender gets " + defenderDice);
		
		int attackLsat, defendLsat;
		attackLsat = attacker.countActiveUnits(new LSat(attacker));
		defendLsat = defender.countActiveUnits(new LSat(defender));
		
		if (attackLsat > defendLsat)
			attackerDice++;
		else if (attackLsat < defendLsat)
			defenderDice++;		
		
		Console.WriteLine("3Attacker gets " + attackerDice + " dice, defender gets " + defenderDice);
		ConvBattleAttackerDice.Text = attackerDice + " d6's ";
		ConvBattleDefenderDice.Text = defenderDice + " d6's ";	
		
		/* Remove set of supplies */
		Orig_AttackConventionalStart cmd = new Orig_AttackConventionalStart(attacker, defender, fullDefense);
		Game.GetInstance().State.Execute(cmd);
		
	}
	
	public void removeUnits(int akills, int dkills)
	{
		ArrayList list = new ArrayList();
		Orig_AttackDeleteUnits cmd;
		
		for (int i=0; (i<dkills) && (i<attackingUnits.Count); i++)
		{
			if (attackingUnits[i] is TacticalUnit)
		    {
				TacticalUnit unit = (TacticalUnit)attackingUnits[i];
				list.Add(unit);
			}
		}
			
		cmd = new Orig_AttackDeleteUnits(list, attacker, stageLocation);
		Game.GetInstance().State.Execute(cmd);
		
		list.Clear();
		
		for (int i=0; (i<akills) && (i<defendingUnits.Count); i++)
		{
			if (defendingUnits[i] is TacticalUnit)
			{
				TacticalUnit unit = (TacticalUnit)defendingUnits[i];
				list.Add(unit);
			}
		}
		cmd = new Orig_AttackDeleteUnits(list, defender, battleLocation);
		Game.GetInstance().State.Execute(cmd);
		
		battleResolved = true;
	}
	
	public void rollDice()
	{
		RandomDie die = new RandomDie();
		int attack, defend;
		
		attack = die.rollSeveral(attackerDice);
		defend = die.rollSeveral(defenderDice);
		
		attackerKills = attack/3;
		defenderKills = defend/3;

		ConvBattleAttackerDice.Text += "= " + attack + "\n Killed "+ attackerKills; 
		ConvBattleDefenderDice.Text += "= " + defend + "\n Killed "+ defenderKills;
		
		Game.GetInstance().GUI.writeToLog("Battle for " +battleLocation.Name+ ": Attacker ("+attacker.Name+") kills " + attackerKills + " and Defender ("+defender.Name+") kills " + defenderKills + "."); 
		removeUnits(attackerKills, defenderKills);
	}
	
	public void showConventionalBattle()
	{
		if ((battleLocation == null) || (attacker == null) || (defender == null))
			throw new ArgumentException("Battle is not well-defined, has null inputs.");

		setup();
			
		ConvBattleLabel.Text = "Battle for " + battleLocation.Name;
		ConventionalBattle.ShowAll();
	}

	public void on_ConvBattleOkay_clicked(System.Object obj, EventArgs e)
	{
		if (!battleResolved)
		{
			decideDice(); 
			/* Begin the combat */
			rollDice();
		}
	}	

	private void on_CombatView_exposed (object o, ExposeEventArgs args)
	{
/*	[Glade.Widget] Gtk.DrawingArea ConvBattleUnitLegend;
	[Glade.Widget] Gtk.DrawingArea ConvBattleAttackerUnits;
	[Glade.Widget] Gtk.DrawingArea ConvBattleDefenderUnits;
*/

		//const int N = 35;
		int N = 35;
		
		Gdk.Color territoryColor = battleLocation.Color;
						
		battleLocation.MapTerritory.draw(ConvBattleUnitLegend.GdkWindow, 15, 15, 100, 100, territoryColor);
		 
		Gdk.GC textcolor = new Gdk.GC(ConvBattleUnitLegend.GdkWindow);		
		textcolor.RgbFgColor = new Gdk.Color(0,0,0);
		
		if ((attackingUnits == null) || (defendingUnits == null))
			return;

		/* Draw the first N units staggered, then use a label to show further #'s */
	   	for (int offset=0; offset < attackingUnits.Count && offset < N; offset++)
	   	{
	   		((Unit)attackingUnits[offset]).draw(ConvBattleAttackerUnits.GdkWindow, offset*5+50, offset*5);
	   	}

		/* Draw the first N units staggered, then use a label to show further #'s */
	   	for (int offset=0; offset < defendingUnits.Count && offset < N; offset++)
	   	{
	   		((Unit)defendingUnits[offset]).draw(ConvBattleDefenderUnits.GdkWindow, offset*5+50, offset*5);
	   	}
	   	
        Pango.Layout label = new Pango.Layout (ConvBattleUnitLegend.PangoContext);
		label.Wrap = Pango.WrapMode.Word;
		label.FontDescription = FontDescription.FromString ("Tahoma 10");

N = -1;

		label.SetMarkup ( attackingUnitsDictionary.toString() );						
		ConvBattleAttackerUnits.GdkWindow.DrawLayout (textcolor, 25, 40, label);
		
		label.SetMarkup ( defendingUnitsDictionary.toString() );						
		ConvBattleDefenderUnits.GdkWindow.DrawLayout (textcolor, 25, 40, label);		
			
	}

	public void on_ConvBattleCancel_clicked(System.Object obj, EventArgs e)
	{
		if (battleResolved)
			resetBattleAndHide();
	}		

	public void on_ConventionalBattle_delete_event(System.Object obj, EventArgs e)
	{
		if (battleResolved)
			resetBattleAndHide();
	}	

}
}
