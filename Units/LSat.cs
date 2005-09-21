// created on 08/28/2005 at 09:34
namespace Gpremacy {
class LSat : StrategicUnit {
	bool FiredThisTurn;
	public LSat (Player lord) : base(lord)
	{
	}	

	public override string Name
	{
		get { return "L-Sat"; }
	}

}
}