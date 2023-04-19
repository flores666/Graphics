using Laba7;

public class Program
{
	private static void Main(string[] args)
	{
		using (var window = new Graphics(1280, 720, "Laba7"))
		{
			window.Run();
		}
	}
}