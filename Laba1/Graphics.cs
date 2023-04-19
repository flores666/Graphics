using GraphicsBase;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Laba1
{
	public class Graphics : GraphicsMain
	{
		public Graphics(int width, int height, string title) : base(width, height, title)
		{
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
		}

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
		}

		protected override void OnUnload()
		{
			base.OnUnload();
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.BindVertexArray(VertexArrayObject);
			GL.DrawElements(PrimitiveType.Triangles, Square.Indices.Length, DrawElementsType.UnsignedInt, 0);

			SwapBuffers();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
		}
	}
}
