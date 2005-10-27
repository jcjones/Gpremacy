// created on 10/27/2005 at 16:50
using System;
using Gtk;
using GtkSharp;

namespace Gpremacy
{
	public class ExceptionWindow : Gtk.Window
	{
		Exception exception;
		
		public ExceptionWindow (String name, Exception exp, Gtk.Window winToHide) : base ("Exception Caught: Application Must Close")
		{
			//e.Message
			//e.StackTrace
			//GLib.Markup.EscapeText(error)
			exception = exp;
					
			Gtk.VBox box = new Gtk.VBox(false, 5);
			
			box.PackStart(new Gtk.Label(name + " has thrown an exception and must close.\n" +
					"This window contains exception information to\n" + 
					"assist the developer in fixing this bug."));
			
			//Style.LookupIconSet("Error")
					
			
          	Gtk.TextView messageView = new Gtk.TextView ();
	        messageView.Buffer.Text = GLib.Markup.EscapeText(exception.Message);
	        Gtk.ScrolledWindow messageViewSW = new Gtk.ScrolledWindow();
	        messageViewSW.HscrollbarPolicy = Gtk.PolicyType.Automatic;
	        messageViewSW.VscrollbarPolicy = Gtk.PolicyType.Automatic;
	        messageViewSW.ShadowType = Gtk.ShadowType.In;
	        messageViewSW.Add(messageView);
	        
          	Gtk.TextView traceView = new Gtk.TextView ();
	        traceView.Buffer.Text = GLib.Markup.EscapeText(exception.StackTrace);
	        Gtk.ScrolledWindow traceViewSW = new Gtk.ScrolledWindow();
	        traceViewSW.HscrollbarPolicy = Gtk.PolicyType.Automatic;
	        traceViewSW.VscrollbarPolicy = Gtk.PolicyType.Automatic;
	        traceViewSW.ShadowType = Gtk.ShadowType.In;
	        traceViewSW.Add(traceView);
	        
	        box.PackStart(messageViewSW);
	        box.PackStart(new Gtk.HSeparator());
	        box.PackStart(traceViewSW);
	        
	        Gtk.HBox buttonbox = new Gtk.HBox();	        
	        
	        Gtk.Button close = new Gtk.Button(Gtk.Stock.Close);
	        close.UseStock = true;
	        close.Clicked += onClose;

	        Gtk.Button save = new Gtk.Button(Gtk.Stock.Save);
	        save.UseStock = true;
	        save.Clicked += onSave;
	        save.Sensitive = false;
	        
	        buttonbox.PackStart(save);
	        buttonbox.PackStart(close);
	        
	        box.PackStart(buttonbox);
			
			this.Add(box);
			
			
			this.Modal = true;
						
			if (winToHide != null)
			{
				winToHide.Hide();
			}
				
			this.ShowAll();			
			Application.Run();
		}

		public void onSave(System.Object obj, EventArgs e)
		{
			
		}
		
		public void onClose(System.Object obj, EventArgs e)
		{
			Application.Quit();
		}
		
	}
}
