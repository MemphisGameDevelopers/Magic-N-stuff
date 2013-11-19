using UnityEngine;
using System.Collections;

public class TestMagicController : MonoBehaviour
{
    private Animator anim;
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKeyDown("q"))
	    {
            anim.SetTrigger("Circle");
	    }

        if (Input.GetKeyDown("w"))
        {
            anim.SetTrigger("Forward");
        }

        if (Input.GetKeyDown("e"))
        {
            anim.SetTrigger("Ground");
        }

        if (Input.GetKeyDown("r"))
        {
            anim.SetTrigger("Upwards");
        }
  

	}
}
