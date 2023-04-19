using GraphicsBase;
using GraphicsBase.GraphicObjectData;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Laba3
{
    public class Graphics : GraphicsMain
	{
		private float _speed = 1.5f;
		private float _sensitivity = 0.005f;
		private Camera _camera;
		private bool _firstMove = true;
		private Vector2 _lastMove;
		private List<GraphicObject> _graphicObjects;

		public Graphics(int width, int height, string title) : base(width, height, title) { }

		protected override void OnLoad()
		{
			base.OnLoad();

			_camera = new(new Vector3(0f, 1.3f, 3f), (float)Size.X / Size.Y);
			_camera.Fov = 75f;

			InitializeGraphicObjects();

			GL.Enable(EnableCap.DepthTest);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			GLFW.WindowHint(WindowHintInt.Samples, 4);
			GL.Enable(EnableCap.Multisample);

			CursorState = CursorState.Grabbed;
		}
		private void InitializeGraphicObjects()
		{
			var blue = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
			_graphicObjects = new List<GraphicObject>()
			{
				new GraphicObject(new Vector3(0f, 0f, 0f), blue),
				new GraphicObject(new Vector3(0f, 1f, 0f), blue),
				new GraphicObject(new Vector3(0f, 2f, 0f), blue),

				new GraphicObject(new Vector3(0f, 1f, 0f), blue),
				new GraphicObject(new Vector3(0f, 2f, 0f), blue),
				new GraphicObject(new Vector3(0f, 3f, 0f), blue),

				new GraphicObject(new Vector3(1f, 0f, 1f), blue),
			};
			_graphicObjects.ElementAt(1).RotateZ(0.3f);
			_graphicObjects.ElementAt(2).RotateZ(0.6f);

			_graphicObjects.ElementAt(3).RotateZ(-0.3f);
			_graphicObjects.ElementAt(4).RotateZ(-0.5f);
			_graphicObjects.ElementAt(5).RotateZ(-0.7f);

			_graphicObjects.ElementAt(6).RotateY(-0.7f);

			var radius = 4;
			var numObj = 16;
			var green = new Vector4(0.0f, 0.5f, 0.5f, 1.0f);
			for (int i = 0; i < numObj; i++)
			{
				var x = Math.Cos(2 * Math.PI * i / numObj) * radius;
				var y = Math.Sin(2 * Math.PI * i / numObj) * radius;
				_graphicObjects.Add(new GraphicObject(new Vector3((float)x, 0f, (float)y), green));
			}
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

			if(KeyboardState.IsKeyDown(Keys.Escape))
			{
				Close();
			}
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			base.OnRenderFrame(args);

			Title = "Laba3 " + $"{Fps} fps";

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			foreach(var obj in _graphicObjects)
			{
				Shader.SetUniform("model", obj.ModelMatrix);
				Shader.SetUniform("view", _camera.GetViewMatrix());
				Shader.SetUniform("projection", _camera.GetProjectionMatrix());
				Shader.SetUniform("vColor", obj.Color);
				Shader.Use();
				
				obj.Draw(VertexArrayObject, VertexBufferObject, Shader);
			}

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
