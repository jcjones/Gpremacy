// created on 09/19/2005 at 21:42
using System;
using System.Collections;

namespace Gpremacy {
class Orig_Play4Move : State {
	Territory previousTerritory;
	Territory currentTerritory;
	bool arrowOn;

	public Orig_Play4Move (Game game_i) : base(game_i,4,5)
	{
	}
	
	public override string Name()
	{
		return "Phase 4: Movement";
	} 		
	
	public override void beginPlayer(Player p) {
		previousTerritory = currentTerritory = null;
		arrowOn = false;		
	}
	
	public override bool mouseClick(Territory target, uint Button)
	{
		if (Button != 1) 
		{
			/* Cancelling */
			arrowOn = false;
			
			if (previousTerritory != null)
				Game.GUI.redrawTerritory(previousTerritory);

			Game.GUI.redrawTerritory(target);			
			previousTerritory = null;			
			return false;
		}
		System.Console.WriteLine("1");
		if (previousTerritory == null && target.occupiedBy(Game.State.CurrentPlayer))
		{
			System.Console.WriteLine("2");
			/* Picking first selection */
			
			arrowOn = true;
			previousTerritory = target;
		} else if (previousTerritory != null && previousTerritory != target) {
			System.Console.WriteLine("3");
			/* Picking target selection */			
			arrowOn = false;
						
			// Clone the list since otherwise modifying the presently used list is bad   					
			ArrayList tmp = (ArrayList)previousTerritory.Units.Clone();

			if (target.IsLand == previousTerritory.IsLand)
			{
				System.Console.WriteLine("4");
				ArrayList moveCost;								
				
				// move units from previousTerritory to target.
				foreach (TacticalUnit unit in tmp)
				{
					// Print diagnostics
					if (unit.Owner == Game.State.CurrentPlayer && unit.canMoveTo(target))
					{
						/* If the unit returns an ArrayList with 
				 		* more than one element, let the user pick */
						moveCost = unit.calculateMovementCost(target);
						
						if (!Game.GetInstance().hasSufficientWeath(Game.State.CurrentPlayer, moveCost, 0))
						{
							break; 
						}
						
						if (Game.GetInstance().GUI.AlwaysMarch && moveCost.Count > 1)
						{
							/* Just march... Get the move cost for a minimal move */
							moveCost = unit.calculateMovementCost(previousTerritory);
							/* Execute */
							Orig_MoveUnit cmd_always = new Orig_MoveUnit(Game.State.CurrentPlayer, unit, target, previousTerritory, moveCost);
							Game.State.Execute(cmd_always);
							return true;
						}						
						
						Orig_MoveUnit cmd = new Orig_MoveUnit(Game.State.CurrentPlayer, unit, target, previousTerritory, moveCost);												
												
						if (moveCost.Count > 1)
						{							
							/* Data packet for the GUI to use */
							ArrayList t = new ArrayList();
							t.Insert(0, moveCost);
							t.Insert(1, cmd);
							data=t;

							Game.GUI.showMoveOptions(previousTerritory, target);
						} else {
							Game.State.Execute(cmd);
						}												   						
						break;
					}
				}
			} else {
				System.Console.WriteLine("5");
				/* Diseperate land/sea areas, so we should load/unload! */
				Territory land, sea;
				
				if (target.IsLand) { /* Order options properly */
					land = target; sea = previousTerritory;
				} else {
					sea = target; land = previousTerritory;
				}
				
				/* Confirm that there is a friendly ship in sea */
				if (!sea.occupiedBy(Game.State.CurrentPlayer))
				{
					System.Console.WriteLine("No Ship Here in " + sea.Name + "!" + sea.Units.Count);
					foreach(TacticalUnit u in sea.Units)
					{
						System.Console.Write("[T" + u.Name + " O: " + u.Owner.Name + "] ");
					} 
					/* No ships! */
					return false;
				}

				/* Confirm that they're adjacent */
				if (Game.GUI.Map.distanceBetween(land, sea) > 1) 
					return false;				
				
				/* Don't allow an unload into an occupied territory */
				if (!land.occupiable(Game.GetInstance().State.CurrentPlayer))
					return false;				
				
				ArrayList b = new ArrayList();				 
				b.Insert(0,sea.Friendlies(Game.State.CurrentPlayer));
				b.Insert(1,land.Friendlies(Game.State.CurrentPlayer));
				b.Insert(0,land);
				b.Insert(1,sea);
				data = b;
				
				Game.GUI.showLoadNavalOptions(land, sea);
				
			}
			System.Console.WriteLine("6");

			previousTerritory = null;
		}
		System.Console.WriteLine("7");
		return true;   			
	}
	
	public override bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		if (arrowOn && previousTerritory != null && curTerr != null && curTerr != currentTerritory)
		{
			currentTerritory = curTerr;
			System.Console.WriteLine("Arrow from " + previousTerritory.Name + " to " + curTerr.Name);
			Game.GUI.drawArrow(previousTerritory, curTerr);
			return true;
		}
		return false;		
	}

}

[Serializable]
class Orig_MoveUnit : Command {
	TacticalUnit unit;
	string nextName, previousName;
	Player curPlay;
	
