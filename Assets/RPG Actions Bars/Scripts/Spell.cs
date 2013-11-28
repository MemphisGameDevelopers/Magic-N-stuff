using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour {
	
	public Texture2D icon;
	public string spellName;
	public float cooldown;
	[HideInInspector]
	public float cooldownTimer;
	[HideInInspector]
	public bool onCooldown;

    public GameObject spellEffect;
    private Transform player;
    private Animator anim;
	
	// Use this for initialization
	void Start ()
	{
	    player = GameObject.FindGameObjectWithTag("Player").transform;
	    anim = player.GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		if(cooldownTimer < cooldown)
			cooldownTimer += Time.deltaTime;
		if(cooldownTimer >= cooldown)
			onCooldown = false;
	}
	
	public void Use() {
		//Here you can implement your cool spells like a big tornado or a lightning hitting enemys.. It's all up to you. :)
        anim.SetTrigger("Forward");
        Instantiate(spellEffect, player.position + new Vector3(0, 1), Quaternion.LookRotation(player.forward));
	    cooldownTimer = 0;
		onCooldown = true;
	}
}
