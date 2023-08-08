using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLAYER_SCRIPT : MonoBehaviour
{
    public ParticleSystem _RING_OF_FIRE;
    public GameObject _BODY;
    public Vector3[] _BODY_SIZES;
    public float ASTEROIDS_EATEN;
    public float ASTEROIDS_NEEDED_FOR_EXPLOSION;

    public float BODY_LERP_POSITION;
    public Camera _CAMERA;

    public float[] SPEED_RANGE;
    public Vector3 VELOCITY_CURRENT;
   
    public Vector3 DESTINATION_POSITION;
    
    public GameObject JOYSTICK_CENTER;
    public GameObject JOYSTICK_END;
    public bool JOYSTICK_ACTIVATED;
    public Vector2 TOUCHPOINT;
    public float JOYSTICK_END_RANGE;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.touchCount > 0)
        //{
        //    Debug.Log("TOUCHED");
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        JOYSTICK_ACTIVATED = true;
        //        TOUCHPOINT = _CAMERA.ScreenToWorldPoint(Input.GetTouch(0).position);

        //        Vector2 _touchpoint = TOUCHPOINT;
        //        JOYSTICK_CENTER.transform.position = _touchpoint;
        //    }
        //    else if (Input.touchCount > 0)
        //    {
        //        DESTINATION_POSITION = _CAMERA.ScreenToWorldPoint(Input.GetTouch(0).position);

        //        Vector2 _destinationpoint = DESTINATION_POSITION;

        //        if (Vector2.Distance(JOYSTICK_CENTER.transform.position, _destinationpoint) > JOYSTICK_END_RANGE)
        //        {
        //            Vector2 _joystick_center_position = JOYSTICK_CENTER.transform.position;
        //            _destinationpoint = _joystick_center_position + (-_destinationpoint + _joystick_center_position).normalized * JOYSTICK_END_RANGE;
        //        }
        //        JOYSTICK_END.transform.position = _destinationpoint;
        //    }
        //    else
        //        DESTINATION_POSITION = _CAMERA.ScreenToWorldPoint(Input.mousePosition);
        //}
        //else if (Input.touchCount <= 0)
        //{
        //    JOYSTICK_CENTER.GetComponent<SpriteRenderer>().enabled = true;
        //    JOYSTICK_END.GetComponent<SpriteRenderer>().enabled = true;
        //    JOYSTICK_ACTIVATED = false;
        //}

        //if (JOYSTICK_ACTIVATED)
        //{
        //    JOYSTICK_CENTER.GetComponent<SpriteRenderer>().enabled = false;
        //    JOYSTICK_END.GetComponent<SpriteRenderer>().enabled = false;
        //    VELOCITY_CURRENT = (-JOYSTICK_CENTER.transform.position + JOYSTICK_END.transform.position) * SPEED_RANGE[0];

        //    if (transform.position.x > 9 && VELOCITY_CURRENT.x > 0 || transform.position.x < -9 && VELOCITY_CURRENT.x < 0)
        //        VELOCITY_CURRENT.x = 0;
        //    if (transform.position.y > 18 && VELOCITY_CURRENT.y > 0 || transform.position.y < -18 && VELOCITY_CURRENT.y < 0)
        //        VELOCITY_CURRENT.y = 0;

        //    transform.Translate(VELOCITY_CURRENT * Time.deltaTime);
        //}

        Vector2 joystickInput = GetJoystickInput();
        Vector2 velocity = joystickInput * SPEED_RANGE[0];
        transform.Translate(velocity * Time.deltaTime);


        _BODY.transform.position = Vector3.Lerp(_BODY.transform.position, transform.position, BODY_LERP_POSITION * Time.deltaTime);

        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, _BODY.transform.position);
    }

    Vector2 joystickStartPoint;
    public float joystickMaxRadius = 10f;

    Vector2 GetJoystickInput()
    {
        if (Input.touchCount == 0)
            return Vector2.zero;

        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                {
                    joystickStartPoint = Input.mousePosition;
                    break;
                }

            case TouchPhase.Moved:
                {
                    Vector2 mousePosition = Input.mousePosition;
                    Vector2 motion = Vector2.ClampMagnitude((mousePosition - joystickStartPoint) / joystickMaxRadius, 1f);
                    return motion;
                }

            default: break;
        }

        return Vector2.zero;
    }

    public void ROCK_EATEN()
    {
        _BODY.transform.localScale = Vector3.Lerp(_BODY_SIZES[0], _BODY_SIZES[1], ASTEROIDS_EATEN / ASTEROIDS_NEEDED_FOR_EXPLOSION);
        ASTEROIDS_EATEN++;
        if(ASTEROIDS_EATEN == ASTEROIDS_NEEDED_FOR_EXPLOSION)
        {
            _RING_OF_FIRE.transform.position = transform.position;
            _RING_OF_FIRE.Play();
            ASTEROIDS_EATEN = 0;
            //EVENTS.EXPLOSION_HAPPENED();

            GameObject _big_rock_candidate = null;
            foreach(GameObject _BR in GameObject.FindGameObjectsWithTag("BIG_ROCK"))
            {
                if (_big_rock_candidate == null)
                {
                    _big_rock_candidate = _BR;
                }
                else if (_BR.transform.position.y < _big_rock_candidate.transform.position.y)
                    _big_rock_candidate = _BR;
            }

            _big_rock_candidate.GetComponent<ENEMY_GIANT_ROCK>().DESTROYED();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            ROCK_EATEN();
            Destroy(collision.gameObject);
        }
    }
}
