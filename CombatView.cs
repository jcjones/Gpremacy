// created on 10/20/2005 at 11:39
using System;
using System.Collections;
using Gtk;
using Pango;
using Gdk;

namespace Gpremacy {

class CombatView {
	[Glade.Widget] Gtk.Window MainWindow;

	/* Conventional Battle */	
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
	const int HitCutOff = 6; // 1 - 5 == hit, 6 == miss
	
	ArrayList attackingUnits; // Of Unit
	ArrayList defendingUnits; // Of Unit 
	
	ArrayList strategicTargets; // Of Territory (for strategic attack)
	ArrayList strategicTargetsWithCounts; // Of NuclearTarget
	
	Dictionary attackingUnitsDictionary;
	Dictionary defendingUnitsDictionary;
	
	bool battleResolved;
	bool defenderReady;
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
		defenderReady = false;
		attackingUnits = stageLocation.Friendlies(attacker);
		attackingUnits.AddRange(attacker.getActiveUnitsOfType(lsat));
		
		defendingUnits = battleLocation.Friendlies(defender);
		defendingUnits.AddRange(defender.getActiveUnitsOfType(lsat));
		
		ConvBattleAttackerDice.Text = "";
		ConvBattleDefenderDice.Text = "";
		
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
		
		strategicTargets.Clear();
		
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
		defenderReady = false;		
	}
		
	private void conventionalDecideDice() 
	{
		if (attacker == null)
			return;
		/* Define dice 
		 * Attacker gets 1, Def gets 2, most units gets +1, most lstars gets +1
		 */
		attackerDice = 1;
		defenderDice = 1;
		
		if (fullDefense)
			defenderDice++;

		/* Now we see who has the most units in battle. We discount 
		 * any Strategic units present, because this is all about
		 * the ground warfare. */
		int atkCount = 0, defCount = 0;
		foreach (Unit u in attackingUnits)
			if (u is TacticalUnit)
				atkCount++;
		foreach (Unit u in defendingUnits)
			if (u is TacticalUnit)
				defCount++;				
						
		if (atkCount > defCount)
			attackerDice++;
		else if (atkCount < defCount)
			defenderDice++;
		
		int attackLsat, defendLsat;
		attackLsat = attacker.countActiveUnits(new LSat(attacker));
		defendLsat = defender.countActiveUnits(new LSat(defender));
		
		if (attackLsat > defendLsat)
			attackerDice++;
		else if (attackLsat < defendLsat)
			defenderDice++;		
		
		ConvBattleAttackerDice.Text = attackerDice + " d6's ";
		ConvBattleDefenderDice.Text = defenderDice + " d6's ";					
	}
	
	public void removeUnits(int akills, int dkills)
	{
		ArrayList attackerDead = new ArrayList(); // of TacticalUnit
		ArrayList defenderDead = new ArrayList(); // of TacticalUnit
		Orig_AttackDeleteUnits cmd;
		
		for (int i=0; (i<dkills) && (i<attackingUnits.Count); i++)
		{
			if (attackingUnits[i] is TacticalUnit)
		    {
				TacticalUnit unit = (TacticalUnit)attackingUnits[i];
				attackerDead.Add(unit);
			}
		}
			
		cmd = new Orig_AttackDeleteUnits(attackerDead, attacker, stageLocation);
		
		System.Console.Write("AtkDelete: " + attacker.Name + " - " + stageLocation.Name);
		foreach(TacticalUnit u in attackerDead)
			System.Console.Write(u.Name + "[" + u.ID + "],");			
		System.Console.WriteLine();
				 
		Game.GetInstance().State.Execute(cmd);
		
		
		
		for (int i=0; (i<akills) && (i<defendingUnits.Count); i++)
		{
			if (defendingUnits[i] is TacticalUnit)
			{
				TacticalUnit unit = (TacticalUnit)defendingUnits[i];
				defenderDead.Add(unit);
			}
		}
		cmd = new Orig_AttackDeleteUnits(defenderDead, defender, battleLocation);
		System.Console.Write("DefDelete: " + defender.Name + " - " + battleLocation.Name);
		foreach(TacticalUnit u in defenderDead)
			System.Console.Write(u.Name + "[" + u.ID + "],");			
		System.Console.WriteLine();
		
		Game.GetInstance().State.Execute(cmd);
		
		battleResolved = true;
	}
	
	public void conventionalRollDice()
	{
		RandomDie die = new RandomDie();
		int attack, defend;
		
		attack = die.rollSeveral(attackerDice);
		defend = die.rollSeveral(defenderDice);
		
		attackerKills = attack/3;
		defenderKills = defend/3;

		Orig_AttackConventionalRoll cmd = new Orig_AttackConventionalRoll(attacker, defender, battleLocation, attackerDice, defenderDice, attack, defend);
		Game.GetInstance().State.Execute(cmd);
		
		removeUnits(attackerKills, defenderKills);
	}
	
