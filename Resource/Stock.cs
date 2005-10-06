// created on 09/23/2005 at 19:29
namespace Gpremacy {
class Stock {
	Resource good;
	int number;
	
	public Stock(Resource agood, int num)
	{
		good = agood;
		number = num;
	}

	public Resource Good
	{
		get { return good; }
	}
	
	public int Number
	{
		get { return number; }
		set { number = value; }
	}
	
 	public static Stock operator *(Stock c, int n)
 	{
	  	Stock temp = new Stock(c.Good, c.Number*n);
  		return temp;
 	}
 	
}
}