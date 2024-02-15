using System.Collections.Generic;
using UnityEngine;

public class LetterRecognizer : MonoBehaviour
{
    public bool Recognize(string character, List<List<Vector2>> allStrokes)
    {
        // Combine all stroke points into a single list
        List<Vector2> combinedPoints = new List<Vector2>();
        foreach (var stroke in allStrokes)
        {
            combinedPoints.AddRange(stroke);
        }

        switch (character)
        {
            case "a":
                return RecognizeA(combinedPoints);
            // Add more cases for other characters as needed
            default:
                return false;
        }
    }

    private bool RecognizeA(List<Vector2> strokePoints)
    {
        bool hasLoop = DetectLoop(strokePoints);
        bool hasTail = hasLoop && DetectTail(strokePoints);
        return hasLoop && hasTail;
    }

    private bool DetectLoop(List<Vector2> points)
    {
        for (int i = 1; i < points.Count - 1; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                if (Vector2.Distance(points[i], points[j]) < 0.1f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool DetectTail(List<Vector2> points)
    {
        if (points.Count < 2) return false;
        Vector2 lastPoint = points[points.Count - 1];
        Vector2 secondLastPoint = points[points.Count - 2];
        Vector2 direction = (lastPoint - secondLastPoint).normalized;
        return direction.y < -0.5 || direction.x > 0.5;
    }
}
