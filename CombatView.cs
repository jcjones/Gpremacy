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
	int attackerResult;
	int defenderResult;
	ArrayList attackingUnits;
	ArrayList defendingUnits;
	
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
		ConventionalBattle.ExposeEvent += on_ConventionalBattle_exposed;		
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
	
	private void decideDice() 
	{
		/* Define dice 
		 * Attacker gets 1, Def gets 2, most units gets +1, most lstars gets +1
		 */
		attackerDice = 1;
		defenderDice = 2;

		Console.WriteLine("1Attacker gets " + attackerDice + " dice, defender gets " + defenderDice);
		
		attackingUnits = stageLocation.Friendlies(attacker);
		defendingUnits = battleLocation.Friendlies(defender);
		
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
			TacticalUnit unit = (TacticalUnit)attackingUnits[i];
			list.Add(unit);
		}
			
		cmd = new Orig_AttackDeleteUnits(list, attacker, stageLocation);
		Game.GetInstance().State.Execute(cmd);
		
		list.Clear();
		
		for (int i=0; (i<akills) && (i<defendingUnits.Count); i++)
		{
			TacticalUnit unit = (TacticalUnit)defendingUnits[i];
			list.Add(unit);
		}
		cmd = new Orig_AttackDeleteUnits(list, defender, battleLocation);
		Game.GetInstance().State.Execute(cmd);
		
		battleResolved = true;
		attackingUnits = defendingUnits = null; // Invalidate lists
	}
	
	public void rollDice()
	{
		RandomDie die = new RandomDie();
		int attack, defend;
		int akills, dkills;
		
		attack = die.rollSeveral(attackerDice);
		defend = die.rollSeveral(defenderDice);

		ConvBattleAttackerDice.Text += "= " + attack; 
		ConvBattleDefenderDice.Text += "= " + defend;
		
		akills = attack/3;
		dkills = defend/3;
		
		Console.WriteLine("Attacker kills " + akills + " and Defender kills " + dkills); 
		removeUnits(akills, dkills);
	}
	
	public void showConventionalBattle()
	{
		if ((battleLocation == null) || (attacker == null) || (defender == null))
			throw new ArgumentException("Battle is not well-defined, has null inputs.");
		battleResolved = false;
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
	public void on_ConvBattleCancel_clicked(System.Object obj, EventArgs e)
	{
		if (battleResolved)
			ConventionalBattle.Hide();
	}		

	private void on_ConventionalBattle_exposed (object o, ExposeEventArgs args)
	{
	}

	public void on_ConventionalBattle_delete_event(System.Object obj, EventArgs e)
	{
		if (battleResolved)
			ConventionalBattle.Hide();
	}	

}
}
