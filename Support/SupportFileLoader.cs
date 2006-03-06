// created on 09/18/2005 at 22:07

// Should use System.AppDomain.CurrentDomain.BaseDirectory

using System;

namespace Gpremacy {
class SupportFileLoader {

	public static string locateGameFile(string filename)
	{		
		string[] paths = new string[] { "/usr/share/gpremacy/",
										"/usr/local/share/gpremacy/",
										"", 
										"./", 
										"../", 
										"../../"};
		 
		for (int i=0; i<paths.Length; i++)
		{
			if (System.IO.File.Exists(paths[i]+filename))
				return String.Concat(paths[i],filename);
		} 
				
		throw ( new System.IO.FileNotFoundException("Couldn't find " + filename) );
		
	}
	
}
}
