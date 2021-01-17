namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;


  public class TDNode {

      public const int POINT = 0;
      public static int SEG = 1;
      public static int LEAF = 2;

      private int type;

      public TDPoint p;
      public TDShape s;

      public TrapezoidFace t;

      public TDNode parent;
      public TDNode leftNode;
      public TDNode rightNode;

      void Awake() {

        p = null;
        s = null;

        t = null;

        parent = null;
        leftNode = null;
        rightNode = null;

      }

      public TDNode() {
      }

      public TDNode(TDPoint _p) {
          p = _p;
          type = POINT;
      }

      public TDNode(TDShape _s) {
          s = _s;
          type = SEG;
      }

      public TDNode(TrapezoidFace _t) {
          t = _t;
          type = LEAF;
          _t.n = this;
      }

      public TDNode(int _type) {
          type = _type;
      }

      public void setType(int _type) {
          type = _type;
      }

      public int getType() {
          return type;
      }

      public string print() {
          string output = "";
          if (type == POINT) {
              output = "P:("+p.x+","+p.y+")";
            }

          if (type == SEG) {
              output = ("S:" + s.index);
          }

          if (type == LEAF){
              output = ("T:" + t.getIndex());
          }

          return output;
      }

      public TDNode getNext(TDPoint p) {
        // x-node
        if(type == POINT)
          return p.right(p) ? rightNode : leftNode;
        // y-node
        else if(type == SEG) {
          return s.above(p) ? leftNode : rightNode;
        }
        return null;
      }

      public bool isLeftNode(TDNode other) {
        return other==null ? (p==null && s==null && t==null) : (leftNode.equals(other) ? true : false) ;
      }

      public bool equals(TDNode other) {

        bool test = false;

        if(other ==null && p==null && s==null && t==null)
          return true;
        else if(other==null)
          return false;
        else{
          if(p != null && other.p != null && p.equals(other.p)) test = true;
          if(s != null && other.s != null && s.equals(other.s)) test = true;
          if(t != null && other.t != null && t.equals(other.t)) test = true;

          if(type != other.getType()) test = false;

          return test;
        }
      }

  }

}
