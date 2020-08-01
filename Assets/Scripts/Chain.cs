using UnityEngine;

public class Chain : MonoBehaviour
{
    public GameObject[] links;
    public GameObject player;
    public int lastLink;

    private void Start()
    {
        lastLink = links.Length - 1;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (lastLink == 0)
                return;
            links[lastLink - 1].GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            player.transform.SetParent(links[lastLink - 1].transform);
            player.transform.position = new Vector2(links[lastLink -1].transform.position.x, links[lastLink - 1].transform.position.y - 0.32f);
            Destroy(links[lastLink]);
            lastLink--;
        }
    }
}
