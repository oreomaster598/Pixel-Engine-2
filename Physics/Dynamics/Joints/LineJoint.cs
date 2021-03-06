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

// Linear constraint (point-to-line)
// d = p2 - p1 = x2 + r2 - x1 - r1
// C = dot(perp, d)
// Cdot = dot(d, cross(w1, perp)) + dot(perp, v2 + cross(w2, r2) - v1 - cross(w1, r1))
//      = -dot(perp, v1) - dot(cross(d + r1, perp), w1) + dot(perp, v2) + dot(cross(r2, perp), v2)
// J = [-perp, -cross(d + r1, perp), perp, cross(r2,perp)]
//
// K = J * invM * JT
//
// J = [-a -s1 a s2]
// a = perp
// s1 = cross(d + r1, a) = cross(p2 - x1, a)
// s2 = cross(r2, a) = cross(p2 - x2, a)


// Motor/Limit linear constraint
// C = dot(ax1, d)
// Cdot = = -dot(ax1, v1) - dot(cross(d + r1, ax1), w1) + dot(ax1, v2) + dot(cross(r2, ax1), v2)
// J = [-ax1 -cross(d+r1,ax1) ax1 cross(r2,ax1)]

// Block Solver
// We develop a block solver that includes the joint limit. This makes the limit stiff (inelastic) even
// when the mass has poor distribution (leading to large torques about the joint anchor points).
//
// The Jacobian has 3 rows:
// J = [-uT -s1 uT s2] // linear
//     [-vT -a1 vT a2] // limit
//
// u = perp
// v = axis
// s1 = cross(d + r1, u), s2 = cross(r2, u)
// a1 = cross(d + r1, v), a2 = cross(r2, v)

// M * (v2 - v1) = JT * df
// J * v2 = bias
//
// v2 = v1 + invM * JT * df
// J * (v1 + invM * JT * df) = bias
// K * df = bias - J * v1 = -Cdot
// K = J * invM * JT
// Cdot = J * v1 - bias
//
// Now solve for f2.
// df = f2 - f1
// K * (f2 - f1) = -Cdot
// f2 = invK * (-Cdot) + f1
//
// Clamp accumulated limit impulse.
// lower: f2(2) = max(f2(2), 0)
// upper: f2(2) = min(f2(2), 0)
//
// Solve for correct f2(1)
// K(1,1) * f2(1) = -Cdot(1) - K(1,2) * f2(2) + K(1,1:2) * f1
//                = -Cdot(1) - K(1,2) * f2(2) + K(1,1) * f1(1) + K(1,2) * f1(2)
// K(1,1) * f2(1) = -Cdot(1) - K(1,2) * (f2(2) - f1(2)) + K(1,1) * f1(1)
// f2(1) = invK(1,1) * (-Cdot(1) - K(1,2) * (f2(2) - f1(2))) + f1(1)
//
// Now compute impulse to be applied:
// df = f2 - f1

using System; using System.Numerics;
using System.Collections.Generic;
using System.Text;

using PE2.Physics.Common;
 

using Transform = PE2.Physics.Common.Transform;

namespace PE2.Physics.Dynamics
{
	/// <summary>
	/// Line joint definition. This requires defining a line of
	/// motion using an axis and an anchor point. The definition uses local
	/// anchor points and a local axis so that the initial configuration
	/// can violate the constraint slightly. The joint translation is zero
	/// when the local anchor points coincide in world space. Using local
	/// anchors and a local axis helps when saving and loading a game.
	/// </summary>
	public class LineJointDef : JointDef
	{
		public LineJointDef()
		{
			Type = JointType.LineJoint;
			localAnchor1 = Vector2.Zero;
			localAnchor2 = Vector2.Zero;
			localAxis1 = new Vector2(1.0f, 0.0f);
			enableLimit = false;
			lowerTranslation = 0.0f;
			upperTranslation = 0.0f;
			enableMotor = false;
			maxMotorForce = 0.0f;
			motorSpeed = 0.0f;
		}

		/// <summary>
		/// Initialize the bodies, anchors, axis, and reference angle using the world
		/// anchor and world axis.
		/// </summary>
		public void Initialize(Body body1, Body body2, Vector2 anchor, Vector2 axis)
		{
			Body1 = body1;
			Body2 = body2;
			localAnchor1 = body1.GetLocalPoint(anchor);
			localAnchor2 = body2.GetLocalPoint(anchor);
			localAxis1 = body1.GetLocalVector(axis);
		}

		/// <summary>
		/// The local anchor point relative to body1's origin.
		/// </summary>
		public Vector2 localAnchor1;

