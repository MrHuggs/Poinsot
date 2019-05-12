using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if targ_double
using targ_type = System.Double;
#else
using targ_type = System.Single;
#endif


// Based on : http://people.sc.fsu.edu/~jburkardt%20/cpp_src/rk4/rk4.cpp


namespace Assets.RigidBody
{
	public class RK4
	{
		public delegate targ_type fsingle_func(targ_type t, targ_type u);

		//****************************************************************************80

		public static targ_type RK4single(targ_type t0, targ_type u0, targ_type dt, fsingle_func f)

		//****************************************************************************80
		//
		//  Purpose:
		// 
		//    RK4 takes one Runge-Kutta step for a scalar ODE.
		//
		//  Discussion:
		//
		//    It is assumed that an initial value problem, of the form
		//
		//      du/dt = f ( t, u )
		//      u(t0) = u0
		//
		//    is being solved.
		//
		//    If the user can supply current values of t, u, a stepsize dt, and a
		//    function to evaluate the derivative, this function can compute the
		//    fourth-order Runge Kutta estimate to the solution at time t+dt.
		//
		//  Licensing:
		//
		//    This code is distributed under the GNU LGPL license. 
		//
		//  Modified:
		//
		//    09 October 2013
		//
		//  Author:
		//
		//    John Burkardt
		//
		//  Parameters:
		//
		//    Input, double T0, the current time.
		//
		//    Input, double U0, the solution estimate at the current time.
		//
		//    Input, double DT, the time step.
		//
		//    Input, double F ( double T, double U ), a function which evaluates
		//    the derivative, or right hand side of the problem.
		//
		//    Output, double RK4, the fourth-order Runge-Kutta solution estimate
		//    at time T0+DT.
		//
		{
			targ_type f0;
			targ_type f1;
			targ_type f2;
			targ_type f3;
			targ_type t1;
			targ_type t2;
			targ_type t3;
			targ_type u;
			targ_type u1;
			targ_type u2;
			targ_type u3;
			//
			//  Get four sample values of the derivative.
			//
			f0 = f(t0, u0);

			t1 = (targ_type) (t0 + dt / 2.0);
			u1 = (targ_type) (u0 + dt * f0 / 2.0);
			f1 = f(t1, u1);

			t2 = (targ_type) (t0 + dt / 2.0);
			u2 = (targ_type) (u0 + dt * f1 / 2.0);
			f2 = f(t2, u2);

			t3 = t0 + dt;
			u3 = u0 + dt * f2;
			f3 = f(t3, u3);
			//
			//  Combine to estimate the solution at time T0 + DT.
			//
			u = (targ_type) (u0 + dt * (f0 + 2.0 * f1 + 2.0 * f2 + f3) / 6.0);

			return u;
		}

		public delegate targ_type[] fvector_func(targ_type t, targ_type[] u);

		public static targ_type[] RK4vec(targ_type t0, targ_type[] u0, targ_type dt, fvector_func f)
		//****************************************************************************80
		//
		//  Purpose:
		//
		//    RK4VEC takes one Runge-Kutta step for a vector ODE.
		//
		//  Discussion:
		//
		//    It is assumed that an initial value problem, of the form
		//
		//      du/dt = f ( t, u )
		//      u(t0) = u0
		//
		//    is being solved.
		//
		//    If the user can supply current values of t, u, a stepsize dt, and a
		//    function to evaluate the derivative, this function can compute the
		//    fourth-order Runge Kutta estimate to the solution at time t+dt.
		//
		//  Licensing:
		//
		//    This code is distributed under the GNU LGPL license. 
		//
		//  Modified:
		//
		//    09 October 2013
		//
		//  Author:
		//
		//    John Burkardt
		//
		//  Parameters:
		//
		//    Input, double T0, the current time.
		//
		//    Input, int M, the spatial dimension.
		//
		//    Input, double U0[M], the solution estimate at the current time.
		//
		//    Input, double DT, the time step.
		//
		//    Input, double *F ( double T, int M, double U[] ), a function which evaluates
		//    the derivative, or right hand side of the problem.
		//
		//    Output, double RK4VEC[M], the fourth-order Runge-Kutta solution estimate
		//    at time T0+DT.
		//
		{
			int n = u0.Length;
			targ_type[] f0 = new targ_type[n];
			targ_type[] f1 = new targ_type[n];
			targ_type[] f2 = new targ_type[n];
			targ_type[] f3 = new targ_type[n];
			int i;
			targ_type t1;
			targ_type t2;
			targ_type t3;
			targ_type[] u = new targ_type[n];
			targ_type[] u1 = new targ_type[n];
			targ_type[] u2 = new targ_type[n];
			targ_type[] u3 = new targ_type[n];
			//
			//  Get four sample values of the derivative.
			//
			f0 = f(t0, u0);

			t1 = (targ_type) (t0 + dt / 2.0);
			for (i = 0; i < n; i++)
			{
				u1[i] = (targ_type) (u0[i] + dt * f0[i] / 2.0);
			}
			f1 = f(t1, u1);

			t2 = (targ_type) (t0 + dt / 2.0);
			for (i = 0; i < n; i++)
			{
				u2[i] = (targ_type) (u0[i] + dt * f1[i] / 2.0);
			}
			f2 = f(t2, u2);

			t3 = t0 + dt;
			for (i = 0; i < n; i++)
			{
				u3[i] = u0[i] + dt * f2[i];
			}
			f3 = f(t3, u3);
			//
			//  Combine them to estimate the solution.
			//
			for (i = 0; i < n; i++)
			{
				u[i] = (targ_type) (u0[i] + dt * (f0[i] + 2.0 * f1[i] + 2.0 * f2[i] + f3[i]) / 6.0);
			}

			return u;
		}
	}
}