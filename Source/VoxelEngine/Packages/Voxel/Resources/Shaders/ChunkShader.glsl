#type vertex
#version 460 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aUV;
layout(location = 2) in float aTextureLayer;
layout(location = 3) in float aFace;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

out vec2 vUV;
flat out float vTextureLayer;
flat out float vFace;

void main()
{
    gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
    vUV = aUV;
    vTextureLayer = aTextureLayer;
    vFace = aFace;
}

#type fragment
#version 460 core

in vec2 vUV;
flat in float vTextureLayer;
flat in float vFace;

layout(std140, binding = 1) uniform PBRMaterialProperties {
    vec4 Color;
} material;

uniform sampler2DArray uTextureArray;

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTextureArray, vec3(vUV, vTextureLayer));
    FragColor = texColor * material.Color;
}
