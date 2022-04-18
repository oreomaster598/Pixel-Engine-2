/*
  Box2DNet Copyright (c) 2018 codeyu https://github.com/codeyu/Box2DNet
  Box2D original C++ version Copyright (c) 2006-2007 Erin Catto http://www.gphysics.com
  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:
  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

//r175

using System; using System.Numerics;
using System.Collections.Generic;
using System.Text;
 

namespace PE2.Physics.Common
{
	public struct Sweep
	{
		public Vector2 LocalCenter;	//local center of mass position
		public Vector2 C0, C; //local center of mass position
		public float A0, A; //world angles
		public float T0; //time interval = [T0,1], where T0 is in [0,1]

		/// <summary>
		/// Get the interpolated Transform at a specific time.
		/// </summary>
		/// <param name="alpha">Alpha is a factor in [0,1], where 0 indicates t0.</param>
		public void GetTransform(out Transform xf, float alpha)
		{
			xf = new Transform();
			xf.position = (1.0f - alpha) * C0 + alpha * C;
			float angle = (1.0f - alpha) * A0 + alpha * A;
			
			xf.rotation = Common.Math.AngleToRotation(angle);
			//xf.R = new Mat22(angle);

			// Shift to origin
			xf.position -= xf.TransformDirection(LocalCenter);
		}

		/// <summary>
		/// Advance the sweep forward, yielding a new initial state.
		/// </summary>
		/// <param name="t">The new initial time.</param>
		public void Advance(float t)
		{
			if (T0 < t && 1.0f - T0 > Settings.FLT_EPSILON)
			{
				float alpha = (t - T0) / (1.0f - T0);
				C0 = (1.0f - alpha) * C0 + alpha * C;
				A0 = (1.0f - alpha) * A0 + alpha * A;
				T0 = t;
			}
		}
	}
}