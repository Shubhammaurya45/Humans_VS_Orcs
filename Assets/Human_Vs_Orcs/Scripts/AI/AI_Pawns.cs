using System.Collections.Generic;
using UnityEngine;

public class AI_Pawns : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    private List<Node> currentPath = new();
    private int currentNodeIndex;

    private void Update()
    {
        if (!isPathValid())
            return;

        Node currentNode = currentPath[currentNodeIndex];
        Vector3 targetPosition = new Vector3(currentNode.centerX, currentNode.centerY);
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= 0.15f)
        {
            if (currentNodeIndex == currentPath.Count - 1)
            {
                Debug.Log("Destination Reached");
                currentPath = new();
            }
            else
            {
                currentNodeIndex++;
            }
        }
    }

    public void SetDistination(Vector3 destination)
    {
        if (currentPath.Count > 0)
        {
            Node newEndNode = TilemapManager.Instance.FindNode(destination);
            if (newEndNode == currentPath[^1])
            {
                return;
            }
        }
        currentPath = TilemapManager.Instance.FindPath(transform.position, destination);
        currentNodeIndex = 0;
    }

    public void Stop()
    {
        currentPath.Clear();
        currentNodeIndex = 0;
    }

    private bool isPathValid()
    {
        return currentPath.Count > 0 && currentNodeIndex < currentPath.Count;
    }
}
