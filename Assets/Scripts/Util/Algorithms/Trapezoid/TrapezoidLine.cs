namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;


  public class TrapezoidLine {

  	private TDPoint start;
    private TDPoint end;
  	private int length;
    private bool up;

  	public TrapezoidLine() {
  		length = 0;
  		start = new TDPoint();
      end = new TDPoint();
  	}
  	public TrapezoidLine(TDPoint a_start, int a_length, bool a_up) {
  		//super();
  		start = a_start;
  		length = a_length;
      up = a_up;
      end = new TDPoint(start.x,up ? start.y - length : start.y + length);
  	}

    public TrapezoidLine(TDPoint a_start, TDPoint a_end) {
      start = a_start;
      end = a_end;
      length = Mathf.Abs(start.y - end.y);
      up = (start.y - end.y > 0);
    }

  	public TrapezoidLine(int x1, int y1, int x2, int y2) {
      start = new TDPoint(x1,y1);
      end = new TDPoint(x2,y2);
      length = Mathf.Abs(y1 - y2);
      up = (y1 - y2 > 0);
    }

  	public TrapezoidLine(int a_x, int a_y, int a_length, bool a_up) {
  		
  		start = new TDPoint(a_x,a_y);
      end = new TDPoint(a_x,a_up ? a_y-a_length : a_y+a_length);
  		length = a_length;
      up = a_up;
  	}

  	public int getLength() {
  		return length;
  	}
  	public void setLength(int a_length) {
  		length = a_length;
      end.y = up ? start.y-length : start.y+length;
  	}
  	public TDPoint getStart() {
  		return start;
  	}
  	public TDPoint getEnd() {
  		return end;
  	}
  	public void setStart(TDPoint a_start) {
  		start = a_start;
  	}
  	public void setEnd(TDPoint a_end) {
  		end = a_end;
      length = Mathf.Abs(start.y - end.y);
      up = (start.y - end.y > 0);
  	}
    public void setUp(bool a_up) {
      up = a_up;
      end.y = up ? start.y-length : start.y+length;
    }
    public bool getUp() {
      return up;
    }

  	public bool equals(TrapezoidLine t) {
  		return (t.start.equals(start) &&
          t.length==length &&
          t.getUp() == up);
  	}

        //TODO: draw Lines of Trapezoid
        public void draw()
        { }
        /*

            public void draw(Graphics g) {

      g.setColor(Color.blue);
      if(up) {
        g.drawLine(start.x,start.y,start.x,start.y - length);
      }
      else {
        g.drawLine(start.x,start.y,start.x,start.y + length);
      }
    }*/


  }


}
