//#define RVOImp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if RVOImp
using RVO;
#endif
using Pathfinding;
using Pathfinding.RVO;

public class RVOExampleAgent : MonoBehaviour {
	
	public float repathRate = 1;
	
	private float nextRepath = 0;
	
#if RVOImp
	private int agentID;
#endif
	
	private Vector3 target;
	private bool canSearchAgain = true;
	
	private RVOController controller;
	
	Path path = null;
	
	List<Vector3> vectorPath;
	int wp;
	
#if RVOImp
	public bool astarRVO = true;
#endif
	
	public float moveNextDist = 1;
	Seeker seeker;
	
	MeshRenderer[] rends;
	
	//IAgent rvoAgent;
#if RVOImp	
	public Vector3 position {
		get {

			if (astarRVO) return rvoAgent.InterpolatedPosition;
			else return RVO.Simulator.Instance.getAgentPosition3(agentID);
//#else
			return rvoAgent.InterpolatedPosition;

		}
	}
#endif
	
	public void Awake () {
		seeker = GetComponent<Seeker> ();
	}
	
	// Use this for initialization
	public void Start () {
#if RVOImp
		if (!astarRVO) {
//#if !AstarRelease
			agentID = RVO.Simulator.Instance.addAgent (new RVO.Vector2(transform.position.x,transform.position.z));
//#endif
		} else {
			Pathfinding.RVO.Simulator sim = (FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator).GetSimulator ();
			rvoAgent = sim.AddAgent (transform.position);
			rvoAgent.Radius = radius;
			rvoAgent.MaxSpeed = maxSpeed;
			rvoAgent.Height = height;
			rvoAgent.AgentTimeHorizon = agentTimeHorizon;
			rvoAgent.ObstacleTimeHorizon = obstacleTimeHorizon;
		}
		
		SetTarget (-transform.position);// + transform.forward * 400);
#endif
		controller = GetComponent<RVOController> ();
		
	}
	
	public void SetTarget (Vector3 target) {
		this.target = target;
		RecalculatePath ();
	}
	
	public void SetColor (Color col) {
		if (rends == null) rends = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer rend in rends) {
			Color current = rend.material.GetColor("_TintColor");
			AnimationCurve curveR = AnimationCurve.Linear (0,current.r,1,col.r);
			AnimationCurve curveG = AnimationCurve.Linear (0,current.g,1,col.g);
			AnimationCurve curveB = AnimationCurve.Linear (0,current.b,1,col.b);
			
			AnimationClip clip = new AnimationClip ();
			clip.SetCurve ("",typeof(Material),"_TintColor.r",curveR);
			clip.SetCurve ("",typeof(Material),"_TintColor.g",curveG);
			clip.SetCurve ("",typeof(Material),"_TintColor.b",curveB);
			
			Animation anim = rend.gameObject.GetComponent<Animation>();
			if (anim == null) {
				anim = rend.gameObject.AddComponent<Animation>();
			}
			clip.wrapMode = WrapMode.Once;
			anim.AddClip (clip,"ColorAnim");
			anim.Play ("ColorAnim");
		}
	}
	
	public void RecalculatePath () {
		canSearchAgain = false;
		nextRepath = Time.time+repathRate*(Random.value+0.5f);
		seeker.StartPath (transform.position,target,OnPathComplete);
	}
	
	public void OnPathComplete (Path _p) {
		ABPath p = _p as ABPath;
		
		canSearchAgain = true;
		
		if (path != null) path.Release (this);
		path = p;
		p.Claim (this);
		
		if (p.error) {
			wp = 0;
			vectorPath = null;
			return;
		}
		
		
		Vector3 p1 = p.originalStartPoint;
		Vector3 p2 = transform.position;
		p1.y = p2.y;
		float d = (p2-p1).magnitude;
		wp = 0;
		
		vectorPath = p.vectorPath;
		Vector3 waypoint;
		
		for (float t=0;t<=d;t+=moveNextDist*0.6f) {
			wp--;
			Vector3 pos = p1 + (p2-p1)*t;
			
			do {
				wp++;
				waypoint = vectorPath[wp];
				waypoint.y = pos.y;
			} while ((pos - waypoint).sqrMagnitude < moveNextDist*moveNextDist && wp != vectorPath.Count-1);
			
		}
	}
	
	public void Update () {
		
		if (Time.time >= nextRepath && canSearchAgain) {
			RecalculatePath ();
		}
		
		Vector3 velocity = controller.velocity;
		
		if (velocity != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (velocity);
		
		Vector3 dir = Vector3.zero;
		
		Vector3 pos = transform.position;
		
		if (vectorPath != null && vectorPath.Count != 0) {
			Vector3 waypoint = vectorPath[wp];
			waypoint.y = pos.y;
			
			while ((pos - waypoint).sqrMagnitude < moveNextDist*moveNextDist && wp != vectorPath.Count-1) {
				wp++;
				waypoint = vectorPath[wp];
				waypoint.y = pos.y;
			}
			
			dir = waypoint - pos;
			float magn = dir.magnitude;
			if (magn > 0) {
				float newmagn = Mathf.Min (magn, controller.maxSpeed);
				dir *= newmagn / magn;
			}
			//dir = Vector3.ClampMagnitude (waypoint - pos, 1.0f) * maxSpeed;
		}
		
		controller.Move (dir);
		
		/*Vector3 pos = position;
		
		RaycastHit hit;
#if !AstarRelease
		float height = astarRVO ? rvoAgent.Height : RVO.Simulator.Instance.getAgentHeight	(agentID);
#else
		float height = rvoAgent.Height;
#endif
		
		Vector3 realPos = rvoAgent.Position;
		
		if (Physics.Raycast	(realPos + Vector3.up*height,Vector3.down, out hit, float.PositiveInfinity, mask)) {
			realPos.y = hit.point.y;
		} else {
			realPos.y = 0;
		}
		
#if !AstarRelease
		if (!astarRVO)
			RVO.Simulator.Instance.setAgentYPosition(agentID,pos.y);
		else
#endif
			rvoAgent.Position = realPos;
		
		Vector3 velocity;
		
#if !AstarRelease
		if (!astarRVO) {
			RVO.Vector2 _vel = RVO.Simulator.Instance.getAgentVelocity (agentID);
			velocity = new Vector3(-_vel.x(),0,-_vel.y());
		} else
#endif
			velocity = rvoAgent.Velocity;
		
		transform.position = pos;
		
		
		
		//Debug.DrawRay (pos,dir*10,Color.blue);
		
		if (astarRVO) {
			List<ObstacleVertex> obst = rvoAgent.NeighbourObstacles;
			
			Vector3 force = Vector3.zero;
			
			for (int i=0;i<obst.Count;i++) {
				Vector3 a = obst[i].position;
				Vector3 b = obst[i].next.position;
				
				Vector3 closest = position - Mathfx.NearestPointStrict (a,b,position);
				
				if (closest == a || closest == b) continue;
				
				float dist = closest.sqrMagnitude;
				closest /= dist*falloff;
				force += closest;
			}
			Debug.DrawRay (position, dir + force*wallAvoidForce);
			
			rvoAgent.DesiredVelocity = dir + force*wallAvoidForce; 
		} else {
#if !AstarRelease
			RVO.Simulator.Instance.setAgentPrefVelocity (agentID, new RVO.Vector2 (dir.x,dir.z));
#endif
		}*/
	}
}
