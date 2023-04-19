using OpenTK.Mathematics;

namespace GraphicsBase
{
	public class Light
	{
		public Vector4 Position { get; set; }
		public Vector4 Ambient { get; set; }
		public Vector4 Diffuse { get; set; }
		public Vector4 Specular { get; set; }
		public bool IsSetToShader = false;

		public Light() { }
		public Light(Vector4 position, Vector4 ambient, Vector4 diffuse, Vector4 specular)
		{
			Position = position;
			Ambient = ambient;
			Diffuse = diffuse;
			Specular = specular;
		}

		public void SetToShader(Shader shader)
		{
			shader.SetUniform("lAmbient", Ambient);
			shader.SetUniform("lDiffuse", Diffuse);
			shader.SetUniform("lSpecular", Specular);
			shader.SetUniform("lPosition", Position);
		}
	}
}
