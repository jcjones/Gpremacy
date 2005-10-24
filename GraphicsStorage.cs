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
	private Gdk.Pixbuf detonation;
	private Gdk.Pixbuf map;
	

	private GraphicsStorage()
	{
		numOfReference = 0;
		army = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/army.png"));
		navy = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/navy.png"));
		lsat = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/lsat.png"));
		icbm = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/icbm.png"));
		
		map = PixbufUtils.LoadFromPath(SupportFileLoader.locateGameFile("Graphics/classic_map.jpg"));
		
		if (army == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find army.png image") );
		if (navy == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find navy.png image") );
		if (lsat == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find lsat.png image") );
		if (icbm == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find icbm.png image") );
		if (map == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find classic_map.jpg image") );
			
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
	public Gdk.Pixbuf Map
	{
		get { return map; }
	}
}
}
