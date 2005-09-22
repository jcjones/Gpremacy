// created on 09/19/2005 at 21:37

namespace Gpremacy {
class State {
	Game game;
	int myOrder;
	int nextOrder;

	public State(Game game_i, int myOrder_i, int nextOrder_i)
	{
		game = game_i;
		myOrder = myOrder_i;
		nextOrder = nextOrder_i;
	}
	
	public Game Game	
	{
		get { return game; }
	}	
	public int MyOrder
	{
		get { return myOrder; }
	}
	public int NextOrder
	{
		get { return nextOrder; }
	}
	
	public virtual bool mouseClick(Territory target, uint Button)
	{
		return false;
	}
	
	public virtual bool mouseMotion(double x, double y, Territory curTerr, uint Button)
	{
		return false;
	}

	public virtual void beginState()
	{
	}
	
	public virtual void doneState()
	{
	}
	
	public virtual void beginPlayer(Player player)
	{
	}
	
	public virtual void donePlayer(Player player)
	{
	}

	
	public virtual string Name()
	{
		return "Generic State";
	}
	
	public virtual string toString()
	{
		return "I am " + Name();
	} 

}
}