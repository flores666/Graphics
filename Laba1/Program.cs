namespace Laba1;

public class Program
{
	private static void Main(string[] args)
	{
		using(var app = new Graphics(800, 600, "Laba1"))
		{
			app.Run();
		}	
	}
}