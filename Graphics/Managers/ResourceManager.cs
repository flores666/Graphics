namespace GraphicsBase.Managers
{
    public class ResourceManager
    {
        protected Dictionary<string, Mesh> _meshes = new();
        protected Dictionary<string, Texture> _textures = new();
        protected Dictionary<string, Material> _materials = new();

        private static ResourceManager _instance;
        private ResourceManager() { }

        public static ResourceManager GetInstance()
        {
            _instance ??= new ResourceManager();
            return _instance;
        }

        public int LoadMesh(string filePath)
        {
			if (!_meshes.ContainsKey(filePath))
            {
			    var mesh = new Mesh(filePath);
				_meshes.Add(filePath, mesh);
			}

            return _meshes.IndexOfKey(filePath);
		}

        public Mesh GetMesh(int index)
        {
            return _meshes.ElementAt(index).Value;
        }

        public int LoadTexture(string filePath)
        {
            if (!_textures.ContainsKey(filePath))
            {
                var texture = new Texture(filePath);
                _textures.Add(filePath, texture);
			}
            
            return _textures.IndexOfKey(filePath);
		}

        public Texture GetTexture(int index)
        {
			return _textures.ElementAt(index).Value;
		}

		public int LoadMaterial(string filePath)
		{
            if (!_materials.ContainsKey(filePath))
            {
			    var material = new Material(filePath);
				_materials.Add(filePath, material);
            }
			return _materials.IndexOfKey(filePath);
		}

		public Material GetMaterial(int index)
		{
			return _materials.ElementAt(index).Value;
		}
	}
}