	public ArrayList moveCost; // allow GUI to set this value.

	public Orig_MoveUnit(Player p, TacticalUnit aunit, Territory anext, Territory aprevious, ArrayList acost) {
		curPlay = p;
		unit = aunit;
		
		nextName = anext.Name; 
		previousName = aprevious.Name;
		
		moveCost = acost;
		undoable = true;
	}
	
	public override void Execute(Game game) {
		curPlay = game.GetLocalCopyOfPlayer(curPlay);
		unit = (TacticalUnit)game.GetLocalCopyOfUnit(unit);
		
		Territory next = game.TerritoryByName(nextName);
		Territory previous = game.TerritoryByName(previousName);
		
		/* Confirm we can move here ... don't bother for unexecute */
		if (unit.canMoveTo(next)) 
		{
			/* Claim territory */
			Player oldOwner = next.Owner;
			next.Owner = curPlay;			
			oldOwner.updateResourceCards(); // deactivate
			/* If owner has this resource card and it's never been active before, notify */
			foreach (ResourceCard card in curPlay.ResourceCards)
			{
				if (card.isResource() && card.Place == next && !card.HasBeenActive)
					game.GUI.writeToLog("Resource card " + card.FlavorText + " now available.");
			}
						
			/* Charge Movement Cost */
			foreach (Stock s in moveCost)
			{
				curPlay.changeResourceStockpile(s);
			}
			
			System.Console.WriteLine("Moving unit " + unit.toString() );

			previous.removeUnit(unit);
			next.addUnit(unit);			
			unit.moveTo(next);							
		}	
		
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
	public override void Unexecute(Game game) {
		curPlay = game.GetLocalCopyOfPlayer(curPlay);
		unit = (TacticalUnit)game.GetLocalCopyOfUnit(unit);
		
		Territory next = game.TerritoryByName(nextName);
		Territory previous = game.TerritoryByName(previousName);
	
		next.removeUnit(unit);
		previous.addUnit(unit);			
		unit.moveTo(previous);
		
		/* Refund Movement Cost */
		foreach (Stock s in moveCost)
		{
			Stock a = new Stock(s.Good, -1*s.Number);
			curPlay.changeResourceStockpile(a);
		}
			
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
}

[Serializable]
class Orig_LoadUnits : Command {
	Navy ship;
	ArrayList troops;
	string portName, seaName;
	
	public Orig_LoadUnits(Navy aship, ArrayList atroops, Territory aport, Territory asea)
	{
		ship = aship;
		troops = atroops;
		portName = aport.Name;
		seaName = asea.Name;
		undoable = true;
	}
	
	public override void Execute(Game game)
	{
		Territory port = game.TerritoryByName(portName);
		Territory sea = game.TerritoryByName(seaName);	
		ship = (Navy)game.GetLocalCopyOfUnit(ship);
	
		foreach(TacticalUnit joe in troops)
		{
			TacticalUnit bob = (TacticalUnit)game.GetLocalCopyOfUnit(joe);
			
			ship.loadUnit(bob);
			port.removeUnit(bob);
			bob.moveTo(ship.Hold);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);
		}
	}
	
	public override void Unexecute(Game game)
	{
		Territory port = game.TerritoryByName(portName);
		Territory sea = game.TerritoryByName(seaName);
		ship = (Navy)game.GetLocalCopyOfUnit(ship);
			
		foreach(TacticalUnit joe in troops)
		{
			TacticalUnit bob = (TacticalUnit)game.GetLocalCopyOfUnit(joe);

			ship.unloadUnit(bob);
			port.addUnit(bob);
			bob.moveTo(port);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);
		}
	}
}

[Serializable]
class Orig_UnloadUnits : Command {
	Navy ship;
	ArrayList troops;
	string portName, seaName;
	
	public Orig_UnloadUnits(Navy aship, ArrayList atroops, Territory aport, Territory asea)
	{
		ship = aship;
		troops = atroops;
		portName = aport.Name;
		seaName = asea.Name;
		undoable = true;
	}
	
	public override void Execute(Game game)
	{
		Territory port = game.TerritoryByName(portName);
		Territory sea = game.TerritoryByName(seaName);
		ship = (Navy)game.GetLocalCopyOfUnit(ship);
	
		foreach(TacticalUnit joe in troops)
		{
			TacticalUnit bob = (TacticalUnit)game.GetLocalCopyOfUnit(joe);
			
			ship.unloadUnit(bob);
			port.addUnit(bob);
			bob.moveTo(port);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);			
		}
	}
	
	public override void Unexecute(Game game)
	{
		Territory port = game.TerritoryByName(portName);
		Territory sea = game.TerritoryByName(seaName);
		ship = (Navy)game.GetLocalCopyOfUnit(ship);
	
		foreach(TacticalUnit joe in troops)
		{
			TacticalUnit bob = (TacticalUnit)game.GetLocalCopyOfUnit(joe);
			
			ship.loadUnit(bob);
			port.removeUnit(bob);
			bob.moveTo(ship.Hold);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);			
		}
	}
}
}