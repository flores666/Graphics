#version 330 core

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTexCoord;

out vec3 position;
out vec3 normal;
out vec2 texCoord;

void main ()
{
	position = vec3(model * view * vec4 (vPosition, 1));
	normal = vec3(model * view * vec4 (vNormal, 0));
	texCoord = vec2(vTexCoord.s, 1.0 - vTexCoord.t); 

	gl_Position = projection * view * model * vec4(vPosition, 1.0);
}
