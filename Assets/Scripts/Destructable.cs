using UnityEngine;
using UnityEngine.Tilemaps;

public class Destructable : MonoBehaviour
{
    Tilemap map;
    GameObject player;
    Vector3 hit;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        map = GetComponent<Tilemap>();
    }

    private void OnMouseDown()
    {
        hit = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector3.Distance(player.transform.position, hit) <= 10.2f)  
            map.SetTile(map.WorldToCell(hit), null);
    }
}
