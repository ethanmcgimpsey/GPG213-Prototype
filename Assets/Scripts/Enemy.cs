using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum AIState
    {
        Walking,
        Die
    }
    public AIState state;
    public float curHealth, maxHealth, moveSpeed;
    public int curWaypoint, difficulty;
    public float minDistance = 2f;
    public bool isDead, dead;

    [Space(5), Header("Base References")]
    public GameObject self;
    public Transform waypointParent;
    protected Transform[] waypoints;
    public NavMeshAgent agent;
    public GameObject healthCanvas;
    public Image healthBar;
    public Animator anim;
    public Transform targetPoint;

    private void OnDrawGizmos()
    {
        // If waypoints isn't assigned
        if (waypoints == null)
            return;

        // If there are waypoints in the array
        if(waypoints.Length > 0)
        {
            // Get current waypoint
            Transform waypoint = waypoints[curWaypoint];
            // If waypoint exists
            if (waypoint)
            {
                // Draw it!
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoint.position, minDistance);
            }
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position , .5f);
        Gizmos.DrawSphere(agent.destination, .5f);

    }
    public void Start()
    {
        waypoints = waypointParent.GetComponentsInChildren<Transform>();
        curWaypoint = 1;
        agent.speed = moveSpeed;
        anim = self.GetComponent<Animator>();
        Walking();
        anim.SetBool("Walking", false);
        anim.SetBool("Die", false);

    }
    private void Update()
    {
        anim.SetBool("Walking", false);
        anim.SetBool("Die", false);

        Walking();
        Die();
    }
    void LateUpdate()
    {
        if (healthBar.fillAmount < 1 && healthBar.fillAmount > 0)
        {
            healthCanvas.SetActive(true);
            healthCanvas.transform.LookAt(Camera.main.transform);
            healthCanvas.transform.Rotate(0, 180, 0);
        }
        else if (healthCanvas.activeSelf == true)
        {
            healthCanvas.SetActive(false);
        }
    }

    public void Walking()
    {
        if (!isDead)
        {
            // DO NOT CONTINUE IF NO WAYPOINTS
            if (waypoints.Length == 0)
            {
                return;
            }
            anim.SetBool("Walking", true);
            // Follow waypoints
            // Set agent to target
            agent.destination = waypoints[curWaypoint].position;
            // Are we at the waypoint?
            float distance = Vector3.Distance(transform.position, agent.destination);
            if (distance < minDistance)
            {
                if (curWaypoint < waypoints.Length - 1)
                {
                    // If so go to next waypoint
                    curWaypoint++;
                    curWaypoint = Random.Range(2, 4);
                }
                else
                {
                    // If at the end of patrol go to start
                    // curWaypoint = 3;
                    Destroy(gameObject);
                    PlayerStats.Lives -= 3;
                }
                // If so go to next waypoint
            }

        }
    }

    public void Die()
    {
        // If we are alive
        if (curHealth > 0)
        {
            // Don't run this
            return;
        }
        // else we are dead so run this
        if (isDead)
        {
            state = AIState.Die;
            if (!dead)
            {
                anim.SetTrigger("Die");
                dead = true;
                PlayerStats.Money += 14;
                PlayerStats.Score += 7;
            }
            agent.destination = self.transform.position;
            // Drop Loot
        }
    }
}