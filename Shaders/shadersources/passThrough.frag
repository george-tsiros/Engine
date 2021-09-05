#version 460 core
#pragma debug(on)

in vec2 not_FragPos;

uniform sampler2D tex;

out vec4 out0;

void main () { 
    vec4 c = texture(tex, not_FragPos);
    out0 = vec4(c.rgb, 1.0);
}