#version 330 core

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec4 vColor;

out vec4 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
	color = vColor;
	gl_Position = projection * view * model * vec4(vPosition, 1.0);
}