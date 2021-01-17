namespace Util.Geometry.Trapezoid
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Util.Geometry.Polygon;
	using Util.Math;


  public class TrapezoidalMap {

  	// private Shape[] shapes;
  	private List<TDShape> shapes = new List<TDShape>();

  	private TDGraph Dgraph;

  	public TrapezoidalMap() {
  		shapes = new List<TDShape>();
  	}

  	public TrapezoidalMap(List<TDShape> a_shapes) {
  		shapes = a_shapes;
  	}

  	public void setShape(List<TDShape> a_shapes) {
  		shapes = a_shapes;
  	}


    //Do increamental construction, return built faces
  	public List<TrapezoidFace> incrementalMap(int height, int width) {
            int i;
  		// Set<TrapezoidLine> trapezoidMap = new List<TrapezoidLine>();
  		List<TrapezoidFace> trapezoidFaces = new List<TrapezoidFace>();

  		List<TDShape> segments =  new List<TDShape>(shapes);

  			/*		int index = 0;
  					for (Shape s : segments) {
  						s.index = index;
  						index++;
  					}
  			*/
  		// T(nil)
  		TrapezoidFace nilFace = new TrapezoidFace();
  		trapezoidFaces.Add(nilFace);

  		// Add T(nil) to D
  		// Graph Dgraph = new Graph();
  		Dgraph = new TDGraph();
  		Dgraph.Add(new TDNode(nilFace));

  		// Debug
  		// Dgraph.traverseTree();

  		while (segments.Count > 0) {
  			TDShape seg = randomShape(segments);
  			segments.Remove(seg);
  			//			System.out.println("Segment: "+seg.index);

  			List<TrapezoidFace> intersectedFaces = FollowSegment(seg);

  			//			System.out.println("Intersected: " + intersectedFaces.size());

  			TDPoint p = seg.getFirst();
  			TDPoint q = seg.getLast();
            

  			if (intersectedFaces.Count == 1) {
  				//segment lies within 1 face, split this face;

  				TrapezoidFace d = intersectedFaces[0];
  				trapezoidFaces.Remove(d);
  				TrapezoidFace A = new TrapezoidFace(d.top, d.bottom, d.leftp, p);
  				TrapezoidFace C = new TrapezoidFace(d.top, seg, p, q);
  				TrapezoidFace D = new TrapezoidFace(seg, d.bottom, p, q);
  				TrapezoidFace B = new TrapezoidFace(d.top, d.bottom, q,d.rightp);

  				A.setNeighbors(d.upperLeft, d.lowerLeft, C, D);
  				C.setNeighbors(A, A, B, B);
  				D.setNeighbors(A, A, B, B);
  				B.setNeighbors(C, D, d.upperRight, d.lowerRight);

  				if(d.upperLeft != null) {
  					d.upperLeft.upperRight = A;
  					d.upperLeft.lowerRight = A;
  				}
  				if(d.lowerLeft != null) {
  					d.lowerLeft.upperRight = A;
  					d.lowerLeft.lowerRight = A;
  				}

  				if(d.upperRight != null) {
  					d.upperRight.upperLeft = B;
  					d.upperRight.lowerLeft = B;
  				}

  				if(d.lowerRight != null) {
  					d.lowerRight.upperLeft = B;
  					d.lowerRight.lowerLeft = B;
  				}

  				trapezoidFaces.Add(A);
  				trapezoidFaces.Add(B);
  				trapezoidFaces.Add(C);
  				trapezoidFaces.Add(D);

  				TDNode subRoot = d.n;// Dgraph.retrieveNode(p);

  				// Switch node over
  				subRoot.t = null;
  				subRoot.setType(TDNode.POINT);
  				subRoot.p = p;

  				// Set left node
  				subRoot.leftNode = new TDNode(A);
  				subRoot.leftNode.parent = subRoot;

  				// Set right tree
  				subRoot.rightNode = new TDNode(q);
  				subRoot.rightNode.parent = subRoot;

  				subRoot.rightNode.rightNode = new TDNode(B);
  				subRoot.rightNode.rightNode.parent = subRoot.rightNode;

  				subRoot.rightNode.leftNode = new TDNode(seg);
  				subRoot.rightNode.leftNode.parent = subRoot.rightNode;

  				subRoot.rightNode.leftNode.leftNode = new TDNode(C);
  				subRoot.rightNode.leftNode.leftNode.parent = subRoot.rightNode.leftNode;
  				subRoot.rightNode.leftNode.rightNode = new TDNode(D);

  				subRoot.rightNode.leftNode.rightNode.parent = subRoot.rightNode.leftNode;

  			} else {
  				i = 0;

  				List<TrapezoidFace> newFaces = new List<TrapezoidFace>();

  				TrapezoidFace prevUpper = null;
  				TrapezoidFace prevLower = null;


  				foreach (TrapezoidFace d in intersectedFaces) {
  					//					if(!trapezoidFaces.contains(d)) System.out.println("AAAAAHHHHHHHHH!!!!");
  					//Intersect with multiple faces
  					// First
  					if (i == 0) {
  						trapezoidFaces.Remove(d);

  						TrapezoidFace A = new TrapezoidFace(d.top, d.bottom,d.leftp, p);
  						TrapezoidFace B = new TrapezoidFace(d.top, seg, p,d.rightp);
  						TrapezoidFace C = new TrapezoidFace(seg, d.bottom, p,d.rightp);

  						A.setNeighbors(d.upperLeft, d.lowerLeft, B, C);
  						B.setNeighbors(A, A, null, null);
  						C.setNeighbors(A, A, null, null);

  						if(d.upperLeft!=null) {
  							//if(d.upperLeft.lowerRight==d.upperLeft.upperRight)
  								d.upperLeft.upperRight = A;
  							d.upperLeft.lowerRight = A;
  						}
  						if(d.lowerLeft!=null) {
  							//if(d.lowerLeft.upperRight==d.upperLeft.upperRight)
  								d.lowerLeft.lowerRight = A;
  							d.lowerLeft.upperRight = A;

  						}

  						trapezoidFaces.Add(A);
  						newFaces.Add(B);
  						newFaces.Add(C);

  						prevUpper = B;
  						prevLower = C;

  						TDNode subRoot = d.n; // Dgraph.retrieveNode(p);

  						subRoot.t = null;
  						subRoot.setType(TDNode.POINT);
  						subRoot.p = p;

  						// Set left node
  						subRoot.leftNode = new TDNode(A);
  						subRoot.leftNode.parent = subRoot;

  						// Set right tree
  						subRoot.rightNode = new TDNode(seg);
  						subRoot.rightNode.parent = subRoot;

  						subRoot.rightNode.leftNode = new TDNode(B);
  						subRoot.rightNode.leftNode.parent = subRoot.rightNode;

  						subRoot.rightNode.rightNode = new TDNode(C);
  						subRoot.rightNode.rightNode.parent = subRoot.rightNode;

  						foreach(TrapezoidFace e in trapezoidFaces) {
  							if(!trapezoidFaces.Contains(e.upperLeft)) e.upperLeft=null;
  							if(!trapezoidFaces.Contains(e.lowerLeft)) e.lowerLeft=null;
  							if(!trapezoidFaces.Contains(e.upperRight)) e.upperRight=null;
  							if(!trapezoidFaces.Contains(e.lowerRight)) e.lowerRight=null;
  						}

  					}
  					// Last
  					else if (i == intersectedFaces.Count - 1) {
  						trapezoidFaces.Remove(d);

  						TrapezoidFace B = new TrapezoidFace(d.top, seg,d.leftp, q);
  						TrapezoidFace C = new TrapezoidFace(seg, d.bottom,d.leftp, q);
  						TrapezoidFace A = new TrapezoidFace(d.top, d.bottom, q,d.rightp);

  						B.setNeighbors(prevUpper, prevUpper, A, A);
  						C.setNeighbors(prevLower, prevLower, A, A);
  						A.setNeighbors(B, C, d.upperRight, d.lowerRight);

  						prevUpper.upperRight = B;
  						prevUpper.lowerRight = B;
  						prevLower.upperRight = C;
  						prevLower.lowerRight = C;

  						if(d.upperRight!=null) {
  							//if(d.upperRight.upperLeft==d.upperRight.lowerLeft)
  								d.upperRight.upperLeft = A;
  							d.upperRight.lowerLeft = A;
  						}
  						if(d.lowerRight!=null) {

  							//if(d.lowerRight.upperLeft==d.lowerRight.lowerLeft)
  								d.lowerRight.lowerLeft = A;
  							d.lowerRight.upperLeft = A;
  						}



  						trapezoidFaces.Add(A);
  						newFaces.Add(B);
  						newFaces.Add(C);

  						TDNode subRoot = d.n; // Dgraph.retrieveNode(q);

  						subRoot.t = null;
  						subRoot.setType(TDNode.POINT);
  						subRoot.p = q;

  						// Set right node
  						subRoot.rightNode = new TDNode(A);
  						subRoot.rightNode.parent = subRoot;

  						// Set left node
  						subRoot.leftNode = new TDNode(seg);
  						subRoot.leftNode.parent = subRoot;

  						subRoot.leftNode.leftNode = new TDNode(B);
  						subRoot.leftNode.leftNode.parent = subRoot.leftNode;

  						subRoot.leftNode.rightNode = new TDNode(C);
  						subRoot.leftNode.rightNode.parent = subRoot.leftNode;

  						foreach (TrapezoidFace e in trapezoidFaces) {
  							if(!trapezoidFaces.Contains(e.upperLeft)) e.upperLeft=null;
  							if(!trapezoidFaces.Contains(e.lowerLeft)) e.lowerLeft=null;
  							if(!trapezoidFaces.Contains(e.upperRight)) e.upperRight=null;
  							if(!trapezoidFaces.Contains(e.lowerRight)) e.lowerRight=null;
  						}
  					}
  					// Middle
  					else {
  						trapezoidFaces.Remove(d);

  						TrapezoidFace A = new TrapezoidFace(d.top, seg,
  								d.leftp, d.rightp);
  						TrapezoidFace B = new TrapezoidFace(seg, d.bottom,
  								d.leftp, d.rightp);

  						A.setNeighbors(prevUpper, prevUpper, null, null);
  						B.setNeighbors(prevLower, prevLower, null, null);

  						prevUpper.upperRight = A;
  						prevUpper.lowerRight = A;
  						prevLower.upperRight = B;
  						prevLower.lowerRight = B;

  						prevUpper = A;
  						prevLower = B;

  						newFaces.Add(A);
  						newFaces.Add(B);

  						TDNode subRoot = d.n;

  						subRoot.t = null;
  						subRoot.setType(TDNode.SEG);
  						subRoot.s = seg;

  						// Set left node
  						subRoot.leftNode = new TDNode(A);
  						subRoot.leftNode.parent = subRoot;

  						subRoot.rightNode = new TDNode(B);
  						subRoot.rightNode.parent = subRoot;

  						foreach (TrapezoidFace e in trapezoidFaces) {
  							if(!trapezoidFaces.Contains(e.upperLeft)) e.upperLeft=null;
  							if(!trapezoidFaces.Contains(e.lowerLeft)) e.lowerLeft=null;
  							if(!trapezoidFaces.Contains(e.upperRight)) e.upperRight=null;
  							if(!trapezoidFaces.Contains(e.lowerRight)) e.lowerRight=null;
  						}

  					}

  					i++;
  				}

  				bool allMerged = false;

  					//				System.out.println("New before merge: " + newFaces.size());

  				// Merge trapezoids
  				while (!allMerged) {
  					foreach (TrapezoidFace d in newFaces) {
  						if (d.rightp != null
  								&& !d.rightp.equals(p)
  								&& !d.rightp.equals(q)
  								&& ((d.top != null && (d.top.above(d.rightp))) || (d.bottom != null && (d.bottom.below(d.rightp))))) {
  							TrapezoidFace next = d.upperRight; // Either should work
  							d.upperRight = next.upperRight;
  							d.lowerRight = next.lowerRight;

  							if ((d.top != null && (d.top.above(d.rightp)))) {
  								d.upperRight.lowerLeft = d;

  							}
  							else {
  								d.upperRight.upperLeft = d;
  							}

  							d.rightp = next.rightp;

  							// Update the node tree as well
  							if (next.n.parent.isLeftNode(next.n))
  								next.n.parent.leftNode = d.n;
  							else
  								next.n.parent.rightNode = d.n;

  							newFaces.Remove(next);

  							//System.out.println("Removed?: "+ newFaces.Remove(next));

  							break;
  						} else {
  							d.merged = true;
  						}
  					}
  					allMerged = true;
  					foreach (TrapezoidFace d in newFaces) {
  						if (!d.merged)
  							allMerged = false;
  					}
  				}

  						//				System.out.println("New after merge: " + newFaces.size());

  				foreach (TrapezoidFace d in newFaces) {
  					d.merged = false;
  					// d.selected=true;
  					trapezoidFaces.Add(d);
  				}
  			}
  			/*			for(TrapezoidFace d : trapezoidFaces) {
  							if(!(trapezoidFaces.contains(d.upperLeft) || d.upperLeft==null) ||
  									!(trapezoidFaces.contains(d.upperRight) || d.upperRight==null ) ||
  									!(trapezoidFaces.contains(d.lowerLeft) || d.lowerLeft==null ) ||
  									!(trapezoidFaces.contains(d.lowerRight) || d.lowerRight==null) )
  								System.out.println("Sanity check failed");
  						}
  			*/
  		}

  		int j = 0;

  		foreach (TrapezoidFace f in trapezoidFaces) {
  			f.setIndex(j);
  			j++;
  		}

  		Dgraph.traverseTree();
  			//		System.out.println("----");

  		return trapezoidFaces;
  	}

    //point location
  	public void retrievePoint(TDPoint p) {
  		Dgraph.retrieveNode(p).t.selected = true;
  			//		System.out.println("Point retrieved");
  	}

  	//compute the faces intersect with segment seg
  	private List<TrapezoidFace> FollowSegment(TDShape seg) {
  		List<TrapezoidFace> traversed = new List<TrapezoidFace>();

  		TDPoint p = seg.getFirst();
  		TDPoint q = seg.getLast();

  			/*		if (p.equals(q))
  						System.out.println("AAAAAAHHHH!");
  			*/
  		TrapezoidFace start = Dgraph.retrieve(p);

  		traversed.Add(start);

  		TrapezoidFace j = start;

  		while (j!=null && (j.rightp != null && q.right(j.rightp))) {
  			if (seg.above(j.rightp))
  				j = j.lowerRight;
  			else
  				j = j.upperRight;

  			if(j!=null)traversed.Add(j);
  		}

  		return traversed;
  	}

  	private TDShape randomShape(List<TDShape> segments) {
  		
  		int n = Random.Range(0, segments.Count - 1);
  		int i = 0;
  		foreach (TDShape seg in segments) {
  			if (i == n)
  				return seg;
  			i = i + 1;
  		}
  		return null;
  	}

  	/*
  	 * Creates trapezoidal maps the "naive" way. More specifically, this
  	 * function iterates through each shape, and each point of each shape,
  	 * creating a TrapezoidLine for each point with an initially long length.
  	 * Then, this length is minimized over all the x intersections for above the
  	 * point. This is repeated for all the lines below the point. This is
  	 * primarily used for drawing the visual lines on the screen (as any time
  	 * savings are already negated when having to draw all objects anyway).
  	 */
  	public List<TrapezoidLine> naiveMap(int height, int width) {

      //return all the vertical lines of trapezoid.
      //TODO:fix the boundary parameter;

  		List<TrapezoidLine> trapezoidMap = new List<TrapezoidLine>();

  		// Add the borders of the window as temporary shapes
  		TDShape border = new TDShape();
  		TDShape border2 = new TDShape();

  		border.getPoints().Add(new TDPoint(0, 0));
  		border.getPoints().Add(new TDPoint(width, 0));

  		border2.getPoints().Add(new TDPoint(0, height));
  		border2.getPoints().Add(new TDPoint(width, height));

  		shapes.Add(border);
  		shapes.Add(border2);

  		// Step through each shape
  		for (int i = 0; i < shapes.Count - 1; i++) {
            TDShape s = shapes[i];
  			TDShape sh = shapes[i + 1];
  			// Ignore the border shapes so their points do not appear as part of
  			// the trapezoidal map
  			if (sh.equals(border) || sh.equals(border2))
  				continue;
  			// Step through the points in the current shape

        for (int pi = 0; pi < sh.getPoints().Count - 1; pi++) {
          TDPoint p = sh.getPoints()[pi];
          TDPoint pt =  sh.getPoints()[pi + 1];

  				// Generate two trapezoidal lines for each point (up and down)
  				TrapezoidLine t = new TrapezoidLine(pt, height + 1, true);
  				TrapezoidLine t2 = new TrapezoidLine(pt, height + 1, false);


  				// Iterate over all shapes, intersecting the trapezoidal lines
  				// with each of the shapes
  				for (int s2i = 0 ; s2i < shapes.Count - 1; s2i ++) {
                    TDShape s2 = shapes[s2i];
                    TDShape sh2 = shapes[s2i + 1];


  					// if(!sh.equals(sh2)) {
  					// If the intersection yields a positive difference that is
  					// smaller than the previous length, update t
  					if (t.getStart().y - sh2.intersect(t, height + 1) > 0) {
  						t.setLength(Mathf.Min((int) (t.getStart().y - sh2
  								.intersect(t, height + 1)), t.getLength()));
  					}
  					// if the intersection yields a negative difference that is
  					// absolutely smaller than the previous length, update t2
  					else if (t2.getStart().y - sh2.intersect(t2, height + 1) < 0) {
  						t2.setLength(Mathf.Min((int) Mathf.Abs(t2.getStart().y
  								- sh2.intersect(t2, height + 1)), t2
  								.getLength()));
  					}
  					// }

  				}
  				// If the lengths have been updated to a reasonable value, Add
  				// them
  				if (t.getLength() < height) {
  					trapezoidMap.Add(t);
  				}
  				if (t2.getLength() < height) {
  					trapezoidMap.Add(t2);
  				}
  			}
  		}

  		// Remove the borders so they are not displayed
  		shapes.Remove(border);
  		shapes.Remove(border2);

  		return trapezoidMap;
  	}
  }



}
