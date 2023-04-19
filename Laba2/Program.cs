namespace Laba2;

public class Program
{
	public static void Main(string[] args)
	{
		using(var app = new Graphics(960, 700, "Laba2"))
		{
			app.Run();
		}
	}
}