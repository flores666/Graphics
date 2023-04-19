using GraphicsBase.Managers;
using OpenTK.Mathematics;
using System.Text.Json;

namespace GraphicsBase.GraphicObjectData
{
    public class GraphicObject
    {
        private Matrix4 _modelMatrix;
        private float _angle;
        private Vector3 _position;
        private Vector3 _dimensions = new();
        private List<Vector4> _vertices = new();
        private string _modelsPath = "../../../DATA/models.json";

        public Vector4 Color { get; set; }
        public Matrix4 ModelMatrix => _modelMatrix;
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public int TextureId { get; set; }
        public int MeshId { get; set; }
        public int MaterialId { get; set; }
        public float Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                RotateY(value);
            }
        }
        public GraphicObjectType Type { get; private set; }
        public Vector3 Dimensions 
        {
            get => _dimensions;
            set
            {
                _dimensions = value;
                var x = _dimensions.X / 2;
                var y = _dimensions.Y / 2;
                var z = _dimensions.Z / 2;
                _vertices.Add(new (+x, +y, +z, 1f));
                _vertices.Add(new (+x, +y, -z, 1f));
                _vertices.Add(new (+x, -y, +z, 1f));
                _vertices.Add(new (+x, -y, -z, 1f));
                _vertices.Add(new (-x, +y, +z, 1f));
                _vertices.Add(new (-x, +y, -z, 1f));
                _vertices.Add(new (-x, -y, +z, 1f));
                _vertices.Add(new (-x, -y, -z, 1f));
            }
        }

        public GraphicObject() { }
        
        public GraphicObject(Vector3 position)
        {
            _position = position;
            _modelMatrix = Matrix4.CreateTranslation(position);
        }

        public GraphicObject(Vector3 position, Vector4 color)
        {
            _position = position;
            _modelMatrix = Matrix4.CreateTranslation(position);
            Color = color;
        }

        public GraphicObject(string model, Vector3 position, float angle)
        {
            _position = position;
            _modelMatrix = Matrix4.CreateTranslation(position);
            Angle = angle;
            ParseModel(model);
        }

        private void ParseModel(string model)
        {
            using (var reader = new StreamReader(_modelsPath))
            {
                var json = reader.ReadToEnd();
                var document = JsonDocument.Parse(json);
                foreach (var element in document.RootElement.EnumerateObject())
                {
                    if (element.Name.Equals(model))
                    {
                        var current = element.Value;
                        var type = current.GetProperty("type").Deserialize<string>();
                        Type = (GraphicObjectType)Enum.Parse(typeof(GraphicObjectType), type.Replace(" ", "_"));

                        var dimensions = current.GetProperty("dimensions").Deserialize<float[]>();
                        Dimensions = new Vector3(dimensions[0], dimensions[1], dimensions[2]);

                        var meshPath = "../../../" + current.GetProperty("mesh").Deserialize<string>();
                        var texturePath = "../../../" + current.GetProperty("texture").Deserialize<string>();
                        var materialPath = "../../../" + current.GetProperty("material").Deserialize<string>();
                        ConfigureObj(meshPath, materialPath, texturePath);
                    }
                }
            }
        }

        private void ConfigureObj(string meshPath, string materialPath, string texturePath)
        {
            var instance = ResourceManager.GetInstance();
            var meshId = instance.LoadMesh(meshPath);
            MeshId = meshId;
            var textureId = instance.LoadTexture(texturePath);
            TextureId = textureId;
            var materialId = instance.LoadMaterial(materialPath);
            MaterialId = materialId;
        }

        public void RotateX(float value)
        {
            value = MathHelper.DegreesToRadians(value);
			var rotationMatrix = Matrix4.CreateRotationY(value);
			rotationMatrix.Transpose();
			if (value != 0f) _modelMatrix = rotationMatrix * _modelMatrix;
        }

        public void RotateY(float value)
        {
            value = MathHelper.DegreesToRadians(value);
            var rotationMatrix = Matrix4.CreateRotationY(value);
            rotationMatrix.Transpose();
			if (value != 0f) _modelMatrix = rotationMatrix * _modelMatrix;
        }

        public void RotateZ(float value)
        {
            value = MathHelper.DegreesToRadians(value);
			var rotationMatrix = Matrix4.CreateRotationY(value);
			rotationMatrix.Transpose();
			if (value != 0f) _modelMatrix = rotationMatrix * _modelMatrix;
        }

        public void Scale(int value)
        {
            _modelMatrix *= Matrix4.CreateScale(new Vector3(1f / value, 1f / value, 1f / value));
        }

        public void Draw(int VAO, int VBO, Shader shader)
        {
           /* VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VBO);

            var positionIndex = shader.GetAttribLocation("vPosition");
            GL.EnableVertexAttribArray(positionIndex);
            GL.VertexAttribPointer(positionIndex, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            var colorIndex = shader.GetAttribLocation("vColor");
            GL.EnableVertexAttribArray(colorIndex);
            GL.VertexAttribPointer(colorIndex, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length);*/
        }

        public bool DrawDistanceTest(Camera camera)
        {
            float distance = Vector3.Distance(camera.Position, Position);
            switch (Type)
            {
                case GraphicObjectType.vehicle:
                    if (distance > 400) return false;
                    break;
                case GraphicObjectType.big_nature:
                    if (distance > 250) return false;
                    break;
                case GraphicObjectType.small_nature:
                    if (distance > 100) return false;
                    break;
                case GraphicObjectType.big_prop:
                    if (distance > 300) return false;
                    break;
                case GraphicObjectType.medium_prop:
                    if (distance > 200) return false;
                    break;
                case GraphicObjectType.small_prop:
                    if (distance > 100) return false;
                    break;
            }
            return true;
        }

        public bool IsOnFrustum(Camera camera)
        {
            var view = camera.GetViewMatrix();
            var projection = camera.GetProjectionMatrix();
            var model = _modelMatrix;
            view.Transpose();
            projection.Transpose();
            model.Transpose();
            var PVM = projection * view * model;
            var obbPoints = new Vector4[8];
            for (int i = 0; i < 8; i++)
            {
                obbPoints[i] = PVM * _vertices[i];
            }
            bool outside = false, outsidePositivePlane, outsideNegativePlane;
            for (int i = 0; i < 3; i++)
            {
                outsidePositivePlane =
                    obbPoints[0][i] > obbPoints[0].W &&
                    obbPoints[1][i] > obbPoints[1].W &&
                    obbPoints[2][i] > obbPoints[2].W &&
                    obbPoints[3][i] > obbPoints[3].W &&
                    obbPoints[4][i] > obbPoints[4].W &&
                    obbPoints[5][i] > obbPoints[5].W &&
                    obbPoints[6][i] > obbPoints[6].W &&
                    obbPoints[7][i] > obbPoints[7].W;
                outsideNegativePlane =
                    obbPoints[0][i] < -obbPoints[0].W &&
                    obbPoints[1][i] < -obbPoints[1].W &&
                    obbPoints[2][i] < -obbPoints[2].W &&
                    obbPoints[3][i] < -obbPoints[3].W &&
                    obbPoints[4][i] < -obbPoints[4].W &&
                    obbPoints[5][i] < -obbPoints[5].W &&
                    obbPoints[6][i] < -obbPoints[6].W &&
                    obbPoints[7][i] < -obbPoints[7].W;
                outside = outside || outsidePositivePlane || outsideNegativePlane;
            }
            return !outside;
        }
    }
}
