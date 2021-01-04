using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for convex hull level, containing point set.
/// </summary>
[CreateAssetMenu(fileName = "P1AreaLevelNew", menuName = "Levels/P1 Largest Area Level")]
public class P1AreaLevel : ScriptableObject
{
    [Header("P1 Area Mode Points")]
    public List<Vector2> Points = new List<Vector2>();
}
