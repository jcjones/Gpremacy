// created on 08/28/2005 at 15:05
namespace Gpremacy {
class Grain : Resource {
	public Grain (Player Owner, int Value) : base(Owner, Value)
	{
	}	

	override public string Name
	{
		get { return "Grain"; }
	}
}
}