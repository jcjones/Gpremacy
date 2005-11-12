// created on 11/11/2005 at 18:26
namespace Gpremacy.AI {

/* This is the highest-level goal in Gpremacy. This goal is "Win the game". */
class Goal_Defend  : Goal_Composite {

	public Goal_Defend(Processor bot) : base("Defend", bot)
	{
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
		if (cmd is Gpremacy.Orig_AttackConventionalStart)
		{
			System.Console.WriteLine("Got a conv attack start");
			defendAgainstConventionalAttack((Gpremacy.Orig_AttackConventionalStart)cmd);
			return true;
		}
		
		if (cmd is Gpremacy.Orig_AttackStrategicStart)
		{
			defendAgainstStrategicAttack((Gpremacy.Orig_AttackStrategicStart)cmd);
			return true;
		}

		return false;
	}
	
	protected void defendAgainstConventionalAttack(Gpremacy.Orig_AttackConventionalStart cmd)
	{
		bool fd = true;
		
		System.Console.WriteLine("Defending with choice " + fd);
				
		if (cmd.defender != spinalCord.myPlayer)
			return;
			
		System.Console.WriteLine("Defending sending reply with choice " + fd);
			
		Gpremacy.Orig_AttackConventionalDefenderReady reply = 
			new Gpremacy.Orig_AttackConventionalDefenderReady(cmd.attacker,
															cmd.defender, fd);

		spinalCord.sendCommand(reply);	
	}

	protected void defendAgainstStrategicAttack(Orig_AttackStrategicStart cmd)
	{
	}
}
}
