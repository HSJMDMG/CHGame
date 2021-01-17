namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;

  public class TrapezoidFace {
    public TDShape top;
    public TDShape bottom;
    public TDPoint leftp;
    public TDPoint rightp;

    public TrapezoidFace upperLeft;
    public TrapezoidFace lowerLeft;
    public TrapezoidFace upperRight;
    public TrapezoidFace lowerRight;

    public TDNode n = null;

    public bool merged = false;
    public bool selected = false;

    private int index = 0;

    public TrapezoidFace() {
      top = null;
      bottom = null;
      leftp = null;
      rightp = null;

      upperLeft = null;
      lowerLeft = null;
      upperRight = null;
      lowerRight = null;
    }

    public TrapezoidFace(TDShape atop, TDShape abottom, TDPoint aleftp, TDPoint arightp) {
      top = atop;
      bottom = abottom;
      leftp = aleftp;
      rightp = arightp;

      upperLeft = null;
      lowerLeft = null;
      upperRight = null;
      lowerRight = null;
    }

    public TrapezoidFace(TDShape atop, TDShape abottom, TDPoint aleftp, TDPoint arightp, TrapezoidFace aupperLeft, TrapezoidFace alowerLeft, TrapezoidFace aupperRight, TrapezoidFace alowerRight) {
      top = atop;
      bottom = abottom;
      leftp = aleftp;
      rightp = arightp;
      upperLeft = aupperLeft;
      lowerLeft = alowerLeft;
      upperRight = aupperRight;
      lowerRight = alowerRight;
    }

    public int getIndex() {
      return index;
    }

    public void setIndex(int a_index) {
      index = a_index;
    }

    public TDShape getTop(){
      return top;
    }
    public TDShape getBottom() {
      return bottom;
    }
    public TDPoint getLeft() {
      return leftp;
    }
    public TDPoint getRight() {
      return rightp;
    }
    public void setTop(TDShape a_top) {
      top = a_top;
    }
    public void setBottom(TDShape a_bottom) {
      bottom = a_bottom;
    }
    public void setLeft(TDPoint a_leftp) {
      leftp = a_leftp;
    }
    public void setRight(TDPoint a_rightp) {
      rightp = a_rightp;
    }

    public void setNeighbors(TrapezoidFace a_upperLeft, TrapezoidFace a_lowerLeft, TrapezoidFace a_upperRight, TrapezoidFace a_lowerRight) {
      upperLeft = a_upperLeft;
      lowerLeft = a_lowerLeft;
      upperRight = a_upperRight;
      lowerRight = a_lowerRight;
    }

    public List<TrapezoidFace> getNeighbors() {
      List<TrapezoidFace> neighbors = new List<TrapezoidFace>();
      if(upperLeft != null) neighbors.Add(upperLeft);
      if(lowerLeft != null) neighbors.Add(lowerLeft);
      if(upperRight != null) neighbors.Add(upperRight);
      if(lowerRight != null) neighbors.Add(lowerRight);
      return neighbors;
    }


    public void draw(Graphics g, int width, int height) {

      // Point classification:
     	//1. Leftp is in top, rightp is in bottom
      // 2. Leftp is in top, rightp is in neither
      // * 3. Leftp is in bottom, rightp is in neither
      // * 4. Leftp is in bottom, rightp is in top
      // * 5. Rightp is in top, leftp is in bottom
      // * 6. Rightp is in top, leftp is in neither
      // * 7. Rightp is in bottom, leftp is in neither
      // * 8. Rightp is in bottom, leftp is in top
      // * 9. Leftp is in top, rightp is in top
      // * 10.Leftp is in bottom, rightp is in bottom


      TDPoint l = new TDPoint(0,0);
      TDPoint r = new TDPoint(width,height);

      if(leftp!=null) {
        l.x = leftp.x;
        l.y = leftp.y;
      }

      if(rightp!=null) {
        r.x = rightp.x;
        r.y = rightp.y;
      }

      if(l.x > r.x) {
      	int tx = l.x;
      	int ty = l.y;
      	l.x = r.x;
      	l.y = r.y;
      	r.x = tx;
      	r.y = ty;
      }



        if(top==null) r.y = 0;
        if(bottom==null) l.y = height;
        /*
        if(selected) g.setColor(Color.green);
        g.drawString("" + index,(r.x+l.x)/2,(r.y+l.y)/2);
        g.setColor(Color.blue);
        */

        //[TODO]TO CHECK: is this draw a face?

            /*
        if(top!=null) {
          g.drawLine(l.x,l.y,l.x,top.intersect(l));
          g.drawLine(r.x,r.y,r.x,top.intersect(r));
        }
        else {
          g.drawLine(l.x,l.y,l.x,0);
          g.drawLine(r.x,r.y,r.x,0);
        }
        if(bottom!=null) {
          g.drawLine(l.x,l.y,l.x,bottom.intersect(l));
          g.drawLine(r.x,r.y,r.x,bottom.intersect(r));
        }
        else {
          g.drawLine(l.x,l.y,l.x,height);
          g.drawLine(r.x,r.y,r.x,height);

        }
        */       
    }

    private bool isEmpty() {
      return top==null && bottom==null && leftp==null && rightp==null;
    }

    public bool equals(TrapezoidFace other) {
      return other==null ? isEmpty() : ( (top==null ? other.top==null : top.equals(other.top)) &&
          (bottom==null ? other.bottom==null : bottom.equals(other.bottom)) &&
          (leftp==null ? other.leftp==null : leftp.equals(other.leftp)) &&
          (rightp==null ? other.rightp==null : rightp.equals(other.rightp)) &&
          upperLeft==other.upperLeft &&
          lowerLeft==other.lowerLeft &&
          upperRight==other.upperRight &&
          lowerRight==other.lowerRight);

    }

  }

}
