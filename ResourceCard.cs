// created on 08/28/2005 at 15:02
namespace Gpremacy {
class ResourceCard {
	Resource Stuff;
	Unit knowledge;
	Territory Location;
	string Text;
	bool isActive;
	bool hasBeenActive;
	
	public Resource Good
	{
		get { return Stuff; }
	}
	public Territory Place
	{
		get { return Location; }
	}
	public string FlavorText
	{
		get { return Text; }
	}
	public Unit Knowledge
	{
		get { return knowledge; }
	}
	public bool Active
	{
		get { return isActive; }
		set { 
			isActive = value;
			if (isActive) 
				hasBeenActive = true;
		}
	}
	public bool HasBeenActive
	{
		get { return hasBeenActive; }
	}
	public bool isResource() 
	{
		return (Location != null);
	}
	
	public ResourceCard(Resource r, Territory l, string t)
	{
		Stuff = r;
		knowledge = null;
		Location = l;
		Text = t;
		isActive = false;
		hasBeenActive = false;
	}
	
	public ResourceCard(Unit u, Territory l, string t)
	{
		Stuff = null;
		knowledge = u;
		Location = l;
		Text = t;
		isActive = false;
		hasBeenActive = false;
	}	
		
	public string toString()
	{
/*		string ret = Stuff.toString() + " at " + Location.toString() + "\n";
		ret += "\tFT: \"" + Text + "\"";
		return ret;
*/
		if (Stuff != null)
			return Text + " in " + Location.Name + " produces " + Stuff.toString() + " per turn.";
		else
			return Text;
	}
	
	/* Since we never hand research-target Resource Cards to players, we don't have to
	   worry about them being charged upkeep for the knowledge. */
	public virtual int Upkeep
	{
		get 
		{
			if (isActive) 
				return 50;
			else
				return 0; 
		}
	}
	
}
}