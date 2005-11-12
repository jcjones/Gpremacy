// created on 11/11/2005 at 11:29
namespace Gpremacy.AI {

public enum GoalStatus
{
	Inactive,Active,Completed,Failed		
}
	
abstract class Goal {

	protected GoalStatus status;
	protected string name;
	protected Processor spinalCord;

	public Goal(string n, Processor bot)
	{
		name = n;
		spinalCord = bot;
		status = GoalStatus.Inactive;
	}
	
	public override string ToString() 
	{
		return name;
	}
	
	abstract public void Activate();	
	abstract public GoalStatus Process();	
	abstract public void Terminate();	
	abstract public bool HandleCommand(Command cmd);	
	abstract public void AddSubgoal(Goal g);
	
	public virtual void ActivateIfInactive()
	{
		if (IsInactive())
			Activate();
	}
	
	public virtual void ReactivateIfFailed()
	{
		if (HasFailed())
			status = GoalStatus.Inactive;		
	} 
	
	public virtual bool IsActive()
	{
		return status==GoalStatus.Active;
	}

	public virtual bool IsInactive()
	{
		return status==GoalStatus.Active;
	}	
	
	public virtual bool IsComplete()
	{
		return status==GoalStatus.Completed;
	}
	
	public virtual bool HasFailed()
	{
		return status==GoalStatus.Failed;
	}	

}
}