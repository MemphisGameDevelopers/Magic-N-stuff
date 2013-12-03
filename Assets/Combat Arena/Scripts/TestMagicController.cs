using UnityEngine;
using System.Collections;

public class TestMagicController : MonoBehaviour
{
    private Animator anim;
    public float speed;
    public float rot;
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
        speed = 0.0f;
        rot = 0.0f;
	}
	
	// Update is called once per frame
	void Update()
    {
        speed = Input.GetAxis("Vertical");
        rot = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", speed);

        transform.position += transform.forward * speed * Time.deltaTime;
        transform.Rotate(Vector3.up * rot );

	    if (Input.GetKeyDown("e"))
	    {
            anim.SetTrigger("Circle");
	    }

        if (Input.GetKeyDown("q"))
        {
            anim.SetTrigger("Forward");
        }

        if (Input.GetKeyDown("r"))
        {
            anim.SetTrigger("Ground");
        }

        if (Input.GetKeyDown("t"))
        {
            anim.SetTrigger("Upwards");
        }

        
  

	}
}
