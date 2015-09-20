
# Dynamic_Static Gravity Demo

================================================================================================================================

Compute shader based demo of pseudo-Einsteinian gravitational curvature.

[![Video Link](https://github.com/DynamicStatic/Gravity-Demo/blob/master/Assets/Resources/GravityScreen.PNG)]
(https://www.youtube.com/watch?v=Hz42sDJE2e4 "VIDEO LINK")

This demo is similar to other n-body gravity demos, except that none of the bodies communicate their gravitational force to each other, rather each body communicates its mass (they are point masses) and position to the gravitational field, the field then communicates its curvature to each body.

Once the initial speed, mass, and position for each body is set and the simulation begins, all of the data is uploaded to the GPU and everything is processed through compute shaders; the CPU's only job while running is to dispatch the compute shaders each frame.  The gravitational field is rendered with a couple of geometry shaders and the bodies are rendered with an extremely simple vertex / pixel shader that grabs their indexed positions from a structured buffer and uses a bare bones lighting model.

Caveats :
While the concept is Einsteinian, the math is all Newtonian (because Einstein's equations are hard), and approximated at that.
The bodies query the gravitational field using trilinear interpolation, which is obviously not physically correct.
There is no collision detection, so bodies happily pass through each other.
And many, MANY other caveats...

...like...really...MANY

You'll need [Unity3D](http://unity3d.com/) (5 or greater) and a DirectX11 capable GPU on a Windows system.
