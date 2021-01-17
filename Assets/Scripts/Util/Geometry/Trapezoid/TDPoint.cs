namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;



	public class TDPoint {

		public float x;
		public float y;

		public TDPoint() {
			x = 0f;
			y = 0f;
		}
		public TDPoint(float _x, float _y) {
			x = _x;
			y = _y;
		}

		public bool equals(TDPoint p) {
			return ((p.x == x) && (p.y == y));
		}

	  public bool right(TDPoint p) {
	    return (x >= p.x);
	  }

		//TODO: fix this draw point method
		public void draw() {

		}
	  /*public void draw(Graphics g) {
	    g.fillOval(x-4,y-4,7,7);
	  }
		*/
	  public string print() {
	    return "(" + x.ToString() + "," + y.ToString() + ")";
	  }

		//TODO: fix this draw line method
		public void drawLine() {

		}
		/*
	  public void drawLine(Graphics g, TDPoint other) {
	    g.setColor(Color.black);
	    g.drawLine(x,y,other.x,other.y);
	  }
		*/
	}
	}
