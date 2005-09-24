/* Using the Command design pattern... */
/* In Gpremacy, use these something like this:
 						Orig_MoveUnit cmd = new Orig_MoveUnit();
						cmd.New = target;
						cmd.Old = previousTerritory;
						cmd.Unit = unit;
						Game.State.Execute(cmd);
*/
namespace Gpremacy {
class Command {
	string name = "Generic";
	protected bool undoable = false;
	
	public virtual void Execute(Game game) 
	{
	}
	
	public virtual void Unexecute(Game game)
	{
	}

	public string Name
	{
		get { return name; }
	}
	
	public bool Undoable
	{
		get { return undoable; } 
	}
}
}