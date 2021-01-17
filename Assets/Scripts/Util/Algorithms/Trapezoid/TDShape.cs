
namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;

  public class TDShape {
    //	private Point[] points;
    private List<TDPoint> points; //= new ArrayList<Point>();

    public int index = 0;

    public TDShape() {
      points = new List<TDPoint>();
    }

    public TDShape(List<TDPoint> a_points) {
      points = a_points;
    }

  	public int numPoints(){
  		return points.Count;
  	}

  	public List<TDPoint> getPoints() {
  		return points;
  	}

  	public void setPoints(List<TDPoint> a_points) {
  		points = a_points;
  	}

    public TDPoint getFirst() {
  //    return points.get(0).x > points.get(points.size() -1).x ? points.get(0) : points.get(points.size() -1) ;
      return points[0];
    }

    public TDPoint getLast() {
  //    return points.get(0).x <= points.get(points.size() -1).x ? points.get(0) : points.get(points.size() -1) ;
      return points[points.Count - 1];

    }

    public bool above(TDPoint p) {

      if(contains(p)) return false;

      // Assume that p is within the proper x coords of the shape
      int x1 = getFirst().x;
      int x2 = getLast().x;
      int y1 = getFirst().y;
      int y2 = getLast().y;
      return (p.y < lineAprox(x1,y1,x2,y2,p.x));

    }

    public bool below(TDPoint p) {

      if(contains(p)) return false;

      // Assume that p is within the proper x coords of the shape
      int x1 = getFirst().x;
      int x2 = getLast().x;
      int y1 = getFirst().y;
      int y2 = getLast().y;
      return (p.y > lineAprox(x1,y1,x2,y2,p.x));

    }

    private float lineAprox(int x1, int y1, int x2, int y2, int x){
  //    return (float)(((y2-y1)/(x2-x1))*(x - x1)+y1);
      return (float)((float)(((float)(y2-y1)/(x2-x1))*(x-x1))+y1);
    }

    private bool consider(TDPoint a, TDPoint b, TDPoint c) {
      return ((a.x > b.x && a.x < c.x ) || (a.x > c.x && a.x < b.x) /* && (a.y > b.y || a.y > c.y) */ );
    }

    public bool contains(TDPoint pt) {
      return points.Contains(pt);
    }

    public int intersect(TDPoint pt) {
      int x1 = getFirst().x;
      int y1 = getFirst().y;
      int x2 = getLast().x;
      int y2 = getLast().y;
      return (int)Mathf.Round(lineAprox(x1,y1,x2,y2,pt.x) );
    }

    /* Intersect the given trapezoidal line with
     * all other lines in this shape. */
    public float intersect(TrapezoidLine t,int max) {
      float min = max;

      foreach (TDPoint pt in points) {
          if(points.IndexOf(pt) > 0){
            TDPoint prevpt = points[points.IndexOf(pt) - 1];
            if(consider(t.getStart(), pt, prevpt)) {
              min = Mathf.Min(min,(float)(pt.y - prevpt.y)/(pt.x-prevpt.x)*(t.getStart().x-prevpt.x)+prevpt.y);
            }
          }
        }
        return min;

      }


  	public bool equals(TDShape s) {
  		return s==null ? points==null : s.getPoints().Equals(points);
  	}

        //TODO: draw shape


        public void draw() { }
        /*

public void draw(Graphics g) {

  for(int i = 0; i<points.Count); i++) {
    g.setColor(Color.red);
    points.get(i).draw(g);
    if(i>0) points.get(i).drawLine(g,points.get(i-1));
  }
}
*/
    }


}
