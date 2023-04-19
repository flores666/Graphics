#version 330 core

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec2 vTexCoord;
layout (location = 2) in vec2 vNormal;

out vec2 texCoordOut;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	texCoordOut = vec2(vTexCoord.s, 1.0 - vTexCoord.t);
    gl_Position = projection * view * model * vec4(vPosition, 1.0);
}