	public void conventionalDiceResults(int attackerDice, int defenderDice, int attackerRoll, int defenderRoll)
	{
		attackerKills = attackerRoll/3;
		defenderKills = defenderRoll/3;
			
		ConvBattleAttackerDice.Text = attackerDice + "d6 = " + attackerRoll + "\n Killed "+ attackerKills; 
		ConvBattleDefenderDice.Text = defenderDice + "d6 = " + defenderRoll + "\n Killed "+ defenderKills;
				
		 battleResolved = true;
		 ConvBattleOkay.Label = "Close Battle";
		 ConvBattleOkay.Sensitive = true;
		 ConvBattleCancel.Sensitive = false;
	}
	
	public void showConventionalBattle()
	{
		if ((battleLocation == null) || (stageLocation == null) || (attacker == null) || (defender == null))
			throw new ArgumentException("Battle is not well-defined, has null inputs.");

		setupConventional();
		
		if (Game.GetInstance().LocalPlayers.Contains(attacker)) 
		{
			ConvBattleOkay.Sensitive = false;
			ConvBattleCancel.Sensitive = true;
			ConvBattleFullDefense.Sensitive = false;
			ConvBattleOkay.Label = "Begin Attack";		
		} 
		else if (Game.GetInstance().LocalPlayers.Contains(defender)) 
		{
			ConvBattleCancel.Sensitive = false;
			ConvBattleOkay.Sensitive = true;				
			ConvBattleOkay.Label = "Prepare Defense";
			
			/* Only allow a full defense iff we have the resources */
			if ((defender.getStockpileAmount(new Oil()) > 0) &&
				(defender.getStockpileAmount(new Grain()) > 0) &&
				(defender.getStockpileAmount(new Minerals()) > 0) )
			{
				ConvBattleFullDefense.Sensitive = true;
			}
			else
			{
				ConvBattleFullDefense.Sensitive = false;
				conventionalDefenderReady(false);											
			}
		}
			
		ConvBattleLabel.Text = "Battle for " + battleLocation.Name;
		conventionalDecideDice();
		ConventionalBattle.ShowAll();
	}
	
	public void conventionalDefenderReady(bool fd)	
	{
		ConvBattleFullDefense.Active = fd;
		fullDefense = fd;
		defenderReady = true;
		if (Game.GetInstance().LocalPlayers.Contains(attacker)) 
		{
			ConvBattleOkay.Sensitive = true;
		}
		conventionalDecideDice();
	}
	
	private void on_ConvBattleFullDefense_toggled(System.Object obj, EventArgs e)
	{
		fullDefense = ConvBattleFullDefense.Active;
		conventionalDecideDice();
	}	

	public void on_ConvBattleOkay_clicked(System.Object obj, EventArgs e)
	{
		if (!battleResolved)
		{
			if (defenderReady && Game.GetInstance().LocalPlayers.Contains(attacker))
			{
				conventionalDecideDice();
				
		 		/* Remove set of supplies */
				Orig_AttackConventionalSupplies cmd = new Orig_AttackConventionalSupplies(attacker, defender, fullDefense);
				Game.GetInstance().State.Execute(cmd);

				/* Begin the combat */
				conventionalRollDice();
			} else if (Game.GetInstance().LocalPlayers.Contains(defender))
			{
				Orig_AttackConventionalDefenderReady cmd = new Orig_AttackConventionalDefenderReady(attacker, defender, fullDefense);
				Game.GetInstance().State.Execute(cmd);
			}
		} else {
			resetBattleAndHide();
		}
	}
	
