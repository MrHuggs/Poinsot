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

Trying the same test with the energy adjustment off shows much greater error, so it's possible the adjustment is haviing an effect.
By setting the step size very small (.00001) it is possible to get good results w/o the adjustment.

A run with-single precision math is shown in "Fig 4a Single Precision.png." Initially, the motion is the same except that the period 
is around 11 seconds, but around 25 seconds there is a qualitative difference in w_x.

Even with double precisin math, the energy and angular momentume group over time. The "Preserve L and K_e" options applies corrections
to preserve these quantities, but the motin is changed - the period is more like 9.5 seconds. 
You can see this in "Fig 4a compare with Adjustment.png"

