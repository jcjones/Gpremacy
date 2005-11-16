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
	private Gdk.Pixbuf army;
	private Gdk.Pixbuf navy;
	private Gdk.Pixbuf lsat;
	private Gdk.Pixbuf icbm;
	private Gdk.Pixbuf radiation;
	private Gdk.Pixbuf detonation;
	private Gdk.Pixbuf map;
	

	private GraphicsStorage()
	{
		numOfReference = 0;
		army = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/army.png"));
		navy = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/navy.png"));
		lsat = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/lsat.png"));
		icbm = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/icbm.png"));
		radiation = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/radiation.png")); 		
		detonation = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/detonation.png"));		
		map = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile(Game.GetInstance().MapFileName));
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
	
}
}
