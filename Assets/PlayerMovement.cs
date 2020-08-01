using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float jumpForce = 50f;
    bool isConnected = true;
    bool isInRange = true;
    public Chain chain;
    public GameObject massPrefab;
    GameObject mass;

    bool isGrounded = false;
    public Transform groundCheck;
    public float checkRadius = 2f;
    public LayerMask wharIsGround;

    public int extraJumpsValue;
    int extraJumps;


    float horizontal = 0f;

    private void Start()
    {
        extraJumps = extraJumpsValue;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(isGrounded)
            {
                extraJumps = extraJumpsValue;
            }
            if (isConnected)
            {
                transform.SetParent(null);
                mass = Instantiate(massPrefab, chain.links[chain.lastLink].transform.position, Quaternion.identity);
                mass.transform.SetParent(chain.links[chain.lastLink].transform);
                chain.links[chain.lastLink].GetComponent<HingeJoint2D>().connectedBody = mass.GetComponent<Rigidbody2D>();
                isConnected = false;
            }
            else
            {
                if (isInRange)
                {
                    transform.SetParent(chain.links[chain.lastLink].transform);
                    transform.position = new Vector2(chain.links[chain.lastLink].transform.position.x, chain.links[chain.lastLink].transform.position.y - 0.32f);
                    chain.links[chain.lastLink].GetComponent<HingeJoint2D>().connectedBody = rb;
                    isConnected = true;
                    Destroy(mass);
                }
            }
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        if(!isConnected)
        {
            if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
                extraJumps--;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && extraJumps <= 0 && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, wharIsGround);
        rb.velocity = new Vector2(horizontal * speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Link"))
            isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Link"))
            isInRange = false;
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }
}
