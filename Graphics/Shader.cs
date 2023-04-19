using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GraphicsBase
{
	public class Shader
	{
		public int Handle { get; private set; }
		public bool IsDisposed { get; private set; }

		public Shader(string vertexPath, string fragmentPath)
		{
			IsDisposed = false;

			int vertexShader, fragmentShader;
			var vertexShaderSource = File.ReadAllText(vertexPath);
			var fragmentShaderSource = File.ReadAllText(fragmentPath);

			vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexShaderSource);

			fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentShaderSource);

			GL.CompileShader(vertexShader);
			Console.WriteLine("Vertex Shader is OK" + GetAndLogShader(vertexShader));

			GL.CompileShader(fragmentShader);
			Console.WriteLine("Fragment Shader is OK" + GetAndLogShader(fragmentShader));

			Handle = GL.CreateProgram();

			GL.AttachShader(Handle, vertexShader);
			GL.AttachShader(Handle, fragmentShader);

			GL.LinkProgram(Handle);

			GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
			if (success == 0)
			{
				string infoLog = GL.GetProgramInfoLog(Handle);
				Console.WriteLine(infoLog);
			}

			GL.DetachShader(Handle, vertexShader);
			GL.DetachShader(Handle, fragmentShader);
			GL.DeleteShader(fragmentShader);
			GL.DeleteShader(vertexShader);
		}

		private static string GetAndLogShader(int shader)
		{
			GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
			if (success == 0)
			{
				return GL.GetShaderInfoLog(shader);
			}
			return string.Empty;
		}

		public void Use()
		{
			GL.UseProgram(Handle);
		}

		public int GetAttribLocation(string name)
		{
			return GL.GetAttribLocation(Handle, name);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				GL.DeleteProgram(Handle);
				IsDisposed = true;
			}
		}

		~Shader()
		{
			GL.DeleteProgram(Handle);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SetUniform(string name, Vector2 value)
		{
			var location = GL.GetUniformLocation(Handle, name);
			GL.Uniform2(location, ref value);
		}

		public void SetUniform(string name, Vector4 value)
		{
			var location = GL.GetUniformLocation(Handle, name);
			GL.Uniform4(location, ref value);
		}

		public void SetUniform(string name, Matrix4 value)
		{
			var location = GL.GetUniformLocation(Handle, name);
			GL.UniformMatrix4(location, false, ref value);
		}

		public void SetUniform(string name, int value)
		{
			var location = GL.GetUniformLocation(Handle, name);
			GL.Uniform1(location, value);
		}

		public void SetUniform(string name, float value)
		{
			var location = GL.GetUniformLocation(Handle, name);
			GL.Uniform1(location, value);
		}
	}
}
