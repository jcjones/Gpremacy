/* Gpremacy : A Free implementation of the 1984 board game 
 * Supremacy by Supremacy Games, Inc.
 * Copyright (C) 2005 James C. Jones
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

// created on 08/28/2005 at 14:54
using System;
using System.Collections;

namespace Gpremacy {
class Game {
	ArrayList players; // of Player
	ArrayList localplayers; // of Player, those who are on the local system.
	ArrayList cards; // of ResourceCard
	ArrayList allcards; // of ResourceCard
	ArrayList allunits; // of Unit
	ArrayList allresources; // of Resource
	IEnumerator cardsIterator;
	Player playerNobody;
	Territory placeNowhere;
	GameState state;
	GpremacyMarket market;
	GpremacyGUI mainGUI;
	bool justShuffledResourceCards;
	
	private static Game instance;
	private static int numOfReference;	
	
	private Game()
	{
		numOfReference = 0;		
	}
	
	public void init()
	{
		cards = new ArrayList();
		allcards = new ArrayList();
		allunits = new ArrayList();
		allresources = new ArrayList();

		playerNobody = new Player(-1, "Nobody", new Gdk.Color(123,123,123));
		placeNowhere = new Territory(-1, "Nowhere", playerNobody.CountryID, playerNobody, false, null, null);

		allunits.Add(new Army(playerNobody, placeNowhere));
		allunits.Add(new Navy(playerNobody, placeNowhere));
		allunits.Add(new Nuke(playerNobody)); 
		allunits.Add(new LSat(playerNobody));
		
		allresources.Add(new Oil());
		allresources.Add(new Minerals());
		allresources.Add(new Grain());

		players = new ArrayList();		
		setupPlayers();
		localplayers = new ArrayList();
		
		/* Force local setup */
		((Player)players[0]).Active = true;
		localplayers.Add(players[0]);
		((Player)players[1]).Active = true;
		localplayers.Add(players[1]);
		
		for (int i=0; i<12; i++)
			((Player)players[0]).ActiveUnits.Add(new Nuke((Player)players[0]));
		for (int i=0; i<3; i++)
			((Player)players[1]).ActiveUnits.Add(new LSat((Player)players[1]));
		/* Done forcing local setup */
				
		market = new GpremacyMarket();
				
		foreach (Resource r in allresources)
		{
			market.initResource(r, 12);
		}
			
		state = new GameState(this);

		mainGUI = new GpremacyGUI(this);
		
		try {
			mainGUI.init();
		} catch (Exception e) {
			Console.WriteLine("Exception caught!");
			Console.WriteLine(e.Message);
			Console.WriteLine("-1-");
			Console.WriteLine(e.StackTrace);
			Console.WriteLine("-2-");
			Console.WriteLine(e.ToString());
			Console.WriteLine("-3-");
			mainGUI.ShowError(e.Message+"\n"+e.StackTrace);
		}		
	}
	
	public static Game GetInstance()
	{
		if(instance == null)
		{
			instance = new Game();
		}
		numOfReference++;
		return instance;
	}           

	public static int Reference
	{
		get { return numOfReference; }
	} 
	
	
	public static void Main(string[] args)
	{
		Game gameObject = Game.GetInstance();	
		gameObject.init();			
	}
	
	public Player PlayerNobody
	{
		get { return playerNobody; }
	}
	public Territory PlaceNowhere
	{
		get { return placeNowhere; }
	}	

	public ArrayList Players
	{
		get { return players; }
	//	set { players = value; }
	}
	
	public ArrayList LocalPlayers
	{
		get { return localplayers; }
	}
	
	public GpremacyGUI GUI
	{
		get { return mainGUI; }
	}
	
	public GpremacyMarket Market
	{
		get { return market; }
	}
	
	public GameState State
	{
		get { return state; }
	}	
			
	public ArrayList AllCards
	{
		get { return allcards; }
	//	set { cards = value; }
	}
	
	public ArrayList Cards
	{
		get { return cards; }
	//	set { cards = value; }
	}
	
	public ArrayList AllUnits
	{
		get { return allunits; }
	}
	
	public ArrayList AllResources
	{
		get { return allresources; }
	}
		
	void setupPlayers()
	{
		players.Add(new Player(1, "United States of America", new Gdk.Color(8,75,8)));
		players.Add(new Player(2, "Confederacy of South America", new Gdk.Color(22,158,22)));
		players.Add(new Player(3, "Federation of African States", new Gdk.Color(157,31,200)));
		players.Add(new Player(4, "League of European Nations", new Gdk.Color(179,94,11)));
		players.Add(new Player(5, "Union of Soviet Sovereign Republics", new Gdk.Color(178,11,93)));
		players.Add(new Player(6, "People's Republic of China", new Gdk.Color(178,137,11)));
		
		/*foreach (Player p in players)
			p.Active = false;
		*/			
	}
	
	public void HaltGame(String a)
	{
		throw new Exception("Game halting because of " + a);
	}
	
	public bool hasSufficientWeath(Player p, Dictionary dict, int cash)
	{
		if (dict != null)
		foreach(DictionaryEntry d in dict.Data)
		{
			/* If charging multiple stocks, this will possibly subtract part of a cost set */
			if (p.getStockpileAmount((Resource)d.Key) < -1*(Int32)d.Value)
			{
				Game.GetInstance().GUI.ShowWarning("You do not have enough " + ((Resource)d.Key).Name );
				return false;
			}
		}
		if ( p.Money < cash )
		{
			Game.GetInstance().GUI.ShowWarning("You do not have enough money");
			return false;		
		}
		return true;	
	}
	
	public bool hasSufficientWeath(Player p, ArrayList stocks, int cash)
	{
		if (stocks != null)
		foreach(Stock s in stocks)
		{
			/* If charging multiple stocks, this will possibly subtract part of a cost set */
			if (p.getStockpileAmount(s.Good) < -1*s.Number)
			{
				Game.GetInstance().GUI.ShowWarning("You do not have enough " + s.Good.Name);
				return false;
			}
		}
		if ( p.Money < cash )
		{
			Game.GetInstance().GUI.ShowWarning("You do not have enough money");
			return false;		
		}
		return true; 					
	}

	public void GiveInitialUnits() 
	{
		Army unit;
		foreach(Territory territory in GUI.Map.Territories)
		{
			if (territory.IsLand && territory.Owner != PlayerNobody && territory.Owner.Active)
			{
				unit = new Army(territory.Owner, territory);
				territory.addUnit(unit);   			
				GUI.redrawTerritory(territory);
				territory.Owner.ActiveUnits.Add(unit);
			}
		}
	}

	public bool JustShuffledResourceCards
	{
		get { return justShuffledResourceCards; }
		set { justShuffledResourceCards = value; }
	}

	public ResourceCard CurrentResourceCard
	{
		get {
			return (ResourceCard)cardsIterator.Current;
		}
	}
	
	public void NextResourceCard()
	{
		if (cardsIterator.MoveNext() == false)
		{
			cardsIterator.Reset();
			cardsIterator.MoveNext();
		}
	}
	
	public void RemoveResourceCard(ResourceCard c) 
	{
		cards.Remove(c);
		cardsIterator = cards.GetEnumerator();
		cardsIterator.Reset();
		cardsIterator.MoveNext();
	}
	
	public void PopCurrentResourceCard()
	{		
		cards.Remove(cardsIterator.Current);
		cardsIterator = cards.GetEnumerator();
		cardsIterator.Reset();
		cardsIterator.MoveNext();
	}
	
	public void ShuffleResourceCards()	
	{
		System.Console.WriteLine("Count of AllCards: " + allcards.Count);
		System.Console.WriteLine("Count of UnclaimedCards: " + cards.Count);

		justShuffledResourceCards = true;
						
		Random r = new Random();

		/* Algorithm: 
  		RandomInx : integer;
  		TempPtr   : pointer;

  		for Inx := aList.Count - 1 downto 1 do begin
	    	RandomInx := Random(Inx + 1);
    		if (RandomInx <> Inx) then begin
	      		TempPtr := aList[Inx];
	      		aList[Inx] := aList[RandomInx];
	      		aList[RandomInx] := TempPtr;
	    	end;
		*/
		
		object TempPtr;
		int RandomIndex;
		int Index;
		for (Index = cards.Count - 1; Index > 0; Index--)
		{
			RandomIndex = r.Next() % Index;
			if (RandomIndex != Index)
			{
				TempPtr = cards[Index];
				cards[Index] = cards[RandomIndex];
				cards[RandomIndex] = TempPtr;
			}
		}
		
		cardsIterator = cards.GetEnumerator();
		cardsIterator.Reset();
		cardsIterator.MoveNext();
		
	}
	
	public void DistributeResourceCards()
	{
		foreach(ResourceCard card in allcards)
		{
			foreach(Player player in players)
			{
				if ((card.isResource()) && (card.Place.Owner == player) && (player.Active))
				{
					player.addResourceCard(card);
					card.Active = true; // mark it active 
					cards.Remove(card); // take it from the normal deck
					break;
				}
			}
		}
	}
	
    public void LoadResourceCards (ArrayList Territories)
    {
       	String line;
       	Resource r;
       	Territory t;
       	Unit u;
       	String name, resourceName, terrName;
       	int amt;
       	ResourceCard newcard;
        	
       	try {
       		
	  		System.IO.StreamReader input = new System.IO.StreamReader(SupportFileLoader.locateGameFile("resource_cards.csv"));
	       	do {
	       		line = input.ReadLine();
	       		if ((line == null) || (line.Length > 0 && line[0]=='#')) continue;
	       		
	       		t = null; r = null; u = null;
	       		
	       		// Format: Business Name, Territory, Resource Name, Resource Amount
	       		//           string         string       string         int	       		
	       		if (line.IndexOf(",") > 0) 
				{
	       			String[] parts;
	       			parts = line.Split(',');
	       		//	for (int i=0;i<parts.Length;i++) 
	       		//		System.Console.WriteLine("got [" + parts[i] + "]");
	       			if (parts.Length > 3) 
	       			{
	       				// Extract data
	       				name = parts[0].Trim('"');
	       				amt = Int16.Parse(parts[3]);
	       				resourceName = parts[2].Trim('"');
	       				terrName = parts[1].Trim('"');
	       				
	       				// Find Resource
	       				if (resourceName.CompareTo("Oil") == 0)
							r = new Oil(playerNobody, amt);
						else if (resourceName.CompareTo("Mineral") == 0)
							r = new Minerals(playerNobody, amt);
						else if (resourceName.CompareTo("Grain") == 0)
							r = new Grain(playerNobody, amt);
						else if (resourceName.CompareTo("Nuke") == 0)
							u = new Nuke(playerNobody);
						else if (resourceName.CompareTo("LSat") == 0)
							u = new LSat(playerNobody);
						else {
	       					System.Console.WriteLine("Could not determine resource type from [" + resourceName + "]");
	       					continue;
	       				}
	       				// Find Territory
	       				foreach (Territory terr in Territories)
	       				{
	       					if (terr.Name.CompareTo(terrName) == 0)
	       					{
	       						t = terr; break;
	       					}
	       				}
	       				if (t == null && u == null)
	       				{
	       					System.Console.WriteLine("Could not determine territory from [" + terrName + "]");
	       					continue;
	       				}
	       				
	       				// Create Card, a Research card or a Resource card
	       				if (u == null) {
	       					newcard = new ResourceCard(r, t, name);
	       					t.addResource(r);
	       				} else { 
	       					newcard = new ResourceCard(u, null, name);
	       				}

	       				allcards.Add(newcard);
	       				cards.Add(newcard);
	       				
	       			
	       			}
	       		}
	       		        		
	      		} while (input.Peek() > -1);
	      		input.Close();
	      		
    	} catch ( System.IO.FileNotFoundException e ) {
       		System.Console.WriteLine("Couldn't open resource_cards.csv. Game halts.");
       		HaltGame("Couldn't open resource_cards.csv. Game halts.");   		       			
    	}
	}

}
}
