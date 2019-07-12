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


# Rolling Effect

Some good values to see the rolling of the ellipsoid of inertia on the invariable plane are:

I_x = .6	I_y = 2, I_z = 2.5
w_x = 1.5	w_y = 2, w_z = .1
