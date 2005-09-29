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
	ArrayList cards; // of ResourceCard
	ArrayList allcards; // of ResourceCard
	Player playerNobody;
	GameState state;
	GpremacyMarket market;
	GpremacyGUI mainGUI;
	
	private static Game instance;
	private static int numOfReference;	
	
	private Game()
	{
		numOfReference = 0;		
	}
	
	public void init()
	{
		players = new ArrayList();		
		setupPlayers();
						
		cards = new ArrayList();
		allcards = new ArrayList();
		
		market = new GpremacyMarket();		
		state = new GameState(this);
		mainGUI = new GpremacyGUI(this);
						
		playerNobody = new Player(-1, "Nobody");
		market.initResource(new Oil(), 12);
		market.initResource(new Minerals(), 12);
		market.initResource(new Grain(), 12); 
		
		mainGUI.init();		
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

	public ArrayList Players
	{
		get { return players; }
	//	set { players = value; }
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
	
	void setupPlayers()
	{
		players.Add(new Player(1, "United States of America"));
		players.Add(new Player(2, "Confederacy of South America"));
		players.Add(new Player(3, "Federation of African States"));
		players.Add(new Player(4, "League of European Nations"));
		players.Add(new Player(5, "Union of Soviet Sovereign Republics"));
		players.Add(new Player(6, "People's Republic of China"));
	}
	
	public void HaltGame(String a)
	{
		throw new Exception("Game halting because of " + a);
	}
	
	public void DistributeResourceCards()
	{
		foreach(ResourceCard card in allcards)
		{
			foreach(Player player in players)
			{
				if (card.Place.Owner == player)
				{
					player.addResourceCard(card);
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
       	String name, resourceName, terrName;
       	int amt;
        	
       	try {
       		
	  		System.IO.StreamReader input = new System.IO.StreamReader(SupportFileLoader.locateGameFile("resource_cards.csv"));
	       	do {
	       		line = input.ReadLine();
	       		if ((line == null) || (line.Length > 0 && line[0]=='#')) continue;
	       		
	       		// Format: Business Name, Territory, Resource Name, Resource Amount
	       		//           string         string       string         int	       		
	       		if (line.IndexOf(",") > 0) 
				{
	       			String[] parts;
	       			parts = line.Split(',');
	       			t = null;
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
	       				if (t == null)
	       				{
	       					System.Console.WriteLine("Could not determine territory from [" + terrName + "]");
	       					continue;
	       				}
	       				
	       				// Create Card
	       				allcards.Add(new ResourceCard(r, t, name));
	       				t.addResource(r);
	       			
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