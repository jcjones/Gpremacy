// created on 09/15/2005 at 11:08
using System;
using System.Collections;


namespace Gpremacy {

class Dictionary {
	ArrayList data;

	public Dictionary()
	{	
		data = new ArrayList();
	}

	public ArrayList Data
	{
		get { return data; }
	}
	
	public System.Collections.IEnumerator GetEnumerator()
	{
		return data.GetEnumerator();
	}	

	public void Add(object key, object val)
	{
		data.Add(new DictionaryEntry(key, val));
	}
	
	public object GetValue(object key)
	{
		object val = null;
		foreach (DictionaryEntry de in data)
		{
			if (de.Key.GetHashCode() == key.GetHashCode())
			{
				return val = de.Value;
			}
		}
		return val;
	}
	
	public bool Exists(object key)
	{
		if (GetValue(key) == null)
			return false;
		return true;
	}		

	public void UpdateValue(object key, object val)
	{
		foreach (DictionaryEntry de in data)
		{
			if (de.Key.GetHashCode() == key.GetHashCode())
			{
				// NOT the way to do it, but this works while other methods fail
				data.Remove(de);
				data.Add(new DictionaryEntry(key, val));
				return;
			}
		}	
	}
	
	public int IncValue(object key)
	{
		// Support Routine for Player.cs -- assumes object "Value" to be Int32.
		if (!Exists(key))
			Add(key, 0);
		int val = (Int32)(GetValue(key)) + 1 ;		
		UpdateValue(key, val);
		return val;
	}
	public int IncValue(object key, int inc)
	{
		// Support Routine for Player.cs -- assumes object "Value" to be Int32.
		if (!Exists(key))
			Add(key, 0);
		int val = (Int32)(GetValue(key)) + inc ;		
		UpdateValue(key, val);
		return val;
	}	
	
	public object Remove(object key)
	{
		object val = null;
		foreach (DictionaryEntry de in data)
		{
			if (de.Key.GetHashCode() == key.GetHashCode())
			{
				val = de.Value;
				data.Remove(de);
				return val;
			}
		}
		return val;	
	}
	
	public void Clear()
	{
		data.Clear();
	}

	public string toString()
	{
		string ret = "";
		foreach (DictionaryEntry de in data)
		{
			ret += de.Value + " " + de.Key + "\n";
		}
		return ret;
	}

}

}