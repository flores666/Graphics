using GraphicsBase.GraphicObjectData;
using GraphicsBase.GraphicObjectData.Comparers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GraphicsBase.Managers
{
    public class RenderManager
	{
		private static RenderManager _instance;
		public List<GraphicObject> _graphicObjects = new();
		private Shader _shader;
		private Camera _camera;

		public int MaterialsChanged { get; private set; }
		public int TexturesChanged { get; private set; }
		public int DrawCalls { get; private set; }

		private RenderManager() { }

		public static RenderManager GetInstance()
		{
			_instance ??= new RenderManager();
			return _instance;
		}

		public void AddToRenderQueue(GraphicObject obj)
		{
			_graphicObjects.Add(obj);
		}

		public void SetCamera(Camera camera)
		{
			_camera = camera;
		}

		public void SetShader(Shader shader)
		{
			_shader = shader;
			_shader.Use();
		}

		private void ClearQueue()
		{
			if(_graphicObjects.Count > 0) _graphicObjects.Clear();
		}

		private void DrawFirst(Shader shader)
		{
			var obj = _graphicObjects.FirstOrDefault();
			if (obj == default(GraphicObject)) return;
			shader.SetUniform("model[0]", obj.ModelMatrix);
			var instance = ResourceManager.GetInstance();
			instance.GetTexture(obj.TextureId)?.Bind(TextureUnit.Texture0);
			TexturesChanged++;
			instance.GetMaterial(obj.MaterialId)?.SetToShader(shader);
			MaterialsChanged++;
		}

		public void RenderObjects(Shader shader)
		{
			TexturesChanged = 0;
			MaterialsChanged = 0;
			DrawCalls = 0;
			shader.Use();
            if (_graphicObjects.Count == 0) return;
			_graphicObjects.Sort(new GraphicObjectsComparer());
			shader.SetUniform("view", _camera.GetViewMatrix());
			shader.SetUniform("projection", _camera.GetProjectionMatrix());
			//shader.SetUniform("texture_0", 0);

			DrawFirst(shader);
			var sameMeshesCount = 0;
			var sameModels = new List<Matrix4>();
			var instance = ResourceManager.GetInstance();
			for (int i = 1; i < _graphicObjects.Count; i++)
			{
				var prevObj = _graphicObjects[i - 1];
				var obj = _graphicObjects[i];
				sameModels.Add(prevObj.ModelMatrix);
				sameMeshesCount++;

				if (prevObj.MeshId != obj.MeshId || prevObj.TextureId != obj.TextureId || sameMeshesCount >= 20)
				{
					for (int j = 0; j < sameModels.Count; j++)
					{
						shader.SetUniform($"model[{j}]", sameModels[j]);
					}
					var mesh = instance.GetMesh(prevObj.MeshId);
					mesh?.DrawMany(shader, sameMeshesCount);
					DrawCalls++;
					sameMeshesCount = 0;
					sameModels.Clear();
				}
				if (prevObj.TextureId != obj.TextureId)
				{
					instance.GetTexture(obj.TextureId)?.Bind(TextureUnit.Texture0);
					TexturesChanged++;
				}
				if (prevObj.MaterialId != obj.MaterialId)
				{
					instance.GetMaterial(obj.MaterialId)?.SetToShader(shader);
					MaterialsChanged++;
				}
			}
			ClearQueue();
			shader.Dispose();
		}
	}
}
