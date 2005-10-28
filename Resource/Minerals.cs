// created on 08/28/2005 at 15:05
using System;

namespace Gpremacy {
[Serializable]
class Minerals : Resource {
	public Minerals (Player Owner, int Value) : base(Owner, Value)
	{
	}	
	public Minerals() : base(null, 0) 
	{
	}

	override public string Name
	{
		get { return "Minerals"; }
	}	
}
}