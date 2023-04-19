using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GraphicsBase
{
	public class Camera
	{
		// Those vectors are directions pointing outwards from the camera to define how it rotated.
		private Vector3 _front = -Vector3.UnitZ;
		private Vector3 _up = Vector3.UnitY;
		private Vector3 _right = Vector3.UnitX;

		// Rotation around the X (radians)
		private float _pitch;
		// Rotation around the Y (radians)
		private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.
		// The field of view (radians)
		private float _fov = MathHelper.PiOver2;

		public Camera(Vector3 position, float aspectRatio)
		{
			Position = position;
			AspectRatio = aspectRatio;
		}

		// The position of the camera
		public Vector3 Position { get; set; }
		// This is simply the aspect ratio of the viewport, used for the projection matrix.
		public float AspectRatio { get; set; }

		public Vector3 Front => _front;
		public Vector3 Up => _up;
		public Vector3 Right => _right;

		// We convert from degrees to radians as soon as the property is set to improve performance.
		public float Pitch
		{
			get => MathHelper.RadiansToDegrees(_pitch);
			set
			{
				// We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
				// of weird "bugs" when you are using euler angles for rotation.
				// If you want to read more about this you can try researching a topic called gimbal lock
				var angle = MathHelper.Clamp(value, -89f, 89f);
				_pitch = MathHelper.DegreesToRadians(angle);
				UpdateVectors();
			}
		}

		// We convert from degrees to radians as soon as the property is set to improve performance.
		public float Yaw
		{
			get => MathHelper.RadiansToDegrees(_yaw);
			set
			{
				_yaw = MathHelper.DegreesToRadians(value);
				UpdateVectors();
			}
		}

		// The field of view (FOV) is the vertical angle of the camera view.
		// This has been discussed more in depth in a previous tutorial,
		// but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
		// We convert from degrees to radians as soon as the property is set to improve performance.
		public float Fov
		{
			get => MathHelper.RadiansToDegrees(_fov);
			set
			{
				var angle = MathHelper.Clamp(value, 1f, 90f);
				_fov = MathHelper.DegreesToRadians(angle);
			}
		}

		// Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
		public Matrix4 GetViewMatrix()
		{
			return Matrix4.LookAt(Position, Position + _front, _up);
		}

		// Get the projection matrix using the same method we have used up until this point
		public Matrix4 GetProjectionMatrix()
		{
			return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
		}

		// This function is going to update the direction vertices using some of the math learned in the web tutorials.
		private void UpdateVectors()
		{
			// First, the front matrix is calculated using some basic trigonometry.
			_front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
			_front.Y = MathF.Sin(_pitch);
			_front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

			// We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
			_front = Vector3.Normalize(_front);

			// Calculate both the right and the up vector using cross product.
			// Note that we are calculating the right from the global up; this behaviour might
			// not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
			_right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
			_up = Vector3.Normalize(Vector3.Cross(_right, _front));
		}

		public void Move(float simulationTime, float speed, KeyboardState input)
		{
			if (input.IsKeyDown(Keys.W))
			{
				Position += _front * speed * (float)simulationTime; //Forward 
			}

			if (input.IsKeyDown(Keys.S))
			{
				Position -= _front * speed * (float)simulationTime; //Backwards
			}

			if (input.IsKeyDown(Keys.A))
			{
				Position -= Vector3.Normalize(Vector3.Cross(_front, _up)) * speed * (float)simulationTime; //Left
			}

			if (input.IsKeyDown(Keys.D))
			{
				Position += Vector3.Normalize(Vector3.Cross(_front, _up)) * speed * (float)simulationTime; //Right
			}

			if (input.IsKeyDown(Keys.Space))
			{
				Position += _up * speed * (float)simulationTime; //Up 
			}

			if (input.IsKeyDown(Keys.LeftShift))
			{
				Position -= _up * speed * (float)simulationTime; //Down
			}
		}

		public void Rotate(float sensitivity, MouseState mouse, ref Vector2 lastPos, ref bool firstMove)
		{
			if (firstMove) // This bool variable is initially set to true.
			{
				lastPos = new Vector2(mouse.X, mouse.Y);
				firstMove = false;
			}
			else
			{
				// Calculate the offset of the mouse position
				var deltaX = mouse.X - lastPos.X;
				var deltaY = mouse.Y - lastPos.Y;
				lastPos = new Vector2(mouse.X, mouse.Y);

				// Apply the camera pitch and yaw (we clamp the pitch in the camera class)
				Yaw += deltaX * sensitivity;
				Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
			}
		}
	}
}
