using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY_GIANT_ROCK : MonoBehaviour
{
    private void OnEnable()
    {
        EVENTS.EXPLOSION_HAPPENED += DESTROYED;
    }

    private void OnDisable()
    {
        EVENTS.EXPLOSION_HAPPENED -= DESTROYED;
    }

    public float SPEED;

    public GameObject _PIECES;
    public float _PIECE_SPAWN_RADIUS;
    public int _PIECES_AMOUNT;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * SPEED);
        transform.GetChild(0).Rotate(SPEED * Time.deltaTime, SPEED * Time.deltaTime, SPEED * Time.deltaTime);
    }

    public void DESTROYED()
    {
        for(int i = _PIECES_AMOUNT; i > 0; i--)
        {
            GameObject _piece = Instantiate(_PIECES, transform.position + new Vector3(Random.Range(-_PIECE_SPAWN_RADIUS, _PIECE_SPAWN_RADIUS), Random.Range(-_PIECE_SPAWN_RADIUS, _PIECE_SPAWN_RADIUS)), Quaternion.identity);
            _piece.GetComponent<ENEMY_BULLET_SCRIPT>().START_LERP = 1;
            _piece.GetComponent<ENEMY_BULLET_SCRIPT>().EXPLOSION_START_YPOSITION = transform.position.y;
            _piece.GetComponent<ENEMY_BULLET_SCRIPT>().EXPLOSION_START_XPOSITION = transform.position.x;
            _piece.GetComponent<ENEMY_BULLET_SCRIPT>().EXPLOSION_END_XPOSITION = transform.position.x * ((Random.Range(-1.1f, 1.1f)));
        }
        Destroy(gameObject);
    }
}
