// created on 11/11/2005 at 11:29
namespace Gpremacy.AI {

	
abstract class Goal {

	protected string name;
	protected Processor spinalCord;

	public Goal(string n, Processor bot)
	{
		name = n;
		spinalCord = bot;
	}
	
	public override string ToString() 
	{
		return spinalCord.myPlayer.Name + "::" + name + " ";
	}
	
	public virtual bool HandleCommand(Command cmd) 
	{ 
		return false; 
	}

	public virtual bool HandleState(Gpremacy.State state) 
	{ 
		return false; 
	}

	public virtual void Arbitrate()
	{
	}
}
}
