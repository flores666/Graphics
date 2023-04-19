﻿using GraphicsBase.Managers;
using GraphicsBase;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GraphicsBase.GraphicObjectData;

namespace Laba4
{
    public class Graphics : GraphicsMain
	{
		private float _speed = 2.5f;
		private float _sensitivity = 0.002f;
		private Camera _camera;
		private bool _firstMove = true;                     
		private Vector2 _lastMove;

		private List<GraphicObject> _graphicObjects;

		public Graphics(int width, int height, string title) : base(width, height, title) { }

		protected override void OnLoad()
		{
			base.OnLoad();

			_camera = new(new Vector3(0f, 1.2f, 15f), (float)Size.X / Size.Y);
			_camera.Fov = 70f;

			_graphicObjects = new();
			InitializeObjects();

			GL.Enable(EnableCap.DepthTest);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);

			GLFW.WindowHint(WindowHintInt.Samples, 8);
			GL.Enable(EnableCap.Multisample);

			CursorState = CursorState.Grabbed;
			Shader.Use();
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

			Title = "Laba4 " + $"{Fps} fps";

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			Shader.SetUniform("view", _camera.GetViewMatrix());
			Shader.SetUniform("projection", _camera.GetProjectionMatrix());
			foreach (var obj in _graphicObjects)
			{
				Shader.SetUniform("model", obj.ModelMatrix);
				Shader.SetUniform("vColor", obj.Color);

				var mesh = ResourceManager.GetInstance().GetMesh(obj.MeshId);
				if (mesh != null)
				{
					mesh.Draw(Shader);
				}
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

		private void InitializeObjects()
		{
			var instance = ResourceManager.GetInstance();
			var meshId = instance.LoadMesh("../../../MESHES/buildings/house_2.obj");
			GraphicObject graphicObject = new(new Vector3(0f, 0f, 0f), new Vector4(0.2f, 0.2f, 0.2f, 1f));
			graphicObject.MeshId = meshId;
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/natures/big_tree.obj");
			graphicObject = new(new Vector3(7.5f, -0.75f, 2.5f), new Vector4(0.2f, 0.8f, 0.2f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(0f);
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/natures/big_tree.obj");
			graphicObject = new(new Vector3(-7.5f, -0.75f, 2.5f), new Vector4(0.2f, 0.8f, 0.2f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(0f);
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/vehicles/police_car.obj");
			graphicObject = new(new Vector3(+4.5f, -2.15f, +6.5f), new Vector4(0.2f, 0.2f, 1.0f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(40f);
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/vehicles/police_car.obj");
			graphicObject = new(new Vector3(+4.25f, -2.15f, +10.5f), new Vector4(0.23f, 0.23f, 1.0f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(-45f);
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/vehicles/jeep.obj");
			graphicObject = new(new Vector3(-3.5f, -2.15f, +9.0f), new Vector4(0.95f, 0.13f, 0.13f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(-3f);
			_graphicObjects.Add(graphicObject);

			meshId = instance.LoadMesh("../../../MESHES/vehicles/car.obj");
			graphicObject = new(new Vector3(0.25f, -2.15f, +9.0f), new Vector4(0.95f, 0.13f, 0.13f, 1f));
			graphicObject.MeshId = meshId;
			graphicObject.RotateY(-17f);
			_graphicObjects.Add(graphicObject);
		}
	}
}