		/// <summary>
		/// The local anchor point relative to body2's origin.
		/// </summary>
		public Vector2 localAnchor2;

		/// <summary>
		/// The local translation axis in body1.
		/// </summary>
		public Vector2 localAxis1;

		/// <summary>
		/// Enable/disable the joint limit.
		/// </summary>
		public bool enableLimit;

		/// <summary>
		/// The lower translation limit, usually in meters.
		/// </summary>
		public float lowerTranslation;

		/// <summary>
		/// The upper translation limit, usually in meters.
		/// </summary>
		public float upperTranslation;

		/// <summary>
		/// Enable/disable the joint motor.
		/// </summary>
		public bool enableMotor;

		/// <summary>
		/// The maximum motor torque, usually in N-m.
		/// </summary>
		public float maxMotorForce;

		/// <summary>
		/// The desired motor speed in radians per second.
		/// </summary>
		public float motorSpeed;
	}

	/// <summary>
	/// A line joint. This joint provides one degree of freedom: translation
	/// along an axis fixed in body1. You can use a joint limit to restrict
	/// the range of motion and a joint motor to drive the motion or to
	/// model joint friction.
	/// </summary>
	public class LineJoint : Joint
	{
		public Vector2 _localAnchor1;
		public Vector2 _localAnchor2;
		public Vector2 _localXAxis1;
		public Vector2 _localYAxis1;

		public Vector2 _axis, _perp;
		public float _s1, _s2;
		public float _a1, _a2;

		public Mat22 _K;
		public Vector2 _impulse;

		public float _motorMass;			// effective mass for motor/limit translational constraint.
		public float _motorImpulse;

		public float _lowerTranslation;
		public float _upperTranslation;
		public float _maxMotorForce;
		public float _motorSpeed;

		public bool _enableLimit;
		public bool _enableMotor;
		public LimitState _limitState;

		public LineJoint(LineJointDef def)
			: base(def)
		{
			_localAnchor1 = def.localAnchor1;
			_localAnchor2 = def.localAnchor2;
			_localXAxis1 = def.localAxis1;
			_localYAxis1 = _localXAxis1.CrossScalarPreMultiply(1.0f);
			
			_impulse = Vector2.Zero;
			_motorMass = 0.0f;
			_motorImpulse = 0.0f;

			_lowerTranslation = def.lowerTranslation;
			_upperTranslation = def.upperTranslation;
			_maxMotorForce = Settings.FORCE_INV_SCALE(def.maxMotorForce);
			_motorSpeed = def.motorSpeed;
			_enableLimit = def.enableLimit;
			_enableMotor = def.enableMotor;
			_limitState = LimitState.InactiveLimit;

			_axis = Vector2.Zero;
			_perp = Vector2.Zero;
		}

		public override Vector2 Anchor1
		{
			get { return _body1.GetWorldPoint(_localAnchor1); }
		}

		public override Vector2 Anchor2
		{
			get { return _body2.GetWorldPoint(_localAnchor2); }
		}

		public override Vector2 GetReactionForce(float inv_dt)
		{
			return inv_dt * (_impulse.X * _perp + (_motorImpulse + _impulse.Y) * _axis);
		}

		public override float GetReactionTorque(float inv_dt)
		{
			return 0.0f;
		}

		/// <summary>
		/// Get the current joint translation, usually in meters.
		/// </summary>
		public float GetJointTranslation()
		{
			Body b1 = _body1;
			Body b2 = _body2;

			Vector2 p1 = b1.GetWorldPoint(_localAnchor1);
			Vector2 p2 = b2.GetWorldPoint(_localAnchor2);
			Vector2 d = p2 - p1;
			Vector2 axis = b1.GetWorldVector(_localXAxis1);

			float translation = Vector2.Dot(d, axis);
			return translation;
		}

		/// <summary>
		/// Get the current joint translation speed, usually in meters per second.
		/// </summary>
		public float GetJointSpeed()
		{
			Body b1 = _body1;
			Body b2 = _body2;

			Vector2 r1 = b1.GetTransform().TransformDirection(_localAnchor1 - b1.GetLocalCenter());
			Vector2 r2 = b2.GetTransform().TransformDirection(_localAnchor2 - b2.GetLocalCenter());
			Vector2 p1 = b1._sweep.C + r1;
			Vector2 p2 = b2._sweep.C + r2;
			Vector2 d = p2 - p1;
			Vector2 axis = b1.GetWorldVector(_localXAxis1);

			Vector2 v1 = b1._linearVelocity;
			Vector2 v2 = b2._linearVelocity;
			float w1 = b1._angularVelocity;
			float w2 = b2._angularVelocity;

			float speed = Vector2.Dot(d, axis.CrossScalarPreMultiply(w1)) + Vector2.Dot(axis, v2 + r2.CrossScalarPreMultiply(w2) - v1 - r1.CrossScalarPreMultiply(w1));
			return speed;
		}

