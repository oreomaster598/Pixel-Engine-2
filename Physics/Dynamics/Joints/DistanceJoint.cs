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

// 1-D constrained system
// m (v2 - v1) = lambda
// v2 + (beta/h) * x1 + gamma * lambda = 0, gamma has units of inverse mass.
// x2 = x1 + h * v2

// 1-D mass-damper-spring system
// m (v2 - v1) + h * d * v2 + h * k * 

// C = norm(p2 - p1) - L
// u = (p2 - p1) / norm(p2 - p1)
// Cdot = dot(u, v2 + cross(w2, r2) - v1 - cross(w1, r1))
// J = [-u -cross(r1, u) u cross(r2, u)]
// K = J * invM * JT
//   = invMass1 + invI1 * cross(r1, u)^2 + invMass2 + invI2 * cross(r2, u)^2

using System; using System.Numerics;
using System.Collections.Generic;
using System.Text;

using PE2.Physics.Common;
 

namespace PE2.Physics.Dynamics
{
	/// <summary>
	/// Distance joint definition. This requires defining an
	/// anchor point on both bodies and the non-zero length of the
	/// distance joint. The definition uses local anchor points
	/// so that the initial configuration can violate the constraint
	/// slightly. This helps when saving and loading a game.
	/// @warning Do not use a zero or short length.
	/// </summary>
	public class DistanceJointDef : JointDef
	{
		public DistanceJointDef()
		{
			Type = JointType.DistanceJoint;
			LocalAnchor1 = Vector2.Zero;
			LocalAnchor2 = Vector2.Zero;
			Length = 1.0f;
			FrequencyHz = 0.0f;
			DampingRatio = 0.0f;
		}

		/// <summary>
		/// Initialize the bodies, anchors, and length using the world anchors.
		/// </summary>
		public void Initialize(Body body1, Body body2, Vector2 anchor1, Vector2 anchor2)
		{
			Body1 = body1;
			Body2 = body2;
			LocalAnchor1 = body1.GetLocalPoint(anchor1);
			LocalAnchor2 = body2.GetLocalPoint(anchor2);
			var d = anchor2 - anchor1;
			Length = d.Length();
		}

		/// <summary>
		/// The local anchor point relative to body1's origin.
		/// </summary>
		public Vector2 LocalAnchor1;

		/// <summary>
		/// The local anchor point relative to body2's origin.
		/// </summary>
		public Vector2 LocalAnchor2;

		/// <summary>
		/// The equilibrium length between the anchor points.
		/// </summary>
		public float Length;

		/// <summary>
		/// The response speed.
		/// </summary>
		public float FrequencyHz;

		/// <summary>
		/// The damping ratio. 0 = no damping, 1 = critical damping.
		/// </summary>
		public float DampingRatio;
	}

	/// <summary>
	/// A distance joint constrains two points on two bodies
	/// to remain at a fixed distance from each other. You can view
	/// this as a massless, rigid rod.
	/// </summary>
	public class DistanceJoint : Joint
	{
		public Vector2 _localAnchor1;
		public Vector2 _localAnchor2;
		public Vector2 _u;
		public float _frequencyHz;
		public float _dampingRatio;
		public float _gamma;
		public float _bias;
		public float _impulse;
		public float _mass;		// effective mass for the constraint.
		public float _length;

		public override Vector2 Anchor1
		{
			get { return _body1.GetWorldPoint(_localAnchor1);}
		}

		public override Vector2 Anchor2
		{
			get { return _body2.GetWorldPoint(_localAnchor2);}
		}

		public override Vector2 GetReactionForce(float inv_dt)
		{
			return (inv_dt * _impulse) * _u;
		}

		public override float GetReactionTorque(float inv_dt)
		{
			return 0.0f;
		}

		public DistanceJoint(DistanceJointDef def)
			: base(def)
		{
			_localAnchor1 = def.LocalAnchor1;
			_localAnchor2 = def.LocalAnchor2;
			_length = def.Length;
			_frequencyHz = def.FrequencyHz;
			_dampingRatio = def.DampingRatio;
			_impulse = 0.0f;
			_gamma = 0.0f;
			_bias = 0.0f;
		}

