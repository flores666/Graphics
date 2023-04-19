using GraphicsBase.GraphicObjectData;
using GraphicsBase.Managers;
using OpenTK.Mathematics;
using System.Text.Json;

namespace GraphicsBase
{
    public class Scene
	{
		private List<GraphicObject> _graphicObjects;
		private Camera _camera;
		private Light _light;
		private Shader _shader;

		public int RenderedObjectsCount { get; private set; }

		public Scene(string filePath, Camera camera, Light light, Shader shader)
		{
			_graphicObjects = new();
			_camera = camera;
			_light = light;
			_shader = shader;
			Load(filePath);
		}

		public void Load(string filePath)
		{
			using (var reader = new StreamReader(filePath))
			{
				var json = reader.ReadToEnd();
				var document = JsonDocument.Parse(json);
				foreach (var element in document.RootElement.EnumerateArray())
				{
					var model = element.GetProperty("model").Deserialize<string>();
					var position = element.GetProperty("position").Deserialize<float[]>();
					var angle = element.GetProperty("angle").Deserialize<float>();
					CreateGraphicObject(model, new Vector3(position[0], position[1], position[2]), angle);
				}
			}
		}

		private void CreateGraphicObject(string model, Vector3 position, float angle)
		{
			var obj = new GraphicObject(model, position, angle);
			_graphicObjects.Add(obj);
		}

		public string GetSceneDescription()
		{
			var instance = RenderManager.GetInstance();
			return "[" + RenderedObjectsCount + "/" + _graphicObjects.Count + " objects rendered]" +
				   "[" + "Materials changed: " + instance.MaterialsChanged + "]" +
				   "[" + "Textures changed: " + instance.TexturesChanged + "]" +
				   "[" + "Draw calls: " + instance.DrawCalls + "]";
		}

		public void SetObjects()
		{
			RenderedObjectsCount = 0;
			_light.Position = new Vector4(0f, 120f, 0f, 1f) * _camera.GetViewMatrix();
			_shader.SetUniform("lPosition", _light.Position);
			_shader.SetUniform("fogColor", new Vector4(0.33f, 0.65f, 0.9f, 1f));

			foreach (var obj in _graphicObjects)
			{
				if (!obj.IsOnFrustum(_camera)) continue;
				if (!obj.DrawDistanceTest(_camera)) continue;

				RenderManager.GetInstance().AddToRenderQueue(obj);
				RenderedObjectsCount++;
			}
		}
	}
}
