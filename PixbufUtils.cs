/* PixbufUtils.cs
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
 * 02111-1307, USA.
 *
 * Author: Pedro Villavicencio.
 * Modified by: James C. Jones <jcjones@cise.ufl.edu>
 *
 */


using Gtk;
using Gdk;

namespace Gpremacy{

	class PixbufUtils {

		// Copied from f-spot
		// These should eventually appear in Gtk#
		static private Pixbuf LoadFromStream (System.IO.Stream input)
		{
			Gdk.PixbufLoader loader = new Gdk.PixbufLoader ();
			byte [] buffer = new byte [8192];
			int n;
														       
			while ((n = input.Read (buffer, 0, 8192)) != 0)
				loader.Write (buffer, (uint) n);
														       
			loader.Close ();
			return loader.Pixbuf;
		}
		
		static public Pixbuf LoadFromPath (string path) 
		{
			Pixbuf PB;

			System.IO.StreamReader stream = new System.IO.StreamReader(path);
			PB = PixbufUtils.LoadFromStream (stream.BaseStream);
			stream.Close();
			
			return PB;
		}
														       
		static public Pixbuf LoadFromAssembly (System.Reflection.Assembly assembly, string resource)
		{
			System.IO.Stream s = assembly.GetManifestResourceStream (resource);
			if (s == null)
				return null;
			else
				return LoadFromStream (s);
		}
														       
		static public Pixbuf LoadFromAssembly (string resource)
		{
			return LoadFromAssembly (System.Reflection.Assembly.GetCallingAssembly (), resource);
		}
	}
}

