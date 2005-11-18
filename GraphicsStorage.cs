// created on 08/28/2005 at 10:36
// This is a singleton object which holds all of the
// pixbufs used by the game, so as to ensure that Gpremacy
// does not waste cycles or memory loading these things 
// multiple times.

namespace Gpremacy {
class  GraphicsStorage
{
	private static GraphicsStorage instance;
	private static int numOfReference;
	private Gdk.Pixbuf blank;
	private Gdk.Pixbuf army;
	private Gdk.Pixbuf navy;
	private Gdk.Pixbuf lsat;
	private Gdk.Pixbuf icbm;
	private Gdk.Pixbuf radiation;
	private Gdk.Pixbuf detonation;
	private Gdk.Pixbuf map;
	private Gdk.Pixbuf flag_usa;
	private Gdk.Pixbuf flag_south_america;
	private Gdk.Pixbuf flag_europe;
	private Gdk.Pixbuf flag_africa;
	private Gdk.Pixbuf flag_ussr;
	private Gdk.Pixbuf flag_china;	

	private GraphicsStorage()
	{
		numOfReference = 0;
		blank = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/blank.png"));
		army = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/army.png"));
		navy = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/navy.png"));
		lsat = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/lsat.png"));
		icbm = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/icbm.png"));
		radiation = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/radiation.png")); 		
		detonation = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/detonation.png"));		
		map = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile(Game.GetInstance().MapFileName));
		flag_usa = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/usa.png"));
		flag_south_america = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/south_america.png"));
		flag_europe = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/europe.png"));
		flag_africa = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/africa.png"));
		flag_ussr = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/ussr.png"));
		flag_china = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/Flags/china.png"));		
	} 

	public static GraphicsStorage GetInstance()
	{
		if(instance == null)
		{
			instance = new GraphicsStorage();
		}
		numOfReference++;
		return instance;
	}           

	public static int Reference
	{
		get { return numOfReference; }
	} 

	public Gdk.Pixbuf Army
	{
		get { return army; }
	}
	public Gdk.Pixbuf Navy
	{
		get { return navy; }
	}
	public Gdk.Pixbuf LSat
	{
		get { return lsat; }
	}
	public Gdk.Pixbuf ICBM
	{
		get { return icbm; }
	}
	public Gdk.Pixbuf Radiation
	{
		get { return radiation; }
	}
	public Gdk.Pixbuf Detonation
	{
		get { return detonation; }
	}
	public Gdk.Pixbuf Flag_USA
	{
		get { return flag_usa; }
	}
	public Gdk.Pixbuf Flag_South_America
	{
		get { return flag_south_america; }
	}
	public Gdk.Pixbuf Flag_Europe
	{
		get { return flag_europe; }
	}
	public Gdk.Pixbuf Flag_Africa
	{
		get { return flag_africa; }
	}
	public Gdk.Pixbuf Flag_USSR
	{
		get { return flag_ussr; }
	}
	public Gdk.Pixbuf Flag_China
	{
		get { return flag_china; }
	}
	public Gdk.Pixbuf Map
	{
		get { return map; }
	}	
	public int DetonationFrames
	{
		get { return 32; }
	}
	
	public Gdk.Pixbuf getFrame(Gdk.Pixbuf buf, int framenum, int maxframes)
	{
		if (framenum >= maxframes)
			throw new System.ArgumentOutOfRangeException("Asked for frame #"+framenum + " when there are only " + maxframes + " frames.");
			
		int frameWidth = buf.Width / maxframes;
		
		Gdk.Pixbuf frame = new Gdk.Pixbuf(buf, framenum*frameWidth, 0, frameWidth, buf.Height);
		
		return frame;
	}
	
	public Gdk.Pixbuf AppropriateFlag(int countryID)
	{
		switch(countryID)
		{
			case 1:
				return flag_usa;
			case 2:
				return flag_south_america;
			case 3:
				return flag_africa;
			case 4:
				return flag_europe;
			case 5:
				return flag_ussr;
			case 6:
				return flag_china;
			default:
				return blank;
		}
	}
	
}
}
