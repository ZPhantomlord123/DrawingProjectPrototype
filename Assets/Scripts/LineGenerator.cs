using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;
    private Line activeLine;
    public List<GameObject> lines = new List<GameObject>(); // List to keep track of all line instances
    private Vector2 startMousePos; // To check if the mouse has moved sufficiently to start a line
    public bool isDragging = false; // To track if the user is dragging
    public bool check = false;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if(!CheckForUI()) 
            {
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (!isDragging) // Ensure we reset dragging state when starting a new drag
                {
                    isDragging = false;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!isDragging && Vector2.Distance(startMousePos, currentMousePos) > 0.1f) // Check if the mouse has moved enough to consider it a drag
            {
                isDragging = true; // Start dragging
                CreateNewLine();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                activeLine = null;
                isDragging = false; // Stop dragging
            }
        }

        if (activeLine != null && isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            activeLine.UpdateLine(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.LeftControl)) // Assuming Z key is for undo
        {
            UndoLastLine();
        }
    }

    private void CreateNewLine()
    {
        GameObject newLine = Instantiate(linePrefab);
        activeLine = newLine.GetComponent<Line>();
        lines.Add(newLine); // Add the new line to the list
    }

    public void UndoLastLine()
    {
        if (lines.Count > 0)
        {
            GameObject lastLine = lines[lines.Count - 1]; // Get the last line drawn
            lines.RemoveAt(lines.Count - 1); // Remove it from the list
            Destroy(lastLine); // Destroy the line GameObject
        }
        // Reset dragging state after undoing a line to ensure we can start a new line immediately
        isDragging = false;
    }

    private bool CheckForUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                // Hit a UI element, do not proceed with game object interaction
                return true;
            }
        }
        return false;
    }
}
