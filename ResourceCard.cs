// created on 08/28/2005 at 15:02
using System;

namespace Gpremacy {
[Serializable]
class ResourceCard {
	Resource Stuff;
	Unit knowledge;	
	string locName;
	string Text;
	bool isActive;
	bool hasBeenActive;
	
	public Resource Good
	{
		get { return Stuff; }
	}
	public Territory Place
	{
		get { 
			return Game.GetInstance().TerritoryByName(locName); 
		}
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
		return locName.Length > 0;
	}
	
	public ResourceCard(Resource r, Territory l, string t)
	{
		Stuff = r;
		knowledge = null;
		locName = l.Name;
		Text = t;
		isActive = false;
		hasBeenActive = false;
	}
	
	public ResourceCard(Unit u, Territory l, string t)
	{
		Stuff = null;
		knowledge = u;
		Text = t;
		isActive = false;
		hasBeenActive = false;
		
		if (l == null)
			locName = "";
		else
			locName = l.Name;
	}	
		
	public override String ToString()
	{
/*		string ret = Stuff.toString() + " at " + Location.toString() + "\n";
		ret += "\tFT: \"" + Text + "\"";
		return ret;
*/
		if (Stuff != null)
			return Text + " in " + locName + " produces " + Stuff.toString() + " per turn.";
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

[Serializable] 
class Orig_ToggleResourceCard : Command {
	ResourceCard card;
	bool nextState;
	Player player;
	
	public Orig_ToggleResourceCard(Player p, ResourceCard c, bool s)
	{
		player = p;
		card = c;
		nextState = s;
	}
	public override void Execute(Game game) 
	{
		if (nextState) 
		{
			/* Make sure they can activate */
			if (card.Place.Owner == player)
			{
				card.Active = nextState;
			}
		
		} else {
			/* Always allow disabling */
			card.Active = nextState;
		}
	}

}

}