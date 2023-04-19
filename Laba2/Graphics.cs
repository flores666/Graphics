using GraphicsBase;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Laba2
{
	public class Graphics : GraphicsMain
	{
		private Vector2 _offset = new(0.25f, 0f);
		private Vector2 _speed = new(+0.30f, -0.25f);
		private Vector4 _color1 = new(0.0f, 0.4f, 0.8f, 1f);
		private Vector4 _color2 = new(1.0f, 1.0f, 0f, 1f);
		private Vector4 _color3 = new(0f, 1f, 0f, 1f);

		public Graphics(int width, int height, string title) : base(width, height, title) { }

		protected override void OnLoad()
		{
			base.OnLoad();
			// 1. bind Vertex Array Object
			GL.BindVertexArray(VertexArrayObject);
			// 2. copy vertices array in a buffer for OpenGL to use
			GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, Square.Indices.Length * sizeof(uint),
				Square.Indices, BufferUsageHint.StaticDraw);
			GL.BufferData(BufferTarget.ArrayBuffer, Square.Vertices.Length * sizeof(float),
				Square.Vertices, BufferUsageHint.StaticDraw);

			// 3. set vertex attributes pointers
			var index = Shader.GetAttribLocation("vPosition");
			GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, 0, 0);
			GL.EnableVertexAttribArray(0);

			Shader.Use();

			Shader.SetUniform("color1", _color1);
			Shader.SetUniform("color2", _color2);
		}

		protected override void OnUnload()
		{
			base.OnUnload();
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

			GL.Clear(ClearBufferMask.ColorBufferBit);

			Shader.SetUniform("offset", _offset);
			MoveSquare((float)args.Time);

			GL.BindVertexArray(VertexArrayObject);
		}

		private void MoveSquare(float simulationTime)
		{
			if (_offset.X >= 0.5f || _offset.X <= -0.5f)
			{
				_speed.X *= -1;
			}

			if (_offset.Y >= 0.5f || _offset.Y <= -0.5f)
			{
				_speed.Y *= -1;
			}

			_offset += _speed * simulationTime;
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			Title = "Laba2 " + $"{Fps} fps";
			GL.DrawElements(PrimitiveType.Triangles, Square.Indices.Length, DrawElementsType.UnsignedInt, 0);

			SwapBuffers();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
		}
	}
}
