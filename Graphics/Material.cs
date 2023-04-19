using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace GraphicsBase
{
	public class Material
	{
		private float[] ambient;
		private float[] diffuse;
		private float[] specular;
		private float shininess;
		public bool IsSetToShader = false;

		public Vector4 Ambient
		{
			get
			{
				return new(ambient[0], ambient[1], ambient[2], ambient[3]);
			}
			set
			{
				ambient[0] = value.X;
				ambient[1] = value.Y;
				ambient[2] = value.Z;
				ambient[3] = value.W;
			}
		}
		public Vector4 Diffuse
		{
			get
			{
				return new(diffuse[0], diffuse[1], diffuse[2], diffuse[3]);
			}
			set
			{
				diffuse[0] = value.X;
				diffuse[1] = value.Y;
				diffuse[2] = value.Z;
				diffuse[3] = value.W;
			}
		}
		public Vector4 Specular
		{
			get
			{
				return new(specular[0], specular[1], specular[2], specular[3]);
			}
			set
			{
				specular[0] = value.X;
				specular[1] = value.Y;
				specular[2] = value.Z;
				specular[3] = value.W;
			}
		}
		public float Shininess => shininess;

		public Material(string filePath)
		{
			ambient = new float[4];
			diffuse = new float[4];
			specular = new float[4];

			Load(filePath);
		}

		[JsonConstructor]
		public Material(float[] ambient, float[] diffuse, float[] specular, float shininess)
		{
			this.ambient = ambient;
			this.diffuse = diffuse;
			this.specular = specular;
			this.shininess = shininess;
		}

		private void Load(string filePath)
		{
			using (var reader = new StreamReader(filePath))
			{
				var json = reader.ReadToEnd();
				var material = JsonConvert.DeserializeObject<Material>(json);
				ambient = material.ambient;
				diffuse = material.diffuse;
				specular = material.specular;
				shininess = material.shininess;
			}
		}

		public void SetToShader(Shader shader)
		{
			shader.SetUniform("mAmbient", Ambient);
			shader.SetUniform("mDiffuse", Diffuse);
			shader.SetUniform("mSpecular", Specular);
			shader.SetUniform("mShininess", Shininess);
		}
	}
}
