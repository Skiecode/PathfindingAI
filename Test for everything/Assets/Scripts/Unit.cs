using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public class Unit : MonoBehaviour
{

    public Grid grid;
    public Transform target;
    public int layerIndex;
    public float speed;

    Vector3[] path;
    List<Transform> unwalkable = new List<Transform>();
    int targetIndex;
    bool isWalking = false;
    public bool hasWayChanged = true;

    /// <summary>
    /// Die FindObjectsOnLayer scheint nicht zu funken
    /// </summary>

    void Awake()
    {
        unwalkable = FindObjectsOnLayer(layerIndex);
        for (int i = 0; i < unwalkable.Count; i++)
        {
            unwalkable[i].hasChanged = false;
        }
        StartCoroutine(CheckIfObjectsHaveChanged());
    }

    IEnumerator CheckIfObjectsHaveChanged()
    {
        while (true)
        {
            print("Checking if objects have changed...");
            for (int i = 0; i < unwalkable.Count; i++)
            {
                if (unwalkable[i].hasChanged)
                {
                    print("Object " + unwalkable[i].name + " have changed");
                    unwalkable[i].hasChanged = false;
                    hasWayChanged = true;
                    grid.SetupGrid();
                    break;
                }
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    List<Transform> FindObjectsOnLayer(int layer)
    {
        GameObject[] goArray = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        List<Transform> goList = new List<Transform>();
        foreach (GameObject go in goArray)
        {
            if (go.layer == layer)
            {
                goList.Add(go.transform);
            }
        }
        return goList;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            print("Request new path");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        isWalking = true;
        while (true)
        {
            if (!hasWayChanged)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                    this.transform.LookAt(path[targetIndex]);
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
            else
            {
                print("Requesting new path " + gameObject.name);
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                hasWayChanged = false;
                break;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
