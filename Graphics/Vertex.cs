using OpenTK.Mathematics;

namespace GraphicsBase
{
	public class Vertex
	{
		public Vector4 Position { get; private set; }
		public Vector2 TexCoords { get; private set; }
		public Vector3 Normals { get; private set; }

		public float[] VertexArray { get; private set; }

		public Vertex(Vector3 position, Vector2 texCoords, Vector3 normals)
		{
			Position = new Vector4(position, 1f);
			TexCoords = texCoords;
			Normals = normals;

			VertexArray = new float[8];
			VertexArray[0] = Position.X;
			VertexArray[1] = Position.Y;
			VertexArray[2] = Position.Z;

			VertexArray[3] = TexCoords.X;
			VertexArray[4] = TexCoords.Y;

			VertexArray[5] = Normals.X;
			VertexArray[6] = Normals.Y;
			VertexArray[7] = Normals.Z;
		}

		public Vertex(Vector4 value)
		{
			Position = value;
		}
	}
}
