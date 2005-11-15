// created on 11/11/2005 at 15:20
using System.Collections;

namespace Gpremacy.AI {
class Goal_Composite : Goal {

	protected ArrayList Subgoals; // of Goal

	public Goal_Composite(string name, Processor bot) : base(name, bot)
	{
		Subgoals = new ArrayList();
	}
	
	public override bool HandleState(Gpremacy.State state)
	{
		bool SubgoalStatus = ProcessSubgoals(state);
		
		return SubgoalStatus;
	}

	public override void Arbitrate()
	{
		foreach (Goal g in Subgoals)
    	{
			g.Arbitrate();
		}
	}
	
	public override bool HandleCommand(Gpremacy.Command cmd)
	{
		bool handled = false;
		
		/* Ask each of our subgoals in sequence to try
		 * and handle this command. If they fail, then we
		 * give it a shot. */
		foreach (Goal g in Subgoals)
		{
			if (g.HandleCommand(cmd))
				handled = true;
		}
			
		/* See if we can handle this ourselves.. */

		/* Done */
		return handled;			
	}
	
	public virtual void AddSubgoal(Goal g)
	{
		Subgoals.Add(g);
	}
	
	public void RemoveAllSubgoals()
	{
		Subgoals.Clear();
	}
	
	public bool ProcessSubgoals(Gpremacy.State state)
	{
		foreach (Goal thisSubGoal in Subgoals)			
		{
			thisSubGoal.HandleState(state);
		}
		return true;
	}
	


}
}
