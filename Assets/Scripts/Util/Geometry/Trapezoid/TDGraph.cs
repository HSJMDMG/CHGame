
namespace Util.Geometry.Trapezoid {

  using System;
  using UnityEngine;
  using Util.Math;

  public class  TDGraph {

    private TDNode root;

    void Awake() {
      root = null;
    }

    public TDGraph() {
    }

    public TDGraph(TDNode _root) {
      root = _root;
    }

    public TrapezoidFace retrieve(TDPoint p) {

      TDNode nextNode = root;
      while(nextNode.getType() != TDNode.LEAF) {
        nextNode = nextNode.getNext(p);
      }
      return nextNode.t;
    }


    public void preorderTraverseTree(TDNode n) {
      String s = "";
      int d = distanceToRoot(n);
      for(int i = 1; i<d; i++) {
        s = s + " ";
      }
      //Debug.Log(s + n.print());

      if (n.leftNode != null) preorderTraverseTree(n.leftNode);
      if (n.rightNode != null) preorderTraverseTree(n.rightNode);
    }

    public void inorderTraverseTree(TDNode n) {
  	    if (n.leftNode != null) inorderTraverseTree(n.leftNode);
  	    String s = "";
  	    int d = distanceToRoot(n);
  	    for(int i = 1; i<d; i++) {
  	      s = s + " ";
  	    }
  	    //Debug.Log(s + n.print());
  	    if (n.rightNode != null) inorderTraverseTree(n.rightNode);
    }

    public void postorderTraverseTree(TDNode n) {
  	    if (n.leftNode != null) postorderTraverseTree(n.leftNode);
  	    if (n.rightNode != null) postorderTraverseTree(n.rightNode);
  	    String s = "";
  	    int d = distanceToRoot(n);
  	    for(int i = 1; i<d; i++) {
  	      s = s + " ";
  	    }
  	    //Debug.Log(s + n.print());
      }

    public void traverseTree() {
  	  //Debug.Log("Preordered Nodes:");
      preorderTraverseTree(root);
       //Debug.Log("Inordered Nodes:");
      inorderTraverseTree(root);
       //Debug.Log("Postordered Nodes:");
      postorderTraverseTree(root);
       //Debug.Log("Search Tree Output Complete");

     }

    private int distanceToRoot(TDNode n) {
      TDNode step = n;
      int distance = 0;
      do {
        distance++;
        step = step.parent;
      } while(step != null);

      return distance;
    }

    public TDNode retrieveNode(TDPoint p) {

  	  //Debug.Log("Retrieving p=("+p.x+","+p.y+")");

      TDNode nextNode = root;
      int steps = 0;
      String s = "";
      while(nextNode.getType() != TDNode.LEAF) {
        //Debug.Log( s+nextNode.print());
        nextNode = nextNode.getNext(p);
        steps++;
        s=s+" ";
      }
      //Debug.Log("Steps: "+steps);
      return nextNode;
    }

    public void Add(TDNode n) {
      if(root == null)
        root = n;
    }

  }

}