		/// <summary>
		/// Is the joint limit enabled?
		/// </summary>
		public bool IsLimitEnabled()
		{
			return _enableLimit;
		}

		/// <summary>
		/// Enable/disable the joint limit.
		/// </summary>
		public void EnableLimit(bool flag)
		{
			_body1.WakeUp();
			_body2.WakeUp();
			_enableLimit = flag;
		}

		/// <summary>
		/// Get the lower joint limit, usually in meters.
		/// </summary>
		public float GetLowerLimit()
		{
			return _lowerTranslation;
		}

		/// <summary>
		/// Get the upper joint limit, usually in meters.
		/// </summary>
		public float GetUpperLimit()
		{
			return _upperTranslation;
		}

		/// <summary>
		/// Set the joint limits, usually in meters.
		/// </summary>
		public void SetLimits(float lower, float upper)
		{
			Box2DNetDebug.Assert(lower <= upper);
			_body1.WakeUp();
			_body2.WakeUp();
			_lowerTranslation = lower;
			_upperTranslation = upper;
		}

		/// <summary>
		/// Is the joint motor enabled?
		/// </summary>
		public bool IsMotorEnabled()
		{
			return _enableMotor;
		}

		/// <summary>
		/// Enable/disable the joint motor.
		/// </summary>
		public void EnableMotor(bool flag)
		{
			_body1.WakeUp();
			_body2.WakeUp();
			_enableMotor = flag;
		}

		/// <summary>
		/// Set the motor speed, usually in meters per second.
		/// </summary>
		public void SetMotorSpeed(float speed)
		{
			_body1.WakeUp();
			_body2.WakeUp();
			_motorSpeed = speed;
		}

		/// <summary>
		/// Set the maximum motor force, usually in N.
		/// </summary>
		public void SetMaxMotorForce(float force)
		{
			_body1.WakeUp();
			_body2.WakeUp();
			_maxMotorForce = Settings.FORCE_SCALE(1.0f) * force;
		}

		/// <summary>
		/// Get the current motor force, usually in N.
		/// </summary>
		public float GetMotorForce()
		{
			return _motorImpulse;
		}

		/// <summary>
		/// Get the motor speed, usually in meters per second.
		/// </summary>
		public float GetMotorSpeed()
		{
			return _motorSpeed;
		}

		internal override void InitVelocityConstraints(TimeStep step)
		{
			Body b1 = _body1;
			Body b2 = _body2;

			_localCenter1 = b1.GetLocalCenter();
			_localCenter2 = b2.GetLocalCenter();

			Transform xf1 = b1.GetTransform();
			Transform xf2 = b2.GetTransform();

			// Compute the effective masses.
			Vector2 r1 = xf1.TransformDirection(_localAnchor1 - _localCenter1);
			Vector2 r2 = xf2.TransformDirection(_localAnchor2 - _localCenter2);
			Vector2 d = b2._sweep.C + r2 - b1._sweep.C - r1;

			_invMass1 = b1._invMass;
			_invI1 = b1._invI;
			_invMass2 = b2._invMass;
			_invI2 = b2._invI;

			// Compute motor Jacobian and effective mass.
			{
				_axis = xf1.TransformDirection(_localXAxis1);
				_a1 = (d + r1).Cross(_axis);
				_a2 = r2.Cross(_axis);

				_motorMass = _invMass1 + _invMass2 + _invI1 * _a1 * _a1 + _invI2 * _a2 * _a2;
				Box2DNetDebug.Assert(_motorMass > Settings.FLT_EPSILON);
				_motorMass = 1.0f / _motorMass;
			}

			// Prismatic constraint.
			{
				_perp = xf1.TransformDirection(_localYAxis1);

				_s1 = (d + r1).Cross(_perp);
				_s2 = r2.Cross(_perp);

				float m1 = _invMass1, m2 = _invMass2;
				float i1 = _invI1, i2 = _invI2;

				float k11 = m1 + m2 + i1 * _s1 * _s1 + i2 * _s2 * _s2;
				float k12 = i1 * _s1 * _a1 + i2 * _s2 * _a2;
				float k22 = m1 + m2 + i1 * _a1 * _a1 + i2 * _a2 * _a2;

				_K.Col1 = new Vector2(k11, k12);
				_K.Col2 = new Vector2(k12, k22);
			}

			// Compute motor and limit terms.
			if (_enableLimit)
			{
				float jointTranslation = Vector2.Dot(_axis, d);
				if (Common.Math.Abs(_upperTranslation - _lowerTranslation) < 2.0f * Settings.LinearSlop)
				{
					_limitState = LimitState.EqualLimits;
				}
				else if (jointTranslation <= _lowerTranslation)
				{
					if (_limitState != LimitState.AtLowerLimit)
					{
						_limitState = LimitState.AtLowerLimit;
						_impulse.Y = 0.0f;
					}
				}
				else if (jointTranslation >= _upperTranslation)
				{
					if (_limitState != LimitState.AtUpperLimit)
					{
						_limitState = LimitState.AtUpperLimit;
						_impulse.Y = 0.0f;
					}
				}
				else
				{
					_limitState = LimitState.InactiveLimit;
					_impulse.Y = 0.0f;
				}
			}
			else
			{
				_limitState = LimitState.InactiveLimit;
			}

			if (_enableMotor == false)
			{
				_motorImpulse = 0.0f;
			}

			if (step.WarmStarting)
			{
				// Account for variable time step.
				_impulse *= step.DtRatio;
				_motorImpulse *= step.DtRatio;

				Vector2 P = _impulse.X * _perp + (_motorImpulse + _impulse.Y) * _axis;
				float L1 = _impulse.X * _s1 + (_motorImpulse + _impulse.Y) * _a1;
				float L2 = _impulse.X * _s2 + (_motorImpulse + _impulse.Y) * _a2;

				b1._linearVelocity -= _invMass1 * P;
				b1._angularVelocity -= _invI1 * L1;

				b2._linearVelocity += _invMass2 * P;
				b2._angularVelocity += _invI2 * L2;
			}
			else
			{
				_impulse = Vector2.Zero;
				_motorImpulse = 0.0f;
			}
		}
		
