﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace g3 {

	public struct Arc2d : IParametricCurve2d
	{
		public Vector2d Center;
		public double Radius;
		public double AngleStartDeg;
		public double AngleEndDeg;
		public bool IsReversed;		// use ccw orientation instead of cw
		

		public Arc2d(Vector2d center, double radius, double startDeg, double endDeg)
		{
			IsReversed = false;
			Center = center;
			Radius = radius;
			AngleStartDeg = startDeg;
			AngleEndDeg = endDeg;
			if ( AngleEndDeg < AngleStartDeg )
				AngleEndDeg += 360;

			// [TODO] handle full arcs, which should be circles?
		}


		public bool IsClosed {
			get { return false; }
		}


		public double ParamLength {
			get { return 1.0f; }
		}


		// t in range[0,1] spans arc
		public Vector2d SampleT(double t) {
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
			double c = Math.Cos(theta), s = Math.Sin(theta);
			return new Vector2d(Center.x + Radius*c, Center.y + Radius*s);
		}


        public Vector2d TangentT(double t)
        {
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
            Vector2d tangent = new Vector2d(-Math.Sin(theta), Math.Cos(theta));
            tangent.Normalize();
            return tangent;
        }


		public bool HasArcLength { get {return true;} }

		public double ArcLength {
			get {
				return (AngleEndDeg-AngleStartDeg) * MathUtil.Deg2Rad * Radius;
			}
		}

		public Vector2d SampleArcLength(double a) {
			double t = a / ArcLength;
			double theta = (IsReversed) ?
				(1-t)*AngleEndDeg + (t)*AngleStartDeg : 
				(1-t)*AngleStartDeg + (t)*AngleEndDeg;
			theta = theta * MathUtil.Deg2Rad;
			double c = Math.Cos(theta), s = Math.Sin(theta);
			return new Vector2d(Center.x + Radius*c, Center.y + Radius*s);
		}

		public void Reverse() {
			IsReversed = ! IsReversed;
		}




        public double Distance(Vector2d point)
        {
            Vector2d PmC = point - Center;
            double lengthPmC = PmC.Length;
            if (lengthPmC > MathUtil.Epsilon) {
                Vector2d dv = PmC / lengthPmC;
                double theta = Math.Atan2(dv.y, dv.x);
                if (theta < AngleStartDeg || theta > AngleEndDeg) {
                    theta = MathUtil.Clamp(theta, AngleStartDeg * MathUtil.Deg2Rad, AngleEndDeg * MathUtil.Deg2Rad);
                    double c = Math.Cos(theta), s = Math.Sin(theta);
                    Vector2d pos = new Vector2d(Center.x + Radius * c, Center.y + Radius * s);
                    return pos.Dist(point);
                } else {
                    return Math.Abs(lengthPmC - Radius);
                }
            } else {
                return Radius;
            }
        }


        public Vector2d NearestPoint(Vector2d point)
        {
            Vector2d PmC = point - Center;
            double lengthPmC = PmC.Length;
            if (lengthPmC > MathUtil.Epsilon) {
                Vector2d dv = PmC / lengthPmC;
                double theta = Math.Atan2(dv.y, dv.x);
                theta *= MathUtil.Rad2Deg;
                theta = MathUtil.ClampAngleDeg(theta, AngleStartDeg, AngleEndDeg);
                theta = MathUtil.Deg2Rad * theta;
                double c = Math.Cos(theta), s = Math.Sin(theta);
                return new Vector2d(Center.x + Radius * c, Center.y + Radius * s);
            } else 
                return SampleT(0.5);        // all points equidistant
        }


	}
}
