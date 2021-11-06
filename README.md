# Deformation

The deformation project is a series of scripts that can create a deformations in [Unity](https://unity3d.com/). 

## [Static](/static)

The script creates an object in the scene and randomly creates a crack and creates two new objects. 
The objects do not split apart, but that logic could be added at a later date. The new objects
can interact with others in the scene, but there are many limitations with this method. 

### Files

- [slicing_deformation.cs](/static/slicing_deformation.cs)

### How to use in [Unity](https://unity3d.com/)


## [Dynamic](/dynamic)

The script can be attached to a Cube object. It should be able to collide with another object in the scene.
The point of collision will be taken (average point when more than one collision point occurs) and a crack
will form from this point until the outside of the Cube has been reached. There will be two new objects in 
the scene after the crack occurs. An explosion event could be added to the scene if the user would like to
see the two new objects split after collision.

### Files

- [Breakable.cs](/dynamic/Breakable.cs) 

### How to use in [Unity](https://unity3d.com/)