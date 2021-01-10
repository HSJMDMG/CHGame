using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for convex hull level, containing point set.
/// </summary>
[CreateAssetMenu(fileName = "P2LevelNew", menuName = "Levels/P2 Level")]
public class P1PtsLevel : ScriptableObject
{
    [Header("P2")]
    public List<Vector2> Points = new List<Vector2>();
}
