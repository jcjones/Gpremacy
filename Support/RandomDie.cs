// created on 10/20/2005 at 22:28
using System;

namespace Gpremacy {
class RandomDie {
	int sides;
	Random rand;
	public RandomDie(int s)
	{
		sides = s;
		rand = new Random();
	}
	
	public RandomDie()
	{
		sides = 6;
		rand = new Random();
	}
	
	public int roll()
	{
		return rand.Next(1,sides);
	}
	
	public int rollSeveral(int num)
	{
		int res = 0;
		for(int i=0; i<num; i++)
			res+=roll();
		return res;
	}

}
}