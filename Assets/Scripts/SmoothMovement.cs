using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    [SerializeField]
    LayerMask lmWalls;
    [SerializeField]
    float fJumpVelocity = 5;

    Rigidbody2D rigid;

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;

    [SerializeField]
    float fHorizontalAcceleration = 1;
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

    bool isConnected = true;
    bool isInRange = true;
    public Chain chain;
    public GameObject massPrefab;
    GameObject mass;

    void Start ()
    {
        rigid = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
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

        Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
        Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
        bool bGrounded = Physics2D.OverlapBox(v2GroundedBoxCheckPosition, v2GroundedBoxCheckScale, 0, lmWalls);

        fGroundedRemember -= Time.deltaTime;
        if (bGrounded)
        {
            fGroundedRemember = fGroundedRememberTime;
        }

        fJumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
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
    }
}
