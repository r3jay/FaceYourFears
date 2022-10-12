using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightSwitch : MonoBehaviour
{

    [SerializeField] GameObject light;
    [SerializeField] GameObject player;
    [SerializeField] float spottingRange;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        changeLight();
    }

    void changeLight()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < spottingRange) 
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }
}
