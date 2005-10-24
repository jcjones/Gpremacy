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
	[Glade.Widget] Gtk.CheckButton ConvBattleFullDefense;

	/* Strategic Target Selection */
	[Glade.Widget] Gtk.Window StrategicTargetSelection;
	[Glade.Widget] Gtk.DrawingArea StrategicLegend;
	[Glade.Widget] Gtk.Label StrategicTargetArsenal;
	[Glade.Widget] Gtk.Table StrategicTargetTable;
	[Glade.Widget] Gtk.Button StrategicTargetOkay;
	[Glade.Widget] Gtk.Button StrategicTargetCancel;
	
	/* Strategic Battle */
	[Glade.Widget] Gtk.Window StrategicBattle;
	[Glade.Widget] Gtk.DrawingArea StrategicBattleLegend;
	[Glade.Widget] Gtk.Label StrategicBattleLabel;
	[Glade.Widget] Gtk.Table StrategicBattleTable;
	[Glade.Widget] Gtk.Button StrategicBattleOkay;
	
	Territory stageLocation;
	Territory battleLocation;
	Player attacker;
	Player defender;
	int attackerDice;
	int defenderDice;
	int attackerKills;
	int defenderKills;
	
	int numberOfICBMs;
	
	ArrayList attackingUnits; // Of Unit
	ArrayList defendingUnits; // Of Unit 
	
	ArrayList strategicTargets; // Of Territory (for strategic attack)
	ArrayList strategicTargetsWithCounts; // Of NuclearTarget
	
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
		gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "StrategicTargetSelection", null);
		gxml.Autoconnect (this);
		gxml = new Glade.XML (SupportFileLoader.locateGameFile("gpremacy_gui/gpremacy_gui.glade"), "StrategicBattle", null);
		gxml.Autoconnect (this);

		attacker = defender = null;
		battleLocation = stageLocation = null;
		
		battleResolved = false;
		fullDefense = true;
		
		strategicTargets = new ArrayList();
		strategicTargetsWithCounts = new ArrayList();

		/* These windows are modal */		
		ConventionalBattle.Modal = true;
		ConventionalBattle.TransientFor = MainWindow;
		StrategicBattle.Modal = true;
		StrategicBattle.TransientFor = MainWindow;
		
		ConventionalBattle.SetSizeRequest(400,300);
		StrategicTargetSelection.SetSizeRequest(400,300);
		StrategicBattle.SetSizeRequest(400,300);
		
		ConvBattleOkay.Clicked += on_ConvBattleOkay_clicked;
		ConvBattleCancel.Clicked += on_ConvBattleCancel_clicked;
		ConvBattleFullDefense.Toggled += on_ConvBattleFullDefense_toggled;

		StrategicTargetOkay.Clicked += on_StrategicTargetOkay_clicked;
		StrategicTargetCancel.Clicked += on_StrategicTargetCancel_clicked;
		
		StrategicBattleOkay.Clicked += on_StrategicBattleOkay_clicked;
		
		ConventionalBattle.DeleteEvent += on_ConventionalBattle_delete_event;
		ConventionalBattle.ExposeEvent += on_ConventionalBattle_exposed;
				
		StrategicTargetSelection.DeleteEvent += on_StrategicTargetSelection_delete_event;
		StrategicTargetSelection.ExposeEvent += on_StrategicTargetSelection_exposed;
				
		StrategicBattle.DeleteEvent += on_StrategicBattle_delete_event;
		StrategicBattle.ExposeEvent += on_StrategicBattle_exposed;
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
		
	private void setupConventional()
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
	
	private void setupStrategic()
	{
		Nuke nuke = new Nuke(attacker);
		//LSat lsat = new LSat(attacker); // Doesn't matter whose it is...
		
		attackingUnits = attacker.getActiveUnitsOfType(nuke);
		//defendingUnits = defender.getActiveUnitsOfType(lsat);
	}

	private void resetBattleAndHide()
	{
		ConventionalBattle.Hide(); // Hide first as to not crash on expose()

		attacker = defender = null; // Invalidate Players	
		attackingUnits = defendingUnits = null; // Invalidate lists
		ConvBattleFullDefense.Active = true;
		fullDefense = true;		
	}
		
	private void decideDice() 
	{
		/* Define dice 
		 * Attacker gets 1, Def gets 2, most units gets +1, most lstars gets +1
		 */
		attackerDice = 1;
		defenderDice = 1;
		
		if (fullDefense)
			defenderDice++;

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

		setupConventional();
			
		ConvBattleLabel.Text = "Battle for " + battleLocation.Name;
		ConventionalBattle.ShowAll();
	}
	
	private void on_ConvBattleFullDefense_toggled(System.Object obj, EventArgs e)
	{
		fullDefense = ConvBattleFullDefense.Active;
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
	
	private void on_ConventionalBattle_exposed (object o, ExposeEventArgs args)
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

	/* Strategic Target Selection */
	public void showStrategicTargetSelection()
	{
		if (attacker == null)
			throw new ArgumentException("Battle is not well-defined, has null inputs.");

		setupStrategic();
			
		numberOfICBMs = attackingUnits.Count;
		
		StrategicTargetArsenal.Text = "Strategic Arsenal: " + numberOfICBMs + " ICBMs";
		
		foreach(Widget wid in StrategicTargetTable)
			StrategicTargetTable.Remove(wid);
		
		uint row = 0;
		foreach(Territory target in strategicTargets)
		{
			AddStrategicTargetToArray(target);
						
			row++;			
		}
				
		StrategicTargetSelection.ShowAll();
	}
	
	public void on_StrategicTargetSelection_spun(object o, EventArgs args)
	{
		int count = 0;
		foreach(Widget wid in StrategicTargetTable)
		{
			if (wid is Gtk.SpinButton)
				count += (int)((Gtk.SpinButton)wid).Value;				
		}
		
		foreach(Widget wid in StrategicTargetTable)
		{
			if (wid is Gtk.SpinButton)
				if (count < numberOfICBMs)
					((Gtk.SpinButton)wid).SetRange(0.0, (double)numberOfICBMs);
				else		
					((Gtk.SpinButton)wid).SetRange(0.0, ((Gtk.SpinButton)wid).Value);							
		}

		/* Disable okay button if we've exceeded the count somehow */		
		StrategicTargetOkay.Sensitive = (count <= numberOfICBMs);		
	}

	public void AddStrategicTargetToArray(Territory target)
	{
		Gtk.Label label = new Gtk.Label(target.Name);
		Gtk.SpinButton spinbox = new Gtk.SpinButton(0.0,(double)numberOfICBMs,1.0);
			
		spinbox.ValueChanged += on_StrategicTargetSelection_spun;
			
		uint row = StrategicTargetTable.NRows;
				
		StrategicTargetTable.Attach(label, 0, 1, row, row+1);
		StrategicTargetTable.Attach(spinbox, 1, 2, row, row+1);
		
		StrategicTargetTable.ShowAll();
	}

	public void AddStrategicTarget(Territory target)
	{
		if (strategicTargets.Contains(target))
			return;
		strategicTargets.Add(target);
		AddStrategicTargetToArray(target);
	}

	public void on_StrategicTargetSelection_exposed(object o, ExposeEventArgs args)
	{
		const int N = 35; 
	   	for (int offset=0; offset < attackingUnits.Count && offset < N; offset++)
	   	{
	   		((Unit)attackingUnits[offset]).draw(StrategicLegend.GdkWindow, offset*8+10, 10);
	   	}

	}


	public void resetStrategicTargetSelectionAndHide()
	{
		StrategicTargetSelection.Hide(); // Hide first as to not crash on expose()

		attacker = defender = null; // Invalidate Players	
		attackingUnits = defendingUnits = null; // Invalidate lists
		
		numberOfICBMs = 0;
		strategicTargets.Clear();
		strategicTargetsWithCounts.Clear();
		
		State curState = Game.GetInstance().State.CurrentState;
		if (curState is Orig_Play3Attack)
			((Orig_Play3Attack)curState).doneSelectingStrategicTargets();		
	}

	public void on_StrategicTargetOkay_clicked(System.Object obj, EventArgs e)
	{
		State curState = Game.GetInstance().State.CurrentState;
		if (curState is Orig_Play3Attack)
			((Orig_Play3Attack)curState).doneSelectingStrategicTargets();
			
		Territory terr;
		int i = strategicTargets.Count;
		
		int count = 0;
		
		strategicTargetsWithCounts.Clear();

		foreach(Widget wid in StrategicTargetTable)
		{
			if (wid is Gtk.SpinButton)
			{
				terr = (Territory)strategicTargets[--i];
				count += (int)((Gtk.SpinButton)wid).Value;
				Console.WriteLine("Territory " + terr.Name + " has " + ((Gtk.SpinButton)wid).Value);
				NuclearTarget tgt = new NuclearTarget(terr, (int)((Gtk.SpinButton)wid).Value);				
				strategicTargetsWithCounts.Add(tgt);
			}			
		}
		
		if (count > numberOfICBMs)
		{
			// Bad!
			Game.GetInstance().GUI.ShowWarning("You are attempting to launch more ICBMs than you have.");
			return;
		}
		
		/* Open Launch Windows*/
		StrategicTargetSelection.Hide();
		
		ArrayList defenders = new ArrayList();
		foreach (NuclearTarget tgt in strategicTargetsWithCounts)
		{
			if (!defenders.Contains(tgt.territory.Owner))
				defenders.Add(tgt.territory.Owner);
		}
		Orig_AttackStrategicStart cmd = new Orig_AttackStrategicStart(attacker, defenders, strategicTargetsWithCounts);
		Game.GetInstance().State.Execute(cmd);
		
	}	
		
	public void on_StrategicTargetCancel_clicked(System.Object obj, EventArgs e)
	{
			resetStrategicTargetSelectionAndHide();
	}	

	public void on_StrategicTargetSelection_delete_event(System.Object obj, EventArgs e)
	{
			resetStrategicTargetSelectionAndHide();
	}
	
	/* Strategic Battle */
	
	public void showStrategicBattle(ArrayList stratTargets, Player you)
	{
		LSat lsat = new LSat(you);
		strategicTargetsWithCounts = stratTargets;
		
		int numLstars = you.getActiveUnitAmount(lsat);
		
		StrategicBattleLabel.Text = attacker.Name + " has launched a strategic attack.";
		
		foreach(Widget wid in StrategicBattleTable)
			StrategicBattleTable.Remove(wid);
		
		uint row = 0;
		strategicTargetsWithCounts.Reverse();
		foreach (NuclearTarget target in strategicTargetsWithCounts)
		{
			Gtk.Label label = new Gtk.Label(target.territory.Name);
			Gtk.Label numIncoming = new Gtk.Label(target.icbms.ToString());
			Gtk.SpinButton lstarsToFire = new Gtk.SpinButton(0.0, (double)numLstars, 1.0);
			
			StrategicBattleTable.Attach(label, 0, 1, row, row+1);
			StrategicBattleTable.Attach(numIncoming, 1, 2, row, row+1);
			StrategicBattleTable.Attach(lstarsToFire, 2, 3, row, row+1);			
		}
	
		StrategicBattle.ShowAll();
	}
	
	public void resetStrategicBattleAndHide()
	{
		resetStrategicTargetSelectionAndHide();
		StrategicBattle.Hide();
	}
	
	public void on_StrategicBattleOkay_clicked(System.Object obj, EventArgs e)
	{
		foreach(NuclearTarget target in strategicTargetsWithCounts)
		{
			System.Console.WriteLine("Nuking " + target.territory.Name + "...");
			Orig_AttackStrategicDetonate cmd = new Orig_AttackStrategicDetonate(target.territory);
			Game.GetInstance().State.Execute(cmd);
		}
		battleResolved = true;
	}
				
	public void on_StrategicBattle_delete_event(System.Object obj, EventArgs e)
	{
		if (battleResolved)
			resetStrategicTargetSelectionAndHide();	
	}
	
	public void on_StrategicBattle_exposed(System.Object obj, ExposeEventArgs e)
	{
		const int N = 35; 
	   	for (int offset=0; offset < attackingUnits.Count && offset < N; offset++)
	   	{
	   		((Unit)attackingUnits[offset]).draw(StrategicBattleLegend.GdkWindow, offset*8+10, 10);
	   	}
	}


}
}
