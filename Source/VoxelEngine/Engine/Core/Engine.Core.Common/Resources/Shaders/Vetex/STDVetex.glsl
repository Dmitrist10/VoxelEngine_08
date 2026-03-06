#type vertex
#version 460 core
layout(location = 0) in vec3 aPosition;

layout(std140, binding = 0) uniform CameraBlock {
    mat4 view;
    mat4 projection;
} camera;

layout(std140, binding = 2) uniform ModelBlock {
    mat4 model;
} modelData;

void main()
{
    gl_Position = camera.projection * camera.view * modelData.model * vec4(aPosition, 1.0);
}