		internal override void InitVelocityConstraints(TimeStep step)
		{
			Body b1 = _body1;
			Body b2 = _body2;

			// Compute the effective mass matrix.
			Vector2 r1 = b1.GetTransform().TransformDirection(_localAnchor1 - b1.GetLocalCenter());
			Vector2 r2 = b2.GetTransform().TransformDirection(_localAnchor2 - b2.GetLocalCenter());
			_u = b2._sweep.C + r2 - b1._sweep.C - r1;

			// Handle singularity.
			float length = _u.Length();
			if (length > Settings.LinearSlop)
			{
				_u *= 1.0f / length;
			}
			else
			{
				_u = Vector2.Zero;
			}

			float cr1u = r1.Cross(_u);
			float cr2u = r2.Cross(_u);
			float invMass = b1._invMass + b1._invI * cr1u * cr1u + b2._invMass + b2._invI * cr2u * cr2u;
			Box2DNetDebug.Assert(invMass > Settings.FLT_EPSILON);
			_mass = 1.0f / invMass;

			if (_frequencyHz > 0.0f)
			{
				float C = length - _length;

				// Frequency
				float omega = 2.0f * Settings.Pi * _frequencyHz;

				// Damping coefficient
				float d = 2.0f * _mass * _dampingRatio * omega;

				// Spring stiffness
				float k = _mass * omega * omega;

				// magic formulas
				_gamma = 1.0f / (step.Dt * (d + step.Dt * k));
				_bias = C * step.Dt * k * _gamma;

				_mass = 1.0f / (invMass + _gamma);
			}

			if (step.WarmStarting)
			{
				//Scale the inpulse to support a variable timestep.
				_impulse *= step.DtRatio;
				Vector2 P = _impulse * _u;
				b1._linearVelocity -= b1._invMass * P;
				b1._angularVelocity -= b1._invI * r1.Cross(P);
				b2._linearVelocity += b2._invMass * P;
				b2._angularVelocity += b2._invI * r2.Cross(P);
			}
			else
			{
				_impulse = 0.0f;
			}
		}

		internal override bool SolvePositionConstraints(float baumgarte)
		{
			if (_frequencyHz > 0.0f)
			{
				//There is no possition correction for soft distace constraint.
				return true;
			}

			Body b1 = _body1;
			Body b2 = _body2;

			Vector2 r1 = b1.GetTransform().TransformDirection(_localAnchor1 - b1.GetLocalCenter());
			Vector2 r2 = b2.GetTransform().TransformDirection(_localAnchor2 - b2.GetLocalCenter());

			Vector2 d = b2._sweep.C + r2 - b1._sweep.C - r1;

			var length = d.Length();
			d.Normalize();
			var C = length - _length;
			C = Common.Math.Clamp(C, -Settings.MaxLinearCorrection, Settings.MaxLinearCorrection);

			var impulse = -_mass * C;
			_u = d;
			var P = impulse * _u;

			b1._sweep.C -= b1._invMass * P;
			b1._sweep.A -= b1._invI * r1.Cross(P);
			b2._sweep.C += b2._invMass * P;
			b2._sweep.A += b2._invI * r2.Cross(P);

			b1.SynchronizeTransform();
			b2.SynchronizeTransform();

			return System.Math.Abs(C) < Settings.LinearSlop;
		}

		internal override void SolveVelocityConstraints(TimeStep step)
		{
			//B2_NOT_USED(step);

			var b1 = _body1;
			var b2 = _body2;

			var r1 = b1.GetTransform().TransformDirection( _localAnchor1 - b1.GetLocalCenter());
			var r2 = b2.GetTransform().TransformDirection(_localAnchor2 - b2.GetLocalCenter());

			// Cdot = dot(u, v + cross(w, r))
			var v1 = b1._linearVelocity + r1.CrossScalarPreMultiply(b1._angularVelocity);
			var v2 = b2._linearVelocity + r2.CrossScalarPreMultiply(b2._angularVelocity);
			var cdot = Vector2.Dot(_u, v2 - v1);
			var impulse = -_mass * (cdot + _bias + _gamma * _impulse);
			_impulse += impulse;

			var p = impulse * _u;
			b1._linearVelocity -= b1._invMass * p;
			b1._angularVelocity -= b1._invI * r1.Cross(p);
			b2._linearVelocity += b2._invMass * p;
			b2._angularVelocity += b2._invI * r2.Cross(p);
		}
	}
}
