# Mathfs
Freya's expanded math functionality for Unity, brought to Godot!
- This is primarily a way to use and share Freya's awesome math stuff to Godot users
- I will also recklessly edit and adapt things without too much thought into backwards compatibility
- Minimum Godot version is currently 4.3. It may be possible to downgrade, but below 4 is completely untested

## Installation instructions

There are several ways to install this library into your project:

- **Plain install**
   - Clone or [download](https://github.com/FreyaHolmer/Mathfs/archive/refs/heads/master.zip) this repository and put it somewhere in the addons folder of your project

After installation you will be able to access the library in scripts by including the namespace `using Freya`

## Features
 - 2D Intersection tests between all combinations of:
   - Ray
   - LineSegment
   - Line
   - Circle
 - Curves & Splines
   - Bézier (Quadratic, Cubic & Generalized)
   - Hermite
   - Catmull-Rom
   - B-Spline (Uniform Cubic & Generalized Non-Uniform)
   - NURBS (Non-Unifrom Rational B-Spline)
   - Trajectory (Cubic & Generalized)
 - Trajectory math
   - GetDisplacement (point in trajectory), given gravity, angle, speed & time
   - GetLaunchSpeed, given gravity, angle & lateral distance
   - GetLaunchAngles, given gravity, speed & lateral distance
   - GetMaxRange, given gravity & speed
   - GetHeightPotential, given gravity, current height and speed
   - GetSpeedFromHeightPotential, given gravity, current height and height potential
 - Triangle math
   - Area / SignedArea, given three points or base and height
   - Contains check, given three triangle vertices and a point to test by
   - Right-angle trig functions to calculate Opposite/Adjacent/Hypotenuse/Angle
   - Incenter / Centroid
   - Incircle / Circumcircle
   - SmallestAngle
 - Polygon math
   - Area / SignedArea
   - IsClockwise
   - WindingNumber
   - Contains
 - Circle math
   - FromTwoPoints (get smallest circle passing through both points)
   - FromThreePoints (get unique circle passing through three points)
   - RadiusToArea / AreaToRadius
   - AreaToCircumference / CircumferenceToArea
   - RadiusToCircumference / CircumferenceToRadius
 - 2D Angle helpers (AngToDir, DirToAng...)
 - 2D Vector extension methods (Rotate90CCW/CW, Rotate, RotateAround...)
 - Quadratic & Linear Root finders
 - Remap functions
 - Constants (Tau, Pi, Golden Ratio, e, sqrt2)
 - Vector extension methods (WithMagnitude, ClampMagnitude(min,max)...)
 - Expanded basic math operations to vectors (Clamp, Round, Abs...)
 - Color extensions (WithAlpha, MultiplyRGB...)
 - Smoothing functions (Smooth01, SmoothCos01...)
 - Triangle Math helpers (SignedArea, Circumcenter, Incircle...)
 - Circle Math helpers (Area, Circumference...)
 - All functions use radians
 - And more!

## Changes
Mathfs.cs **differs a bit from common/standard Math functions (specifically, Unity's)**:
 - All angles are in radians, no methods use degrees
 - Lerp and InverseLerp:
   - Unclamped by default
   - LerpClamped/InverseLerpClamped are now the special case functions/exceptions
   - Uses the more numerically stable evaluation
 - Smoothstep is removed in favor of the more explicit:
   - LerpSmooth (which is how it was implemented) and
   - InverseLerpSmooth (which is how it is implemented everywhere but Unity's Mathf.cs)
 - Min/Max functions with arbitrary inputs/array input will throw on empty instead of returning 0
