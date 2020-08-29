using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    enum Lane
    {
        middle,
        left,
        right
    }

    [SerializeField] Material[] materials;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float laneShift = 5.5f;


    [SerializeField] float moved = 0f;
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] int currentMaterial = 1;
    [SerializeField] Animator animator = null;
    [SerializeField] Lane currentLane = Lane.middle;
    [SerializeField] bool isMoving = false;
    [SerializeField] float moveStartX = 0;
    [SerializeField] float moveEndX = 0;
    [SerializeField] ParticleSystem deathEffect = null;
    ParticleSystemRenderer deathEffectRenderer = null;
    MeshRenderer meshRenderer = null;
    bool gameStopped = true;
    [SerializeField] GameSceneController gameSceneController;



    // Start is called before the first frame update
    void Start()
    {
        gameSceneController = FindObjectOfType<GameSceneController>();
        meshRenderer = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
        deathEffectRenderer = deathEffect.GetComponent<ParticleSystemRenderer>();
        animator.enabled = false;
        UpdateMaterial();

        //Disable Input
        isMoving = true;
        gameStopped = true;

    }

    public void Begin()
    {
        // Enable Input
        gameStopped = false;
    }

    public void End()
    {
        //Disable Input
        gameStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        Move();
    }

    private void HandleInput()
    {
        if (isMoving || gameStopped) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GoRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GoLeft();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Rollback();
        }
    }

    void ChangeMaterial()
    {
        //Debug.Log("change color");
        currentMaterial = (currentMaterial + 1) % materials.Length;
    }

    void GoRight()
    {
        switch(currentLane)
        {
            case Lane.right:
                return;

            case Lane.left:
                //Debug.Log("move right");
                currentLane = Lane.middle; 
                break;

            case Lane.middle:
                //Debug.Log("move right");
                currentLane = Lane.right; 
                break;
        }
        moveStartX = transform.position.x;
        moveEndX = (transform.position + laneShift * Vector3.right).x;
        Debug.Log((transform.position + laneShift * Vector3.right).ToString());
        moveDirection = Vector3.right;
        isMoving = true;
    }

    void GoLeft()
    {
        switch (currentLane)
        {
            case Lane.left: 
                return;

            case Lane.right:
                Debug.Log("move left");
                currentLane = Lane.middle; 
                break;

            case Lane.middle:
                Debug.Log("move left");
                currentLane = Lane.left; 
                break;

        }
        Debug.Log((transform.position + laneShift * Vector3.left).ToString());
        moveEndX = (transform.position + laneShift * Vector3.left).x;
        moveStartX = transform.position.x;
        moveDirection = Vector3.left;
        isMoving = true;
    }

    void Rollback()
    {
        Debug.Log("roll back + change material");
        ChangeMaterial();
        animator.enabled = true;
        animator.SetTrigger("rollback");
    }

    void UpdateMaterial()
    {
        meshRenderer.material = materials[currentMaterial];
    }

    void RollbackCompleted()
    {
        Debug.Log("rollback completed");
        animator.enabled = false;
    }

    void Move()
    {
        if (isMoving)
        {
            float newX = Mathf.Lerp(moveStartX, moveEndX, moved);
            moved += Time.deltaTime * moveSpeed;
            Vector3 position = transform.position;
            position.x = newX;
            transform.position = position;

            if (Mathf.Approximately(Mathf.Abs(newX), Mathf.Abs(moveEndX)))
            {
                isMoving = false;
                moved = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "block") 
        {
            Material blockMaterial = other.gameObject.GetComponent<MeshRenderer>().material;
            if (meshRenderer.material.color.ToString() == blockMaterial.color.ToString())
            {
                deathEffect.transform.position = other.gameObject.transform.position;
                deathEffect.Play();
                deathEffectRenderer.material = blockMaterial;
                Destroy(other.gameObject);
                gameSceneController.IncrementScore();
            }
            else
            {
                deathEffect.transform.position = gameObject.transform.position;
                deathEffect.Play();
                deathEffectRenderer.material = blockMaterial;
                gameObject.SetActive(false);
                gameSceneController.StopGame();
            }
        }
    }
}
