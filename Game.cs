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
	
	public Game () {	
		players = new ArrayList();		
		setupPlayers();
						
		cards = new ArrayList();
		allcards = new ArrayList();
		
		market = new GpremacyMarket();		
		state = new GameState(this);
		mainGUI = new GpremacyGUI(this);
						
		playerNobody = new Player(-1, "Nobody");
		market.initResource(new Oil(playerNobody, 1), 500);
		market.initResource(new Minerals(playerNobody, 1), 500);
		market.initResource(new Grain(playerNobody, 1), 500); 
		
		mainGUI.init();		
		
	}
	
	public static void Main(string[] args)
	{
		Game gameObject = new Game();				
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
       		System.Console.WriteLine("Couldn't open resource_cards.csv.");        		       			
    	}
	}

}
}