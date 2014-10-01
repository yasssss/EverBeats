This package contains a set of glass shader: 
- Simple glass (used only for cubmap reflection) 
- Textured glass 
- Luminous glass 
- And a set of shaders cutout

These opaque geometry shaders are used, so no problems inherent in transparent shaders. 
For correct operation requires shader model 3.0. 
Shader works fine on directx9 and directx11, supports "Forward Rendering" and "Deferred Lighting", as well as working on the free and pro version. 

Shader uses your cubmap, turns the image + imposes certain distortion. You can use the PRO version, "Render to texture" in cubemap glass that will give a more realistic reflection of the given environment. 

Shader parameters, similar to the parameters of standard shader units. For example cutout, diffuse, bumpmap and other, so deal with the appointment of each of them is not difficult. 