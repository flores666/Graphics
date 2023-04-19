using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace GraphicsBase
{
	public class Square
	{
		public float[] Vertices = {
			 0.5f,  0.5f, 0.0f, //x y z
			 0.5f, -0.5f, 0.0f,
			-0.5f, -0.5f, 0.0f,
			-0.5f,  0.5f, 0.0f
		};

		public uint[] Indices = {
			0, 1, 3,   // first triangle
			1, 2, 3    // second triangle
		};
	}
}
