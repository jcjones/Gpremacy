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
		if (Button != 1) return false;
		
		if (previousTerritory == null && target.occupiedBy(Game.State.CurrentPlayer))
		{
			System.Console.WriteLine("Arrow On");
			arrowOn = true;
			previousTerritory = target;
		} else if (previousTerritory != null) {
			System.Console.WriteLine("Arrow Off");
			arrowOn = false;

			// Clone the list since otherwise modifying the presently used list is bad   					
			ArrayList tmp = (ArrayList)previousTerritory.Units.Clone();   					

			// move units from previousTerritory to target.
			foreach (TacticalUnit unit in tmp)
			{
				// Print diagnostics
				if (unit.Owner == Game.State.CurrentPlayer)   						
					unit.moveTo(target);
			}

			Game.GUI.redrawTerritory(previousTerritory);
			Game.GUI.redrawTerritory(target);

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

	public override void done()
	{
	}

}
}