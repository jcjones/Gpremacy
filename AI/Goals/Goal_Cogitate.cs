// created on 11/11/2005 at 17:02
namespace Gpremacy.AI {

/* This is the highest-level goal in Gpremacy. This goal is "Win the game". */
class Goal_Cogitate  : Goal_Composite {

	public Goal_Cogitate(Processor bot) : base("Cogitate", bot)
	{
		this.AddSubgoal(new Goal_Defend(bot));
	}

	/*public override void Arbitrate()
	{
		base.Arbitrate(); // manage subgoals' decisions
	}*/
	
	public override bool HandleCommand(Gpremacy.Command cmd)
	{
		System.Console.WriteLine(this + " is handling command " + cmd);
		return base.HandleCommand(cmd);
	}
	


}
}
