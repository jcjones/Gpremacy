// created on 09/19/2005 at 21:42
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
		
		if (previousTerritory == null && target.occupiedBy(Game.State.CurrentPlayer))
		{
			/* Picking first selection */
			
			arrowOn = true;
			previousTerritory = target;
		} else if (previousTerritory != null && previousTerritory != target) {
			/* Picking target selection */			
			arrowOn = false;
						
			// Clone the list since otherwise modifying the presently used list is bad   					
			ArrayList tmp = (ArrayList)previousTerritory.Units.Clone();

			if (target.IsLand == previousTerritory.IsLand)
			{
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
						
						Orig_MoveUnit cmd = new Orig_MoveUnit(unit, target, previousTerritory, moveCost);												
												
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
				if (Game.GUI.Map.distanceBetween(land, sea) > 1) {
					return false;
				}				
				
				ArrayList b = new ArrayList();				 
				b.Insert(0,sea.Friendlies(Game.State.CurrentPlayer));
				b.Insert(1,land.Friendlies(Game.State.CurrentPlayer));
				b.Insert(0,land);
				b.Insert(1,sea);
				data = b;
				
				Game.GUI.showLoadNavalOptions(land, sea);
				
			}

			previousTerritory = null;
		}
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
class Orig_MoveUnit : Command {
	TacticalUnit unit;
	Territory next, previous;
	public ArrayList moveCost; // allow GUI to set this value.

	public Orig_MoveUnit(TacticalUnit aunit, Territory anext, Territory aprevious, ArrayList acost) {
		unit = aunit; next = anext; previous = aprevious;
		moveCost = acost;
		undoable = true;
	}
	
	public override void Execute(Game game) {		
		/* Confirm we can move here ... don't bother for unexecute */
		if (unit.canMoveTo(next)) 
		{
			/* Charge Movement Cost */
			foreach (Stock s in moveCost)
			{
				/* If charging multiple stocks, this will possibly subtract part of a cost set */
				if (game.State.CurrentPlayer.getStockpileAmount(s.Good) < -1*s.Number)
				{
					game.GUI.ShowError("Not enough " + s.Good.Name);
					return;
				}
				game.State.CurrentPlayer.changeResourceStockpile(s);
			}

			previous.removeUnit(unit);
			next.addUnit(unit);			
			unit.moveTo(next);							
		}	
		
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
	public override void Unexecute(Game game) {
		next.removeUnit(unit);
		previous.addUnit(unit);			
		unit.moveTo(previous);
		
		/* Refund Movement Cost */
		foreach (Stock s in moveCost)
		{
			Stock a = new Stock(s.Good, -1*s.Number);
			game.State.CurrentPlayer.changeResourceStockpile(a);
		}
			
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
}
class Orig_LoadUnits : Command {
	Navy ship;
	ArrayList troops;
	Territory port;
	Territory sea;
	public Orig_LoadUnits(Navy aship, ArrayList atroops, Territory aport, Territory asea)
	{
		ship = aship;
		troops = atroops;
		port = aport;
		sea = asea;
		undoable = true;
	}
	
	public override void Execute(Game game)
	{
		foreach(TacticalUnit joe in troops)
		{
			ship.loadUnit(joe);
			port.removeUnit(joe);
			joe.moveTo(ship.Hold);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);
		}
	}
	
	public override void Unexecute(Game game)
	{
		foreach(TacticalUnit joe in troops)
		{
			ship.unloadUnit(joe);
			port.addUnit(joe);
			joe.moveTo(port);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);
		}
	}
}
class Orig_UnloadUnits : Command {
	Navy ship;
	ArrayList troops;
	Territory port;
	Territory sea;
	public Orig_UnloadUnits(Navy aship, ArrayList atroops, Territory aport, Territory asea)
	{
		ship = aship;
		troops = atroops;
		port = aport;
		sea = asea;
		undoable = true;
	}
	
	public override void Execute(Game game)
	{
		foreach(TacticalUnit joe in troops)
		{
			ship.unloadUnit(joe);
			port.addUnit(joe);
			joe.moveTo(port);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);			
		}
	}
	
	public override void Unexecute(Game game)
	{
		foreach(TacticalUnit joe in troops)
		{
			ship.loadUnit(joe);
			port.removeUnit(joe);
			joe.moveTo(ship.Hold);
			game.GUI.redrawTerritory(sea);
			game.GUI.redrawTerritory(port);			
		}
	}
}
}