	private void on_ConventionalBattle_exposed (object o, ExposeEventArgs args)
	{

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

	public void on_ConventionalBattle_delete_event(System.Object obj, DeleteEventArgs e)
	{
		e.RetVal = !battleResolved; // True == do not allow delete
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
		battleResolved = false;
		
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

	public void on_StrategicTargetSelection_delete_event(System.Object obj, DeleteEventArgs e)
	{
			e.RetVal = true;
			resetStrategicTargetSelectionAndHide();
	}
	
	/* Strategic Battle */
	
	public void showStrategicBattle(ArrayList stratTargets, Player you)
	{
		defender = you;
		
		LSat lsat = new LSat(defender);
		strategicTargetsWithCounts = stratTargets;
		
		int numLstars = defender.getActiveUnitAmount(lsat);
		int countLstars = 0;
		
		StrategicBattleLabel.Text = attacker.Name + "\nhas launched a\nstrategic attack.";
		
		foreach(Widget wid in StrategicBattleTable)
			StrategicBattleTable.Remove(wid);
		
		uint row = 0;
		strategicTargetsWithCounts.Reverse();
		foreach (NuclearTarget target in strategicTargetsWithCounts)
		{
			Gtk.Label label = new Gtk.Label(target.territory.Name);
			Gtk.Label numIncoming = new Gtk.Label(target.icbms.ToString());
			Gtk.SpinButton lstarsToFire = new Gtk.SpinButton(0.0, (double)numLstars+0.1f, 1.0);
			
			lstarsToFire.ValueChanged += on_StrategicBattle_spun;
			
			StrategicBattleTable.Attach(label, 0, 1, row, row+1);
			StrategicBattleTable.Attach(numIncoming, 1, 2, row, row+1);
			StrategicBattleTable.Attach(lstarsToFire, 2, 3, row, row+1);
			
			/* Auto-populate defensive fields */
			if (target.territory.Owner == defender)
			{
				while ((countLstars < numLstars) && (lstarsToFire.Value < target.icbms + 1))
				{
					lstarsToFire.Value += 1.0;
					countLstars++;
				}				
			}
			
			row++; 			
		}
	
		StrategicBattle.ShowAll();
	}
	
	private void on_StrategicBattle_spun(object o, EventArgs args)
	{
		LSat lsat = new LSat(defender);
		int numLstars = defender.getActiveUnitAmount(lsat);
		int countLstars = 0;
		
		foreach(Widget wid in StrategicBattleTable)
		{		
			if (!(wid is Gtk.SpinButton))
				continue;
				
			countLstars += (int)((Gtk.SpinButton)wid).Value;
		}
		foreach(Widget wid in StrategicBattleTable)
		{
			if (!(wid is Gtk.SpinButton))
				continue;		
			if (countLstars < numLstars)
				((Gtk.SpinButton)wid).SetRange(0.0, (double)numLstars);
			else		
				((Gtk.SpinButton)wid).SetRange(0.0, ((Gtk.SpinButton)wid).Value);							

		}
		
		/* Disable okay button if we've exceeded the count somehow */		
		StrategicBattleOkay.Sensitive = (countLstars <= numLstars);							
	}
	
	private void strategicRollDice(NuclearTarget target, int defendingLSats)
	{
		RandomDie die = new RandomDie();	
		Game game = Game.GetInstance();
		// Defender rolls 1d / L-Star. A 1-5 destroys a nuke, a 6 misses
		
		if (defendingLSats > 0)
		{
			string result = "L-Sat Screen of " + defendingLSats + " firing in defense of " + target.territory.Name + " against " + target.icbms + " ICBMs: ";
			for (int i=0; i<defendingLSats; i++)
			{
				int roll = die.roll();
				result += roll.ToString() + ", ";
				if (roll < HitCutOff && target.icbms > 0)
				{
					target.icbms--;
				}
			}
			game.GUI.writeToLog(result);
		}
		
		System.Console.WriteLine("Remaining Nukes: " + target.icbms);
		
		if (target.icbms > 0) 
		{		
			Orig_AttackStrategicDetonate cmd = new Orig_AttackStrategicDetonate(target.territory);
			game.State.Execute(cmd);
		}
			
	}
	
	private void resetStrategicBattleAndHide()
	{
		StrategicBattle.Hide(); // Don't allow more exposes to muck things up
		resetStrategicTargetSelectionAndHide();
	}
	
	private void on_StrategicBattleOkay_clicked(System.Object obj, EventArgs e)
	{
		if (battleResolved)
		{
			resetStrategicBattleAndHide();
			return;	
		}
		
		int i = strategicTargetsWithCounts.Count;
		foreach(Gtk.Widget wid in StrategicBattleTable)
		{
			if (! (wid is Gtk.SpinButton))
				continue;
				
			NuclearTarget target = (NuclearTarget)strategicTargetsWithCounts[--i];
			strategicRollDice(target, (int)((Gtk.SpinButton)wid).Value);				
		}			
		
		battleResolved = true;		
	}
				
	private void on_StrategicBattle_delete_event(System.Object obj, DeleteEventArgs e)
	{
		e.RetVal = !battleResolved; // True == do not allow delete
		if (battleResolved)
			resetStrategicTargetSelectionAndHide();	
	}
	
	private void on_StrategicBattle_exposed(System.Object obj, ExposeEventArgs e)
	{
		const int N = 35; 
		if (attackingUnits == null)
			return;
			
	   	for (int offset=0; offset < attackingUnits.Count && offset < N; offset++)
	   	{
	   		((Unit)attackingUnits[offset]).draw(StrategicBattleLegend.GdkWindow, offset*8+10, 10);
	   	}
	}


}
}
