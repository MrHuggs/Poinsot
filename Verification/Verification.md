The precession of symmetric free rigid body can be easily calculated using the formula:


 T = 2 pi I_1
    ---------
	w|I_2-I_1|

The plot "Free Precession.png" shows I_1 = I_3 = 1, I_2 = 1.1 and w = 10, the period
is about 6.2 sec.
The formula gives 2 pi ~ 6.28 seconds, which matches.

The plot "Free Precession.png" has the same initial conditions, but has the energy normalization turned off.

The paper Dzhanibekov Effect.pdf contains a simulation of an object with:

	Ixx = 0.3
	Iyy = 0.35,
	Izz = 0.4 (all in kg*m2)

and the initial conditions 
	wx = 0.1,
	wy = 15
	wz = 0.1 (all in rad/s).

Figure 4.a shows the results. The flipping period seems to be about 12 seconds.

Compare to the plot from "Fig 4a compare.png." This shows a flipping period of also around 12 seconds.

A run with-single precision math is shown in "Fig 4a Single Precision.png." Initially, the motion is the same except that the period 
is around 11 seconds, but around 25 seconds there is a qualitative difference in w_x.

Even with double precision math, the energy and angular momentum group over time. The "Preserve L and K_e" options applies corrections
to preserve these quantities, but the motion is changed - the period is more like 9.5 seconds. You can see this in "Fig 4a compare with Adjustment.png."

Explict Euler with a .001 step sizes does reasonably well, but the 4th order Runge-Kutta preserves L and K_e much better:

Initial:
	t =0 L=(0.0, 5.2, 0.0) L^2=27.5649990614206 |L|=5.25023800045489 E=39.3784993296415

Runge-Kutta:
	t =51 L=(0.0, 5.3, 0.0) L^2=27.5649990614198 |L|=5.25023800045482 E=39.3784993296404
Explicit Euler:
	t =51 L=(0.0, 5.3, 0.0) L^2=28.5453250007816 |L|=5.34278251483079 E=40.7789784231468
