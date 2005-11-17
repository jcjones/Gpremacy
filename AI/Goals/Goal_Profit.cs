// created on 11/11/2005 at 18:26
using System.Collections;

namespace Gpremacy.AI {

/* This is the generic "defend against attacks" goal, which also 
 * arbeitrates some unit building. */
 
class Goal_Profit : Goal {

	public Goal_Profit(Processor bot) : base("Profit", bot)
	{
	}
	

	public override void Arbitrate()
	{		
	}	
	
	public override bool HandleState(Gpremacy.State state)
	{
		if (state is Orig_Play2Sell)
		{
			System.Console.WriteLine("Can sell.");
			return true;
		}
		if (state is Orig_Play6Prospect)
		{
			System.Console.WriteLine("Can buy.");
			return true;
		}
		
		return false;
	}		
	
}
}
