using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] Vector3 startV3;
    [SerializeField] Vector3 endV3;

    [SerializeField] GameObject[] obstaclesPF; // Obstacle Prefabs;
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>(); // Obstacles on the board
    [SerializeField] List<Obstacle> obstaclesToRemove = new List<Obstacle>(); // Obstacles on the board
    [SerializeField] float moveSpeed = 0.5f;
    [Header ("in Seconds")]
    [SerializeField] float creationFrequencyTime = 1;
    [SerializeField] bool createObstacles = false;

    // Start is called before the first frame update
    void Start() 
    {
        createObstacles = false;
    }

    public void Begin()
    {
        createObstacles = true;
        StartCoroutine("CreateObstacle");
    }

    public void End()
    {
        createObstacles = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveObstacles();
        ClearObstacles();
    }

    IEnumerator CreateObstacle()
    {
        do
        {
            int randomIdx = Random.Range(0, obstaclesPF.Length);
            GameObject go = Instantiate(obstaclesPF[randomIdx]);
            go.transform.position = startV3;
            obstacles.Add(go.GetComponent<Obstacle>());
            yield return new WaitForSeconds(creationFrequencyTime);
        } while (createObstacles);
    }

    void MoveObstacles()
    {
        if (createObstacles)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].Move(moveSpeed, Time.deltaTime);

                if (obstacles[i].transform.position.z < endV3.z)
                {
                    obstaclesToRemove.Add(obstacles[i]);
                }
            }
        }
    }

    void ClearObstacles()
    {
        for (int i=0; i<obstaclesToRemove.Count; i++)
        {
            Obstacle ob = obstaclesToRemove[i];
            obstaclesToRemove.Remove(ob);
            obstacles.Remove(ob);
            Destroy(ob);
        }
    }
}