using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject player;
    public GameObject rope;

    private void Start()
    {
        rope = GameObject.FindGameObjectWithTag("Rope");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(1, rope.transform.position);
        lineRenderer.SetPosition(0, player.transform.position);
    }
}
