using Laba5;

public class Program
{
	private static void Main(string[] args)
	{
		using (var window = new Graphics(1280, 720, "Laba5"))
		{
			window.Run();
		}
	}
}