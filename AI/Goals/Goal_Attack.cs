// created on 11/11/2005 at 18:26
using System.Collections;

namespace Gpremacy.AI {

/* This is the generic "defend against attacks" goal, which also 
 * arbeitrates some unit building. */
 
class Goal_Attack  : Goal {

	public Goal_Attack(Processor bot) : base("Attack", bot)
	{
	}
	

	public override void Arbitrate()
	{		
	}	
	
	public override bool HandleState(Gpremacy.State state)
	{
		if (state is Orig_Play3Attack)
		{
			System.Console.WriteLine("Can Attack.");
			return true;
		}		
		return false;
	}		
	
}
}
