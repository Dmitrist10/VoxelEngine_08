#type vertex
#version 460 core
layout(location = 2) in vec2 aTexCoords;
layout(location = 1) in vec3 aNormal;
layout(location = 0) in vec3 aPosition;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

out vec2 vTexCoords;
out vec3 vNormal;

void main()
{
    vTexCoords = aTexCoords;
    vNormal = aNormal;
    gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
}

#type fragment
#version 460 core
layout(std140, binding = 1) uniform TextureMaterialProperties {
    vec4 Color;
} material;

layout(binding = 0) uniform sampler2D uTexture;

in vec2 vTexCoords;
in vec3 vNormal;

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTexture, vTexCoords);
    if(texColor.a < 0.1)
        discard;
    FragColor = texColor * material.Color;
}
