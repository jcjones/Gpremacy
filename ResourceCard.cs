// created on 08/28/2005 at 15:02
namespace Gpremacy {
class ResourceCard {
	Resource Stuff;
	Territory Location;
	string Text;
	
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
	
	public ResourceCard(Resource r, Territory l, string t)
	{
		Stuff = r;
		Location = l;
		Text = t;
	}
		
	public string toString()
	{
		string ret = Stuff.toString() + " at " + Location.toString() + "\n";
		ret += "\tFT: \"" + Text + "\"";
		return ret;
	}
	
}
}