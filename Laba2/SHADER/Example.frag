#version 330 core

uniform vec4 color1;
uniform vec4 color2;

in vec2 position;

out vec4 fragColor;

void main ()
{
	vec4 White = vec4(1.0, 1.0, 1.0, 1.0);
    vec4 Blue = vec4(0.0, 0.0, 1.0, 1.0);
    vec4 Red = vec4(1.0, 0.0, 0.0, 1.0);

    vec2 center = position.xy;
    float radius = 200.0;

    float angle = atan(center.y, center.x);

   if (mod(angle + 3.1415926, 2.0 * 3.1415926 / 13.0) < 3.1415926 / 13.0) {
        fragColor = Red;
    } else if (mod(angle + 3.1415926 / 13.0, 2.0 * 3.1415926 / 13.0) < 3.1415926 / 13.0) {
        fragColor = White;
    }   else if (position.x*position.x+position.y*position.y < 0.03) { fragColor = Red; }
    else {
        fragColor = mix(Blue, White, radius * 2.0);
    }
}
