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

using System; using System.Numerics;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace PE2.Physics
{
	public static class Box2DNetDebug
	{
		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				condition = condition;
			}
		//	Debug.Assert(condition);
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				condition = condition;
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, string detailMessage)
		{
			if (!condition)
			{
				condition = condition;
			}
		}

		public static void ThrowBox2DNetException(String message)
		{
			string msg = $"Error: {message}";
			throw new Exception(msg);
		}
	}
}