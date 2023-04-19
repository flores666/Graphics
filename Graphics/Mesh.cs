using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Globalization;

namespace GraphicsBase
{
	public class Mesh
	{
		private int _VAO;
		private int _vertexBuffer;
		private int _indexBuffer;
		private bool isVAOConfigured = false;

		public float[] Vertices { get; private set; }
		public int[] Indices { get; private set; }

		public Mesh(string filePath)
		{
			Load(filePath);
			_VAO = GL.GenVertexArray();
			_vertexBuffer = GL.GenBuffer();
			_indexBuffer = GL.GenBuffer();
		}

		private void Load(string filePath)
		{
			List<Vector3> vertices = new();
			List<Vector2> texCoords = new();
			List<Vector3> normals = new();
			List<int> indices = new();
			List<Vertex> outVertices = new();

			var fileLines = File.ReadAllLines(filePath);
			var cInfo = CultureInfo.InvariantCulture;
			foreach (var line in fileLines)
			{
				string[] tokens = line.Split(' ');
				if (tokens[0] == "v")
				{
					var vert = new Vector3(float.Parse(tokens[1], cInfo), float.Parse(tokens[2], cInfo), float.Parse(tokens[3], cInfo));
					vertices.Add(vert);
				}
				else if (tokens[0] == "vt")
				{
					var texCoord = new Vector2(float.Parse(tokens[1], cInfo), float.Parse(tokens[2], cInfo));
					texCoords.Add(texCoord);
				}
				else if (tokens[0] == "vn")
				{
					var normal = new Vector3(float.Parse(tokens[1], cInfo), float.Parse(tokens[2], cInfo), float.Parse(tokens[3], cInfo));
					normals.Add(normal);
				}
				else if (tokens[0] == "f")
				{
					for (var i = 1; i < tokens.Length; i++)
					{
						var vertexData = tokens[i].Split("/");
						var X = int.Parse(vertexData[0]) - 1;
						var Y = int.Parse(vertexData[1]) - 1;
						var Z = int.Parse(vertexData[2]) - 1;
						outVertices.Add(new Vertex(vertices[X], texCoords[Y], normals[Z]));
						indices.Add(indices.Count);
					}
				}
			}

			Vertices = VertexArrayToFloatAray(outVertices.ToArray());
			Indices = indices.ToArray();
		}

		private static float[] VertexArrayToFloatAray(Vertex[] vertices)
		{
			var i = 0;
			var result = new float[vertices.Length * 8];
			foreach (var vertex in vertices)
			{
				result[i++] = vertex.VertexArray[0]; //vertex coordinates
				result[i++] = vertex.VertexArray[1];
				result[i++] = vertex.VertexArray[2];

				result[i++] = vertex.VertexArray[3]; //texture coordinates
				result[i++] = vertex.VertexArray[4];

				result[i++] = vertex.VertexArray[5]; //normales
				result[i++] = vertex.VertexArray[6];
				result[i++] = vertex.VertexArray[7];
			}
			return result;
		}

		public void ConfigureVAO(Shader shader)
		{
			if (isVAOConfigured) return;
			var stride = 8 * sizeof(float);

			GL.BindVertexArray(_VAO);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

			var index = shader.GetAttribLocation("vPosition");
			GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, stride, 0);
			GL.EnableVertexAttribArray(index);

			index = shader.GetAttribLocation("vTexCoord");
			GL.VertexAttribPointer(index, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
			GL.EnableVertexAttribArray(index);

			index = shader.GetAttribLocation("vNormal");
			GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
			GL.EnableVertexAttribArray(index);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.StaticDraw);
			isVAOConfigured = true;
			GL.BindVertexArray(0);
		}

		public void Draw(Shader shader)
		{
			ConfigureVAO(shader);
			GL.BindVertexArray(_VAO);
			GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);
		}

		public void DrawMany(Shader shader, int instanceCount)
		{
			ConfigureVAO(shader);
			GL.BindVertexArray(_VAO);
			GL.DrawElementsInstanced(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0, instanceCount);
			GL.BindVertexArray(0);
		}
	}
}
