// created on 08/27/2005 at 21:42
using System.Collections;

namespace Gpremacy {
class Player {
	int countryID;
	string name;
	ArrayList stockpile; // of Resources
	ArrayList availableUnits; // of Units
	ArrayList activeUnits; // of Units
	long money;
	ArrayList resourceCards; // of ResourceCards
	
	public Player (int id, string n)
	{
		countryID = id;
		name = n;
		money = 0;
		stockpile = new ArrayList();
		availableUnits = new ArrayList();
		activeUnits = new ArrayList();
		resourceCards = new ArrayList();
	}
	
	public string toString ()
	{
		return name;
	}
	
	public long Money
	{
		get { return money; }
		set { money = value; }
	}
	public int CountryID
	{
		get { return countryID; }
	}	
	public string Name
	{
		get { return name; }
	}
	public ArrayList ActiveUnits
	{
		get { return activeUnits; }
	}
	public ArrayList Stockpile
	{
		get { return stockpile; }
	}	
	public ArrayList AvailableUnits
	{
		get { return availableUnits; }
	}
	public ArrayList ResourceCards
	{
		get { return resourceCards; }
	}

	public void printResources()
	{
		int m=0,o=0,g=0;
		
		foreach (ResourceCard card in resourceCards)
		{
			if (card.Good is Minerals) {
				m+= card.Good.Value;
			} else if (card.Good is Oil) {
				o+= card.Good.Value;
			} else if (card.Good is Grain) {
				g+= card.Good.Value;
			} else {
			}			
		}
		System.Console.WriteLine("Grain: " + g + " Oil: " + o + " Minerals: " + m);
	}
	
	public void addResourceCard(ResourceCard i)
	{
		resourceCards.Add(i);
	}
	
	public bool removeResourceCard(ResourceCard i)
	{
		resourceCards.Remove(i);
		return true;
	}
	
	public bool removeResourceCard(Territory t, Resource r)
	{
		foreach (ResourceCard card in resourceCards)
		{
			if ( card.Place == t )
			{
				// evil if statement, but there is no other runtype type check
				if ( (card.Good is Oil && r is Oil) || 
				(card.Good is Grain && r is Grain) || 
				(card.Good is Minerals && r is Minerals) ) 
				{
				resourceCards.Remove(card);
				return true;
				}
			}
		}
		return false;
	}

}
}