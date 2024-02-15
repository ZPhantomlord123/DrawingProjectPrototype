using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;
    private Line activeLine;
    public List<GameObject> lines = new List<GameObject>();
    private Vector2 startMousePos;
    public bool isDragging = false;
    private LetterRecognizer letterRecognizer;
    public LineRenderer debugLineRenderer;
    public TextMeshProUGUI debugText;

    private void Start()
    {
        letterRecognizer = GetComponent<LetterRecognizer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !CheckForUI())
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!isDragging && Vector2.Distance(startMousePos, currentMousePos) > 0.1f)
            {
                isDragging = true;
                CreateNewLine();
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            activeLine = null;
            isDragging = false;
        }

        if (activeLine != null && isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.UpdateLine(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            UndoLastLine();
        }
    }

    private void CreateNewLine()
    {
        GameObject newLineGO = Instantiate(linePrefab);
        activeLine = newLineGO.GetComponent<Line>();
        lines.Add(newLineGO);
    }

    public void UndoLastLine()
    {
        if (lines.Count > 0)
        {
            GameObject lastLineGO = lines[lines.Count - 1];
            lines.RemoveAt(lines.Count - 1);
            Destroy(lastLineGO);
        }
        isDragging = false;
    }

    private bool CheckForUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("UI");
        }
        return false;
    }

    // New method to be attached to a button for checking the match for letter 'a'
    public void CheckMatchForLetterA()
    {
        List<Vector3> allPoints = new List<Vector3>(); // Use Vector3 for LineRenderer

        // Combine all points from each stroke into a single list
        foreach (GameObject lineGO in lines)
        {
            Line lineComponent = lineGO.GetComponent<Line>();
            if (lineComponent != null)
            {
                foreach (var point in lineComponent.GetPoints())
                {
                    allPoints.Add(new Vector3(point.x, point.y, 0)); // Convert Vector2 to Vector3
                }
            }
        }

        LineRenderer debugline = Instantiate(debugLineRenderer);

        // Update the LineRenderer with the combined points
        if (debugline != null)
        {
            debugline.positionCount = allPoints.Count;
            debugline.SetPositions(allPoints.ToArray());
        }

        List<List<Vector2>> allStrokes = new List<List<Vector2>>();
        foreach (GameObject lineGO in lines)
        {
            Line lineComponent = lineGO.GetComponent<Line>();
            if (lineComponent != null)
            {
                allStrokes.Add(lineComponent.GetPoints());
            }
        }

        bool isAMatch = letterRecognizer.Recognize("a", allStrokes);
        if (isAMatch)
        {
            debugText.text = "Match found for 'a'.";
        }
        else
        {
            debugText.text = "No match found for 'a'.";
        }
        Debug.Log(isAMatch ? "Match found for 'a'." : "No match found for 'a'.");
    }

}
