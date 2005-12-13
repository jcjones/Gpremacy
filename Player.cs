// created on 08/27/2005 at 21:42
using System;
using System.Collections;

namespace Gpremacy {
[Serializable]
class Player {
	int countryID;
	string name;
	ArrayList stockpile; // of Stock
	ArrayList availableUnits; // of Units
	ArrayList activeUnits; // of Units
	long money;
	ArrayList resourceCards; // of ResourceCards
	bool isActive;

	byte colorBlue, colorRed, colorGreen;
	
	int numberBlindBidsLeftThisTurn;
	bool bidIntoNextRound;
	
	
	public Player (int id, string n, Gdk.Color c)
	{
		isActive = false;
		countryID = id;
		name = n;
		money = 5000; // In Millions
		stockpile = new ArrayList(); // of Resource
		availableUnits = new ArrayList(); // of Unit
		activeUnits = new ArrayList(); // of Unit
		resourceCards = new ArrayList(); // of ResourceCard
		
		stockpile.Add(new Stock(new Minerals(), 6));
		stockpile.Add(new Stock(new Grain(), 6));
		stockpile.Add(new Stock(new Oil(), 6));
		
		availableUnits.Add(new Army(this, Game.GetInstance().PlaceNowhere));
		availableUnits.Add(new Navy(this, Game.GetInstance().PlaceNowhere));
		//availableUnits.Add(new Nuke(this));
		//availableUnits.Add(new LSat(this));
		
		numberBlindBidsLeftThisTurn = 4;
		bidIntoNextRound = true;
		
		Color = c; // use function Color
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
	public bool Active
	{
		set { isActive = value; } 
		get { return isActive; }  
	}	
	public ArrayList ActiveUnits
	{
		get { return activeUnits; }
	}
	public ArrayList AvailableUnits
	{
		get { return availableUnits; }
	}
	public ArrayList ResourceCards
	{
		get { return resourceCards; }
	}
	public int BlindBidsLeft
	{
		get { return numberBlindBidsLeftThisTurn; }
		set { numberBlindBidsLeftThisTurn = value; }
	}
	public bool BidIntoNextRound
	{
		get { return bidIntoNextRound; }
		set { bidIntoNextRound = value; }
	}	
	public int MaximumStockpile
	{
		get { return 12; }
	}
	public Gdk.Color Color
	{
		get { 
			return new Gdk.Color(colorRed, colorGreen, colorBlue); 
		}
		set { 
			colorRed = (byte)value.Red;
			colorBlue = (byte)value.Blue;
			colorGreen = (byte)value.Green; 
		}
	}
		
	public int CalculateUpkeep()
	{
		int amount = 0;
		foreach (Unit unit in activeUnits)
		{
			amount += unit.Upkeep;
		}
		foreach (ResourceCard card in resourceCards)
		{
			if (card.Active)
				amount += card.Upkeep;
		}
		return amount;
	}	

	public string toStringResources()
	{
		// This function uses Gpremacy's custom Dictionary class
		Dictionary rTypes = new Dictionary(); 
		
		string ret;

		//Console.WriteLine("AU: " + activeUnits.Count + " RC: " + resourceCards.Count + " SP: " + stockpile.Count);
		ret = "Available Capital:\n";
		ret += "$" + money + " M\n";
		ret += "Operating Costs:\n";
		ret += "$" + CalculateUpkeep() + " M / turn\n";

		ret += "Resources Produced Per Turn:\n";
		foreach (ResourceCard card in resourceCards)
		{
			if (card.Active) {
				if (!rTypes.Exists(card.Good.Name))
				{
					rTypes.Add(card.Good.Name, card.Good.Value);
				} else {
					rTypes.IncValue(card.Good.Name, card.Good.Value);
				}
			}
		}
		ret += rTypes.ToString();
		
		//ret += " [" + rTypes.Data.Count + "]\n ";
		
		rTypes.Clear();
		
		ret += "Resource Stockpile:\n";
		foreach (Stock goods in stockpile)
		{
			rTypes.Add(goods.Good.Name, (Int32)goods.Number);
		}
		ret += rTypes.ToString();
		
		//ret += " [" + rTypes.Data.Count + "]\n ";
		
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
		ret += rTypes.ToString();
		
		//ret += " [" + rTypes.Data.Count + "]\n ";
		
		rTypes.Clear();		
		
		return ret;
	}
	
	/* Slow... don't run often */	
	public void updateResourceCards()
	{
		foreach(ResourceCard card in resourceCards)
		{
			if (card.Place.Owner != this)
			{
				Orig_ToggleResourceCard cmd = new Orig_ToggleResourceCard(this, card, false);
				Game.GetInstance().State.Execute(cmd);
			}
		}
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
	
	public int getStockpileAmount(Resource r)
	{
		foreach (Stock goods in stockpile)
		{
			if (goods.Good.Name == r.Name && goods.Number > 0)
			{
				return goods.Number;
			}
		}
		return 0;
	}
	
	public int getStockpileNumberFullSets()
	{
		int min = this.MaximumStockpile;
		foreach (Stock goods in stockpile)
		{
			if (min > goods.Number)
				min = goods.Number;
		}
		return min;
	}
	
	public ArrayList getActiveUnitsOfType(Unit u)
	{
		ArrayList ret = new ArrayList();
		
		foreach (Unit unit in activeUnits)
		{
			if (u.Name == unit.Name)
				ret.Add(unit);
		}
		
		return ret;
	}
	
	public int getActiveUnitAmount(Unit u)
	{
		int count = 0;
		foreach (Unit unit in activeUnits)
		{
			if (unit.Name == u.Name)
				count++;
		}
		return count;
	}	
	
	public int changeResourceStockpile(Stock a)
	{		
		foreach (Stock stockgood in stockpile)
		{
			if (stockgood.Good.Name == a.Good.Name)
			{
				stockgood.Number += a.Number;
				if (stockgood.Number > MaximumStockpile) stockgood.Number = MaximumStockpile;
				if (stockgood.Number < 0) throw new ArgumentOutOfRangeException("Resource " + stockgood.Good.Name + " has gone negative " + stockgood.Number + " after taking " + a.Number);
				return stockgood.Number;
			}
		}
		return -1;
	}

	public int changeResourceStockpile(Resource good, int val)
	{
		return changeResourceStockpile(new Stock(good, val));
	}
	
	public int countActiveUnits(Unit target)
	{
		if (target == null)
			return activeUnits.Count;
		
		int count = 0;
		
		foreach (Unit unit in activeUnits)
			if (unit.Name == target.Name)
				count++;
		
		return count;
	}
	
	public bool canBuild(Unit u) {
		foreach (Unit a in availableUnits) {
			if (u.Name == a.Name)
				return true;
		}
		return false;
	}
	
	public void makeUnitAvailable(Unit u) 
	{
		if (canBuild(u))
			return;
		
		availableUnits.Add(u);
	}

	/* Since we won't have exactly the same objects on client systems, 
	 * we must do pattern equality matching based on serial number. */
	override public int GetHashCode() 
	{
		return countryID.GetHashCode();
	}
	override public bool Equals(object o)
	{
		try {
			return countryID == ((Player)o).countryID;
		} catch {
			return false;
		}
	}
	public static bool operator ==(Player a, Player b)
 	{	
		try {
			return a.countryID == b.countryID;
		} catch {
			if (!(a is object) && !(b is object))
			{
				return true;
			}
			return false;
		}
	}
	public static bool operator !=(Player a, Player b)
	{
 		return !(a==b);
	}
	/* Done screwing with equality matching */	
	
}
}