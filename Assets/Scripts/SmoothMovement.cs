using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    [SerializeField]
    LayerMask lmWalls;
    [SerializeField]
    float fJumpVelocity = 5;

    Animator animator;
    bool facingRight = false;
    Rigidbody2D rigid;

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;

    public bool isConnected = true;
    private bool isInRange = true;
    public Chain chain;
    public GameObject massPrefab;
    GameObject mass;

    bool bGrounded;

    void Start ()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
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
                    chain.links[chain.lastLink].GetComponent<HingeJoint2D>().connectedBody = rigid;
                    isConnected = true;
                    Destroy(mass);
                }
            }
        }
        
        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.9f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded)
        {
            fGroundedRemember = fGroundedRememberTime;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
        }

        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
            if (rigid.velocity.y > 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * fCutJumpHeight);
            }
        }

        if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
        {
            fJumpPressedRemember = 0;
            fGroundedRemember = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, fJumpVelocity);
            animator.SetBool("IsJumping", true);
        }

        float fHorizontalVelocity = rigid.velocity.x;
        fHorizontalVelocity += Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
        else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
        else
            fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

        rigid.velocity = new Vector2(fHorizontalVelocity, rigid.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));

        if(!facingRight && Input.GetAxisRaw("Horizontal") > 0)
        {
            Flip();
        }
        else if(facingRight && Input.GetAxisRaw("Horizontal") < 0)
        {
            Flip();
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
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

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
