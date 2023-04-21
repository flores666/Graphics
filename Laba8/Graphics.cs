using GraphicsBase.Managers;
using GraphicsBase;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

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

		private List<Shader> _shaders = new List<Shader>();
		private Light _light = new Light();
		private FrameBufferObject _fbo0 = new FrameBufferObject();
		private FrameBufferObject _fbo1 = new FrameBufferObject();
		private int _currentShader = 0;
		private Stopwatch _stopwatch = new Stopwatch();

		public Graphics(int width, int height, string title) : base(width, height, title) 
		{
			FragShaderSource = "../../../DATA/SHADERS/DiffuseTextureInstanced.fsh";
			VertShaderSource = "../../../DATA/SHADERS/DiffuseTextureInstanced.vsh";
		}

		protected override void OnLoad()
		{
			base.OnLoad();
			_stopwatch.Start();
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.TextureColorTableSgi);
			GL.Enable(EnableCap.DepthClamp);

			_fbo0.Init(Size.X, Size.Y, true);
			_fbo0.Unbind();
			_fbo1.Init(Size.X, Size.Y, false);
			_fbo1.Unbind();

			var instance = RenderManager.GetInstance();
			_camera = new(new Vector3(0f, 1.2f, 15f), (float)Size.X / Size.Y) { Fov = 70f };
			_light = new Light(new Vector4(0f, 0f, 0f, 1f), new Vector4(0.6f, 0.6f, 0.6f, 1f),
				new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector4(1f, 1f, 1f, 1f));

			_shaders.Add(new Shader("../../../DATA/SHADERS/SimplePostProcessing.fsh",
				"../../../DATA/SHADERS/SimplePostProcessing.fsh"));

			_shaders.Add(new Shader("../../../DATA/SHADERS/GreyPostProcessing.fsh",
				"../../../DATA/SHADERS/GreyPostProcessing.fsh"));

			_shaders.Add(new Shader("../../../DATA/SHADERS/SepiaPostProcessing.fsh",
				"../../../DATA/SHADERS/SepiaPostProcessing.fsh"));

			instance.SetCamera(_camera);
			_scene = new(_scenePath, _camera, _light);
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
			if (KeyboardState.IsKeyDown(Keys.Enter))
			{
				if (_stopwatch.ElapsedMilliseconds >= 500)
				{
					_currentShader++;
					_stopwatch.Restart();
				}
				if (_currentShader >= _shaders.Count)
				{
					_currentShader = 0;
				}
			}
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			/*_fbo0.Bind();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);*/

			Title = "Laba8 " + $"{Fps} fps" + " " + _scene.GetSceneDescription();

			_scene.SetObjects(Shader);
			RenderManager.GetInstance().RenderObjects(Shader);

			/*_fbo0.ResolveToFBO(_fbo1);
			_fbo0.Unbind();
			_fbo1.BindColorTexture();
			_shaders[_currentShader].Use();
			_shaders[_currentShader].SetUniform("texture_0", 0);*/
			DrawBox();
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

		private void DrawBox()
		{
			int VAO_Index = 0;
			int VBO_Index = 0;
			int VertexCount = 0;
			bool Init = true;
			if (Init)
			{
				Init = false;
				VBO_Index = GL.GenBuffer();
				GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_Index);
				float[] Verteces = {
					-0.5f, +0.5f,
					-0.5f, -0.5f,
					+0.5f, +0.5f,
					+0.5f, +0.5f,
					-0.5f, -0.5f,
					+0.5f, -0.5f
				};
				GL.BufferData(BufferTarget.ArrayBuffer, Verteces.Length * sizeof(float), Verteces, BufferUsageHint.StaticDraw);

				VAO_Index = GL.GenVertexArray();
				GL.BindVertexArray(VAO_Index);

				GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_Index);
				int location = 0;
				GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, 0, 0);
				GL.EnableVertexAttribArray(location);

				GL.BindVertexArray(0);

				VertexCount = 6;
			}
			GL.BindVertexArray(VAO_Index);
			GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
		}
	}
}
