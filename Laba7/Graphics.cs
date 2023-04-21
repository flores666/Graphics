using GraphicsBase.Managers;
using GraphicsBase;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Laba8
{
    public class Graphics : GraphicsMain
	{
		private float _speed = 3.5f;
		private float _sensitivity = 0.2f;

		private Camera _camera;
		private bool _firstMove = true;                     
		private Vector2 _lastMove;

		private Scene _scene;
		private string _scenePath = "../../../DATA/SCENES/small_scene.json";

		public Graphics(int width, int height, string title) : base(width, height, title) 
		{
			FragShaderSource = "../../../DATA/SHADERS/DiffuseTextureInstanced.fsh";
			VertShaderSource = "../../../DATA/SHADERS/DiffuseTextureInstanced.vsh";
		}

		protected override void OnLoad()
		{
			base.OnLoad();

			GL.Enable(EnableCap.DepthTest);

			var instance = RenderManager.GetInstance();
			_camera = new(new Vector3(0f, 1.2f, 15f), (float)Size.X / Size.Y) { Fov = 70f };
			var light = new Light(new Vector4(0f, 0f, 0f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f),
				new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector4(1f, 1f, 1f, 1f));

			instance.SetCamera(_camera);
			_scene = new(_scenePath, _camera, light);
			CursorState = CursorState.Grabbed;
		}
		
		protected override void OnUnload()
		{
			base.OnUnload();

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
			GL.UseProgram(0);
			// Delete all the resources.
			GL.DeleteBuffer(VertexBufferObject);
			GL.DeleteVertexArray(VertexArrayObject);

			GL.DeleteProgram(Shader.Handle);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

			_camera.Move((float)args.Time * 4, _speed, KeyboardState);
			_camera.Rotate(_sensitivity, MouseState, ref _lastMove, ref _firstMove);
			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				Close();
			}
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			Title = "Laba8 " + $"{Fps} fps" + " " + _scene.GetSceneDescription();
			Shader.Use();
			Shader.SetUniform("texture_0", 0);
			_scene.SetObjects(Shader);
			RenderManager.GetInstance().RenderObjects(Shader);
			SwapBuffers();
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			_camera.AspectRatio = Size.X / (float)Size.Y;
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			_camera.Fov -= e.OffsetY;
		}
	}
}
