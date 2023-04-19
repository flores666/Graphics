using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

namespace GraphicsBase
{
	public class GraphicsMain : GameWindow
	{
		protected int VertexBufferObject { get; set; }
		protected int ElementBufferObject { get; set; }
		protected int VertexArrayObject { get; set; }
		protected Square Square { get; set; }
		protected Shader Shader { get; set; }
		protected string FragShaderSource { get; set; }
		protected string VertShaderSource { get; set; }

		protected Stopwatch _fpsStopwatch;
		protected int _frameCount;
		protected int Fps { get; set; }

		protected GraphicsMain(int width, int height, string title)
			: base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) 
		{
			VSync = VSyncMode.On;
			VertShaderSource = "../../../SHADER/Example.vert";
			FragShaderSource = "../../../SHADER/Example.frag";
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			_fpsStopwatch = new Stopwatch();
			_fpsStopwatch.Start();

			GL.ClearColor(0.8f, 1f, 1f, 1f);

			//VertexBufferObject = GL.GenBuffer();
			//ElementBufferObject = GL.GenBuffer();
			//VertexArrayObject = GL.GenVertexArray();

			//Square = new Square();
			Shader = new Shader(VertShaderSource, FragShaderSource);
		}

		protected override void OnUnload()
		{
			base.OnUnload();

			Shader.Dispose();
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			CountFps();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, e.Width, e.Height);
		}

		private void CountFps()
		{
			_frameCount++;
			if (_fpsStopwatch.ElapsedMilliseconds > 1000)
			{
				Fps = _frameCount;
				_frameCount = 0;
				_fpsStopwatch.Restart();
			}
		}
	}
}