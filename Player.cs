// created on 08/27/2005 at 21:42
using System.Collections;
using System;

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
		money = 5000; // In Millions
		stockpile = new ArrayList(); // of Resource
		availableUnits = new ArrayList(); // of Unit
		activeUnits = new ArrayList(); // of Unit
		resourceCards = new ArrayList(); // of ResourceCard
		
		stockpile.Add(new Minerals(this, 6));
		stockpile.Add(new Grain(this, 6));
		stockpile.Add(new Oil(this, 6));
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

	public string toStringResources()
	{
		// This function uses Gpremacy's custom Dictionary class
		Dictionary rTypes = new Dictionary(); 
		
		int m=0,o=0,g=0;
		string ret;

		//Console.WriteLine("AU: " + activeUnits.Count + " RC: " + resourceCards.Count + " SP: " + stockpile.Count);
		ret = "Available Capital:\n";
		ret += money + "\n";

		ret += "Resources Produced Per Turn:\n";
		foreach (ResourceCard card in resourceCards)
		{
			if (!rTypes.Exists(card.Good.Name))
			{
				rTypes.Add(card.Good.Name, card.Good.Value);
			} else {
				rTypes.IncValue(card.Good.Name, card.Good.Value);
			}
		}
		ret += rTypes.toString();
		
		ret += " [" + rTypes.Data.Count + "]\n ";
		
		rTypes.Clear();
		
		ret += "Resource Stockpile:\n";
		foreach (Resource good in stockpile)
		{
			if (!rTypes.Exists(good.Name))
			{
				rTypes.Add(good.Name, (Int32)good.Value);
			} else {
				rTypes.IncValue(good.Name);
			}
		}
		ret += rTypes.toString();
		
		ret += " [" + rTypes.Data.Count + "]\n ";
		
		rTypes.Clear();		
		
		ret += "Active Units:\n";
		foreach (Unit unit in activeUnits)
		{
			if (!rTypes.Exists(unit.Name))
			{
				rTypes.Add(unit.Name, (Int32)1);
			} else {
				rTypes.IncValue(unit.Name);
			}
		}
		ret += rTypes.toString();
		
		ret += " [" + rTypes.Data.Count + "]\n ";
		
		rTypes.Clear();		
		
		return ret;
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