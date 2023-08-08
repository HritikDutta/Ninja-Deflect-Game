using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMY_SPAWNER_SCRIPT : MonoBehaviour
{
    [System.Serializable]
    public class ENEMY_TO_SPAWN
    {
        public GameObject _PREFAB;
        public float SPAWN_INTERVAL;
        public float SPAWN_TIMER;
        public GameObject[] SPAWNPOINTS;
        public int SPAWNPOINT_INDEX = 0;
        public bool _isRANDOM;
    }

    public ENEMY_TO_SPAWN[] SPAWNLIST;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        foreach (ENEMY_TO_SPAWN _ES in SPAWNLIST)
        {
            if (_ES.SPAWN_TIMER < 0)
            {
                _ES.SPAWN_TIMER = _ES.SPAWN_INTERVAL;
                Instantiate(_ES._PREFAB, _ES.SPAWNPOINTS[_ES.SPAWNPOINT_INDEX].transform.position, Quaternion.identity);

                if(!_ES._isRANDOM)
                    _ES.SPAWNPOINT_INDEX = (_ES.SPAWNPOINT_INDEX + 1) >= _ES.SPAWNPOINTS.Length ? 0 : _ES.SPAWNPOINT_INDEX + 1;
                else
                    _ES.SPAWNPOINT_INDEX = Random.Range(0,_ES.SPAWNPOINTS.Length-1);
            }
            else
                _ES.SPAWN_TIMER -= Time.deltaTime;
        }
    }
}
