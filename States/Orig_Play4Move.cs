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
			
			System.Console.WriteLine("Arrow On");
			arrowOn = true;
			previousTerritory = target;
		} else if (previousTerritory != null) {
			/* Picking target selection */
			
			System.Console.WriteLine("Arrow Off");
			arrowOn = false;

			// Clone the list since otherwise modifying the presently used list is bad   					
			ArrayList tmp = (ArrayList)previousTerritory.Units.Clone();   					

			if (target.IsLand == previousTerritory.IsLand)
			{
				// move units from previousTerritory to target.
				foreach (TacticalUnit unit in tmp)
				{
					// Print diagnostics
					if (unit.Owner == Game.State.CurrentPlayer)
					{
						Orig_MoveUnit cmd = new Orig_MoveUnit(unit, target, previousTerritory);
						Game.State.Execute(cmd);												   						
						break;
					}
				}
			} else {
				/* Diseperate land/sea areas, so we should load! */
				Game.GUI.showLoadNavalOptions(target);
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

	public Orig_MoveUnit(TacticalUnit aunit, Territory anext, Territory aprevious) {
		unit = aunit; next = anext; previous = aprevious;
		undoable = true;
	}
	
	public override void Execute(Game game) {
		unit.moveTo(next);
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
	public override void Unexecute(Game game) {
		unit.moveTo(previous);
		game.GUI.redrawTerritory(next);
		game.GUI.redrawTerritory(previous);
	}
}
}