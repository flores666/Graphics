namespace GraphicsBase
{
	public static class DictionaryExtensions
	{
		public static int IndexOfKey<T>(this Dictionary<string, T> dictionary, string key)
		{
			var index = 0;
			foreach(var item in dictionary) 
			{
				if (item.Key == key) return index;
				index++;
			}
			return -1;
		}
	}
}
