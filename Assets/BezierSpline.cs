using UnityEngine;
using System.Collections;
using System;

//TODO Maybe a plain class
//TODO implement N-surfaces?  :)
using System.Text;


[Serializable]
public class BezierSpline {//: MonoBehaviour {
    private int mOrder;
	//TODO Do
	//private bool mReuseEnds = true;
	// Or
	private int mPointReuseCount = 1;
	//TODO Reverse order could improve efficiency, 40% chance
	// dim, point
	private float[,] mPoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /**
     * Currently [dim, point]
     */
    public void SetPoints(float[,] points) {
        int pts = points.GetLength(1);
        if ((pts - mPointReuseCount) % (mOrder + 1 - mPointReuseCount) != 0) {
            throw new System.Exception("invalid # of points for order " + mOrder + " curve with " + mPointReuseCount + " point reuse");
        }
        int segs = (pts - mPointReuseCount) / (mOrder + 1 - mPointReuseCount);
        //Debug.Log("Segments: " + segs);
        this.mPoints = points;
    }

    /**
     * 2 for quadratic, 3 for cubic, etc.
     */
    public void SetOrder(int order) {
        this.mOrder = order;
    }

    public float[] Interpolate(float t) {
		//TODO Clamp?
        //TODO Optimize?
        int segs = (mPoints.GetLength(1) - mPointReuseCount) / (mOrder + 1 - mPointReuseCount);
        int seg = Mathf.FloorToInt(segs * t);
        //TODO Make better
        if (t == 1) {
            seg = segs - 1;
        }
        float width = 1f / segs;
        t = (t - (width * seg)) / width;
        int i = seg * (mOrder + 1 - mPointReuseCount);
        // i represents current curve, as an offset
        float[,] curPoints = ReduceOrder(mPoints, i, mOrder + 1, t);
        for (int j = 0; j < (mOrder - 1); j++) {
            curPoints = ReduceOrder(curPoints, 0, mOrder - j, t);
        }
        float[] result = new float[curPoints.GetLength(0)];
        for (int d = 0; d < curPoints.GetLength(0); d++) {
            result[d] = curPoints[d, 0];
        }
        return result;
    }

    /**
     * [dim, points], t
     * returns kinda [dim, points - 1], having been interpolated
     * 
     * except, I added offset and count.
     */
    private static float[,] ReduceOrder(float[,] points, int offset, int count, float t) {
        if (offset == -1) {
            offset = 0;
        }
        if (count == -1) {
            count = points.GetLength(1);
        }
        int dims = points.GetLength(0);
        float[,] result = new float[dims, count - 1];
        //TODO Optimize?
        for (int i = offset; i < (offset + count - 1); i++) {
            for (int d = 0; d < dims; d++) {
                result[d, i - offset] = (points[d, i]*(1 - t)) + (points[d, i + 1]*t);
            }
        }
        return result;
    }

	public static float Clamp(float x, float min, float max) {
		return Mathf.Max(Mathf.Min(x, max), min);
	}

	public string ToJSON() {
		StringBuilder sb = new StringBuilder();
		sb.Append("{");
		sb.Append("mOrder:" + mOrder);
		sb.Append(", ");
		sb.Append("mPointReuseCount:" + mPointReuseCount);
		sb.Append(", ");
		sb.Append("[");
		//TODO Hmm.  Maybe I should consider flipping order.
		for (int dim = 0; dim < 3; dim++) {
			if (dim > 0) {
				sb.Append(", ");
			}
			sb.Append("[");
			for (int point = 0; point < mPoints.GetLength(1); point++) {
				if (point > 0) {
					sb.Append(", ");
				}
				sb.Append(mPoints[dim, point]);
			}
			sb.Append("]");
		}
		sb.Append("]");
		sb.Append("}");

		return sb.ToString();
	}

	public static BezierSpline FromJSON(string json) {
		throw new Exception("Not yet implemented");
	}
}