		internal override void SolveVelocityConstraints(TimeStep step)
		{
			Body b1 = _body1;
			Body b2 = _body2;

			Vector2 v1 = b1._linearVelocity;
			float w1 = b1._angularVelocity;
			Vector2 v2 = b2._linearVelocity;
			float w2 = b2._angularVelocity;

			// Solve linear motor constraint.
			if (_enableMotor && _limitState != LimitState.EqualLimits)
			{
				float Cdot = Vector2.Dot(_axis, v2 - v1) + _a2 * w2 - _a1 * w1;
				float impulse = _motorMass * (_motorSpeed - Cdot);
				float oldImpulse = _motorImpulse;
				float maxImpulse = step.Dt * _maxMotorForce;
				_motorImpulse = Common.Math.Clamp(_motorImpulse + impulse, -maxImpulse, maxImpulse);
				impulse = _motorImpulse - oldImpulse;

				Vector2 P = impulse * _axis;
				float L1 = impulse * _a1;
				float L2 = impulse * _a2;

				v1 -= _invMass1 * P;
				w1 -= _invI1 * L1;

				v2 += _invMass2 * P;
				w2 += _invI2 * L2;
			}

			float Cdot1 = Vector2.Dot(_perp, v2 - v1) + _s2 * w2 - _s1 * w1;

			if (_enableLimit && _limitState != LimitState.InactiveLimit)
			{
				// Solve prismatic and limit constraint in block form.
				float Cdot2 = Vector2.Dot(_axis, v2 - v1) + _a2 * w2 - _a1 * w1;
				Vector2 Cdot = new Vector2(Cdot1, Cdot2);

				Vector2 f1 = _impulse;
				Vector2 df =  _K.Solve(-Cdot);
				_impulse += df;

				if (_limitState == LimitState.AtLowerLimit)
				{
					_impulse.Y = System.Math.Max(_impulse.Y, 0.0f);
				}
				else if (_limitState == LimitState.AtUpperLimit)
				{
					_impulse.Y = System.Math.Min(_impulse.Y, 0.0f);
				}

				// f2(1) = invK(1,1) * (-Cdot(1) - K(1,2) * (f2(2) - f1(2))) + f1(1)
				float b = -Cdot1 - (_impulse.Y - f1.Y) * _K.Col2.Y;
				float f2r = b / _K.Col1.X + f1.X;
				_impulse.X = f2r;

				df = _impulse - f1;

				Vector2 P = df.X * _perp + df.Y * _axis;
				float L1 = df.X * _s1 + df.Y * _a1;
				float L2 = df.X * _s2 + df.Y * _a2;

				v1 -= _invMass1 * P;
				w1 -= _invI1 * L1;

				v2 += _invMass2 * P;
				w2 += _invI2 * L2;
			}
			else
			{
				// Limit is inactive, just solve the prismatic constraint in block form.
				float df = (-Cdot1) / _K.Col1.X;
				_impulse.X += df;

				Vector2 P = df * _perp;
				float L1 = df * _s1;
				float L2 = df * _s2;

				v1 -= _invMass1 * P;
				w1 -= _invI1 * L1;

				v2 += _invMass2 * P;
				w2 += _invI2 * L2;
			}

			b1._linearVelocity = v1;
			b1._angularVelocity = w1;
			b2._linearVelocity = v2;
			b2._angularVelocity = w2;
		}

