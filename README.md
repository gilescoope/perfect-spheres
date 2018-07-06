# perfect-spheres

This is a system for rendering spheres cheaply and accurately. To be used in Unity 2018.2 with the scriptable render pipeline.

## Why use this

Rendering spheres traditionally uses a high number of triangles to achieve a smooth surface - even with 768 triangles the default Unity sphere has visible angles at the edge. On top of this the binary nature of rendering a mesh to pixels means the silhouette of the mesh will be aliased. Due to the easily computable nature of the sphere we can do much better. By placing a quad that faces the camera and doing some smart calculations inside the shader we can render the entire sphere easily on just one or two triangles, the sphere is perfectly round at any magnification and has a smooth anti-aliased edge. This system works seamlessly with Unity's built-in physically based rendering giving beautiful results.

![](https://i.imgur.com/CMgwqRE.png)

![](https://i.imgur.com/SdqMc8p.png)

## Why not use this

This is only for rendering spheres, so any other shape of mesh won't work with this code. At the moment there is no support for casting or receiving shadows. In some environments there may not be a performance increase, as the gain in mesh complexity may be offset with an increase in shader complexity and fill.

## How to use this

To use this you must be using Unity 2018.2 beta or later and either the lightweight or HD render pipeline. Import the files into the "Assets" folder of your Unity project. Place a "Perfect Sphere" prefab into the scene and hit "Play". The rotation and rosition of the sphere can be controlled from the transform of the gameobject. The radius of the sphere can be controlled by the exposed "Radius" variable on the script. Both orthographic and perspective cameras are supported. Scaling is not supported.

To texture the sphere simply assign a texture to the Script before the prefab starts. You can change the number of sides the polygon has (to tighten the mesh at the expense of more triangles) by editing the "numSides" variable in the script.

## How it works

The behaviour script controls the mesh for the sphere, a polygon is created and then on every frame the vertices are manipulated so that they create a polygon that faces the camera, exactly encompass the sphere and are at the correct distance from the camera for sorting. This is all done without editing the position and rotation of the gameobject so these can be used as controls. Normals and tangents are computed for the vertices.

![](https://media.giphy.com/media/wJiDELd6qOxs5FDSOL/giphy.gif)

Next the behaviour script sends the position, rotation and radius to the shader on the material. Inside the shader the world position of the pixel along with a direction vector (for a perspective camera this is the pixel position minus the camera position, for an orthographic camera it is simply the camera direction) are used to calculate the world space intersection (if there is one), from which a world space normal can be calculated. AFter this the world space normal is converted to a tangent space normal and then sent to the PBR normal input node. The alpha channel is calculated as 1 if the ray hits the sphere and 0 otherwise, with fwidth used for anti aliasing at the fringes.

For a textured sphere we have to calculate the original position on the sphere from the world position, here we use a quaternion calculation from my library here https://github.com/gilescoope/shader-graph-nodes/tree/master/Nodes/Quaternions. To rotate our world space normal vector to object space. From this we can extract the polar coordinates which map to our UV for the mesh.

Unfortunately doing this naively when using any non point filtering on our mesh leaves us with a visible seam in the texture. This is due to the texture coordinates looping around for neighbouring pixels.

![](https://i.imgur.com/sfD8Eb7.png)

My fix for this involves sampling the texture twice in the shader to give seams in two different places then interpolating between these values to give a smooth result.

![](https://i.imgur.com/XVtzziU.png)
