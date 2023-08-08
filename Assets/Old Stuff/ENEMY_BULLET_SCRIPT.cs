using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY_BULLET_SCRIPT : MonoBehaviour
{
    public float EXPLOSION_START_XPOSITION;
    public float EXPLOSION_START_YPOSITION;
    public float EXPLOSION_END_XPOSITION;
    //public float EXPLOSION_END_YPOSITION;
    public float SPEED;
    public float START_LERP;

    void Start()
    {
        
    }

    void Update()
    {
        if (START_LERP > 0)
        {
            transform.position = new Vector3(Mathf.Lerp(EXPLOSION_END_XPOSITION, EXPLOSION_START_XPOSITION, START_LERP), Mathf.Lerp(EXPLOSION_START_YPOSITION + 5f, EXPLOSION_START_YPOSITION, START_LERP));
            START_LERP -= Time.deltaTime * 3f;
        }
        else
            transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * SPEED);
    }

    

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Destroy(gameObject);
    //}
}
