// created on 08/28/2005 at 15:05
using System;

namespace Gpremacy {
[Serializable]
class Oil : Resource {
	public Oil (Player Owner, int Value) : base(Owner, Value)
	{
	}	
	public Oil() : base(null, 0) 
	{
	}

	override public string Name
	{
		get { return "Oil"; }
	}

}
}