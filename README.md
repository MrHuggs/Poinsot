# Poinsot
Unity Rigid Bodiy Experiments

This is a visualizer for the Poinsot construction for rigid body motion. 

See https://en.wikipedia.org/wiki/Poinsot%27s_ellipsoid.

# Simulation

The core is a simulation of a rotating rigid body and a couple of cameras to watch the body spin.

Euler's equations are integrated in the easiest way possible...with the explicit Euler method.

# Binet Ellipsoid

One camera shows the Binet Ellipsoid.

The idea is that as angular momementum and energy are both conserved, if you look at E and L in the body frame with
appropriate scaling, the trajectory moves along the intersection of a sphere and an ellipse.

Depending on the initial anglular momentim and the inertia tensor, this motion may be stable or not.