		internal override bool SolvePositionConstraints(float baumgarte)
		{
			Body b1 = _body1;
			Body b2 = _body2;

			Vector2 c1 = b1._sweep.C;
			float a1 = b1._sweep.A;

			Vector2 c2 = b2._sweep.C;
			float a2 = b2._sweep.A;

			// Solve linear limit constraint.
			float linearError = 0.0f, angularError = 0.0f;
			bool active = false;
			float C2 = 0.0f;

			Mat22 R1 = new Mat22(a1), R2 = new Mat22(a2);

			Vector2 r1 = R1.Multiply(_localAnchor1 - _localCenter1);
			Vector2 r2 = R2.Multiply(_localAnchor2 - _localCenter2);
			Vector2 d = c2 + r2 - c1 - r1;

			if (_enableLimit)
			{
				_axis = R1.Multiply(_localXAxis1);

				_a1 = (d + r1).Cross(_axis);
				_a2 = r2.Cross(_axis);

				float translation = Vector2.Dot(_axis, d);
				if (System.Math.Abs(_upperTranslation - _lowerTranslation) < 2.0f * Settings.LinearSlop)
				{
					// Prevent large angular corrections
					C2 = Common.Math.Clamp(translation, -Settings.MaxLinearCorrection, Settings.MaxLinearCorrection);
					linearError = Common.Math.Abs(translation);
					active = true;
				}
				else if (translation <= _lowerTranslation)
				{
					// Prevent large linear corrections and allow some slop.
					C2 = Common.Math.Clamp(translation - _lowerTranslation + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0.0f);
					linearError = _lowerTranslation - translation;
					active = true;
				}
				else if (translation >= _upperTranslation)
				{
					// Prevent large linear corrections and allow some slop.
					C2 = Common.Math.Clamp(translation - _upperTranslation - Settings.LinearSlop, 0.0f, Settings.MaxLinearCorrection);
					linearError = translation - _upperTranslation;
					active = true;
				}
			}

			_perp = R1.Multiply(_localYAxis1);

			_s1 = (d + r1).Cross(_perp);
			_s2 = r2.Cross(_perp);

			Vector2 impulse;
			float C1;
			C1 = Vector2.Dot(_perp, d);

			linearError = Common.Math.Max(linearError, Common.Math.Abs(C1));
			angularError = 0.0f;

			if (active)
			{
				float m1 = _invMass1, m2 = _invMass2;
				float i1 = _invI1, i2 = _invI2;

				float k11 = m1 + m2 + i1 * _s1 * _s1 + i2 * _s2 * _s2;
				float k12 = i1 * _s1 * _a1 + i2 * _s2 * _a2;
				float k22 = m1 + m2 + i1 * _a1 * _a1 + i2 * _a2 * _a2;

				_K.Col1 = new Vector2(k11, k12);
				_K.Col2 = new Vector2(k12, k22);

				Vector2 C = new Vector2();
				C.X = C1;
				C.Y = C2;

				impulse = _K.Solve(-C);
			}
			else
			{
				float m1 = _invMass1, m2 = _invMass2;
				float i1 = _invI1, i2 = _invI2;

				float k11 = m1 + m2 + i1 * _s1 * _s1 + i2 * _s2 * _s2;

				float impulse1 = (-C1) / k11;
				impulse.X = impulse1;
				impulse.Y = 0.0f;
			}

			Vector2 P = impulse.X * _perp + impulse.Y * _axis;
			float L1 = impulse.X * _s1 + impulse.Y * _a1;
			float L2 = impulse.X * _s2 + impulse.Y * _a2;

			c1 -= _invMass1 * P;
			a1 -= _invI1 * L1;
			c2 += _invMass2 * P;
			a2 += _invI2 * L2;

			// TODO_ERIN remove need for this.
			b1._sweep.C = c1;
			b1._sweep.A = a1;
			b2._sweep.C = c2;
			b2._sweep.A = a2;
			b1.SynchronizeTransform();
			b2.SynchronizeTransform();

			return linearError <= Settings.LinearSlop && angularError <= Settings.AngularSlop;
		}
	}
}