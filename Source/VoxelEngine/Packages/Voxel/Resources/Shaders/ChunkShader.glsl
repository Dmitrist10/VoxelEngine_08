#type vertex
#version 460 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;


out vec3 vNormal;
out vec2 vTexCoord;

void main()
{
    gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
    vNormal = aNormal;
    vTexCoord = aTexCoord;
}

#type fragment
#version 460 core

in vec3 vNormal;
in vec2 vTexCoord;

layout(std140, binding = 1) uniform PBRMaterialProperties {
    vec4 Color;
} material;

out vec4 FragColor;

void main()
{
    FragColor = material.Color;
}


