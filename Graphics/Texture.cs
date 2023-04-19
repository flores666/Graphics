using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace GraphicsBase
{
	public class Texture
	{
		public readonly int Handle;
		public bool IsSetToShader = false;

		public Texture(string filePath)
		{
			Handle = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Handle);

			StbImage.stbi_set_flip_vertically_on_load(0);
			using (Stream stream = File.OpenRead(filePath))
			{
				ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
			}

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void Bind(TextureUnit unit)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}
	}
}
