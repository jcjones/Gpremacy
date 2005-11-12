// created on 11/11/2005 at 15:20
using System.Collections;

namespace Gpremacy.AI {
class Goal_Composite : Goal {

	protected Stack Subgoals; // of Goal

	public Goal_Composite(string name, Processor bot) : base(name, bot)
	{
		Subgoals = new Stack();
	}
	
	public override void Activate()
	{
		/* Ensure we have no stale goals */
		Subgoals.Clear();
	}
	
	public override GoalStatus Process()
	{
		return GoalStatus.Inactive;
	}
	
	public override void Terminate()
	{
		RemoveAllSubgoals();
	}
	
	public override bool HandleCommand(Gpremacy.Command cmd)
	{
		bool handled = false;
		
		/* Ask each of our subgoals in sequence to try
		 * and handle this command. If they fail, then we
		 * give it a shot. */
		foreach (Goal g in Subgoals)
		{
			/*@TODO: Confirm this is going in stack order */
			handled = g.HandleCommand(cmd);
			if (handled)
				return true;	
		}
			
		/* See if we can handle this since our subgoals didn't. */
		return false;			
	}
	
	public override void AddSubgoal(Goal g)
	{
		Subgoals.Push(g);
	}
	
	public void RemoveAllSubgoals()
	{
		while (Subgoals.Count > 0)
		{
			Goal g = (Goal)Subgoals.Pop();
			g.Terminate();
		}
		Subgoals.Clear();
	}
	
	public GoalStatus ProcessSubgoals()
	{
		while (Subgoals.Count > 0 && 
			( ((Goal)Subgoals.Peek()).IsComplete() || 
			   ((Goal)Subgoals.Peek()).HasFailed() ) )
		{
			/* Remove all completed and failed goals */
			Goal rmGoal = (Goal)Subgoals.Pop();
			rmGoal.Terminate();			
		} 
		
		if (Subgoals.Count > 0)
		{
			GoalStatus StatusOfGoals = ((Goal)Subgoals.Peek()).Process();
			/* Handle special case where the topmost subgoal reports completed 
			 * and the list contains additional goals. In this case, to ensure
			 * the parent keeps processing this subgoal list we return "Active"
			 * status. */
			 if (StatusOfGoals == GoalStatus.Completed && Subgoals.Count > 1)
			 {
			 	return GoalStatus.Active;
			 } 
			 
			 return StatusOfGoals;
			
		} else {
			/* No subgoals left, we're done */
			return GoalStatus.Completed;
		}
	}
	


}
}