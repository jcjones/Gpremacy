// created on 08/28/2005 at 10:36
namespace Gpremacy {
class  GraphicsStorage
{
	private static GraphicsStorage instance;
	private static int numOfReference;
	private Gdk.Pixbuf army;
	private Gdk.Pixbuf navy;
	private Gdk.Pixbuf map;
	
	protected string file_path = "/home/pug/src/gpremacy/Graphics/";


	private GraphicsStorage()
	{
		numOfReference = 0;
		army = PixbufUtils.LoadFromPath(file_path + "army.png");
		navy = PixbufUtils.LoadFromPath(file_path + "navy.png");
		map = PixbufUtils.LoadFromPath(file_path + "classic_map.jpg");
		
		if (army == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find army.png image in " + file_path) );
		if (navy == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find navy.png image in " + file_path) );
		if (map == null)
			throw ( new System.IO.FileNotFoundException("Couldn't find classic_map.jpg image in " + file_path) );
			
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
	public Gdk.Pixbuf Map
	{
		get { return map; }
	}
}
}