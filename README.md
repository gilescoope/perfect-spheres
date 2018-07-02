# perfect-spheres

This is a system for rendering spheres cheaply and accurately in Unity.

## Why use this

Rendering spheres traditionally uses a high number of triangles to achieve a smooth surface - even with 768 triangles the default Unity sphere has visible angles at the edge. On top of this the binary nature of rendering a mesh to pixels means the silhouette of the mesh will be aliased. Due to the easily computable nature of the sphere we can do much better. By placing a quad that faces the camera and doing some smart calculations inside the shader we can render the entire sphere easily on just one or two triangles, the sphere is perfectly round at any magnification and has a smooth anti-aliased edge. This system works seamlessly with Unity's built-in physically based rendering giving beautiful results.

![](https://i.imgur.com/CMgwqRE.png)

!{}(https://i.imgur.com/SdqMc8p.png)

## Why not use this

This is only for rendering spheres, so any other shape of mesh won't work with this code. At the moment there is no support for casting or receiving shadows. In some environments there may not be a performance increase, as the gain in mesh complexity may be offset with an increase in shader complexity and fill.

## How to use this

To use this you must be using Unity 2018.2 beta or later and either the lightweight or HD render pipeline. Import the files into the "Assets" folder of your Unity project. Place a "Perfect Sphere" prefab into the scene and hit "Play". The rotation and rosition of the sphere can be controlled from the transform of the gameobject. The radius of the sphere can be controlled by the exposed "Radius" variable on the script. Scaling is not supported. To texture the sphere simply assign a texture to the Script before the prefab starts. You can change the number of sides the polygon has (to tighten the mesh at the expense of more triangles) by editing the "numSides" variable in the script.
