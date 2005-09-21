// created on 08/28/2005 at 09:32
namespace Gpremacy {
class StrategicUnit : Unit {
	public StrategicUnit (Player lord) : base(lord)
	{
	}	
	public override string Name
	{
		get { return "Generic Strategic Unit"; }
	}
}
}