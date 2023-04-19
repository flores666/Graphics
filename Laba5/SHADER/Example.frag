#version 330 core

in vec2 texCoordOut;

out vec4 fragColor;

uniform sampler2D texture_0;

void main()
{
    fragColor = texture(texture_0, texCoordOut);
}