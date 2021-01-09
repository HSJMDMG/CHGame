using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for convex hull level, containing point set.
/// </summary>
[CreateAssetMenu(fileName = "P1PtsLevelNew", menuName = "Levels/P1 Most Points Level")]
public class P1PtsLevel : ScriptableObject
{
    [Header("P1 Point Mode Points")]
    public List<Vector2> Points = new List<Vector2>();
}
