using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BossBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<Transform> waypoints = new List<Transform>();
    

    [Header("BossStats")]
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public float currentHealth;
    public float patrolPointStopDistance = 0.5f;

    [Header("Attack Settings")]
    public int attackDamage = 20;
    public float attackRange = 5f;
    public float attackRate = 1f;

    [Header("Decision Range")]
    public float detectionRange = 10f;
    public float attackThreshold = 15f;
    public float lowHealthThreshold = 30f;

    [Header("State Debug")]
    public string currentDecision;               
    public Color gizmoColor = Color.white;

    private DecisionNode rootNode;               
    private int patrolIndex;                     
    private float attackTimer;                   
    private float reloadTimer;                   
    private bool isReloading; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        currentHealth = maxHealth;
        BuildDecisionTree();
    }

    // Update is called once per frame
    void Update()
    {
        string decision = EvaluateDecisionTree(rootNode);
        currentDecision = decision;

        switch (decision)
        {
            case "Attack":
                gizmoColor = Color.red;
                AttackPlayer();
                break;
            case "Chase":
                gizmoColor = Color.yellow;
                ChasePlayer();
                break;
            case "Flee":
                gizmoColor = Color.blue;
                Flee();
                break;
            case "Patrol":
                gizmoColor = Color.green;
                Patrol();
                break;
            default:
                gizmoColor = Color.white;
                break;
        }
    }
    private void BuildDecisionTree()
    {
        // Create decision nodes
        ActionNode attackNode = new ActionNode("Attack");
        ActionNode patrolNode = new ActionNode("Patrol");
        ActionNode chaseNode = new ActionNode("Chase");
        ActionNode fleeNode = new ActionNode("Flee");

        QuestionNode playerInAttackRangeNode = new QuestionNode(
            "Is player in attack range?",
            () => Vector3.Distance(transform.position, player.position) <= attackRange);

        QuestionNode lowHealthNode = new QuestionNode(
            "Is health low?",
             () => currentHealth < lowHealthThreshold);

        QuestionNode playerDetectedNode = new QuestionNode(
            "Is player detected?",
             () => Vector3.Distance(transform.position, player.position) <= detectionRange);
             
             
        playerInAttackRangeNode.falseNode = patrolNode;
        playerInAttackRangeNode.trueNode = lowHealthNode;

        lowHealthNode.trueNode = fleeNode;
        lowHealthNode.falseNode = playerInAttackRangeNode;

        playerInAttackRangeNode.falseNode = chaseNode;
        playerInAttackRangeNode.trueNode = attackNode;


        // Set the first question as the root of the tree
        rootNode = playerInAttackRangeNode;
        

    }
    private void Patrol()
    {
        // Stop if there are no patrol points assigned
        if (waypoints == null || waypoints.Count == 0)
            return;

        // Get the current patrol target
        Transform target = waypoints[patrolIndex];

        // Keep target on the same Y level as the enemy
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

        // Check distance to the patrol point
        float distance = Vector3.Distance(transform.position, targetPosition);

        // If close enough, switch to the next patrol point
        if (distance <= patrolPointStopDistance)
        {
            patrolIndex = (patrolIndex + 1) % waypoints.Count;
            target = waypoints[patrolIndex];
            targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        }

        // Move to the patrol point
        MoveTowards(targetPosition, moveSpeed);
    }
    private string EvaluateDecisionTree(DecisionNode currentNode)
    {
        while (currentNode != null)
        {
            // If we reached an action node, return that action
            if (currentNode is ActionNode actionNode)
                return actionNode.actionName;

            // If this is a question node, evaluate its condition
            if (currentNode is QuestionNode questionNode)
            {
                bool result = questionNode.condition.Invoke();

                // Move to the true branch or false branch depending on result
                currentNode = result ? questionNode.trueNode : questionNode.falseNode;
            }
        }

        // Safety fallback
        return "None";
    }
    private void AttackPlayer()
    {
        // Face the player while attacking
        FaceTarget(player.position);

        // Handle attack cooldown
        if (attackTimer <= 0f)
        {
            // Deal damage to player
            PlayerRespawn playerHealth = player.GetComponent<PlayerRespawn>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // Reset attack timer
            attackTimer = 1f / attackRate;
        }
        else
        {
            // Countdown the attack timer
            attackTimer -= Time.deltaTime;
        }
    }
    private void ChasePlayer()
    {
        MoveTowards(player.position, moveSpeed);
    }
    private void Flee()
    {

        Vector3 awayDirection = (transform.position - player.position).normalized;

        Vector3 fleeTarget = transform.position + awayDirection * 3f;

        MoveTowards(fleeTarget, moveSpeed);
    }
    private void MoveTowards(Vector3 targetPosition, float speed)
    {
        // Move step by step toward the target
        Vector3 nextPosition = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Work out the direction the enemy should face
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f;

        // Only rotate if the direction is large enough
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.forward = direction.normalized;
        }

        // Apply the new position
        transform.position = nextPosition;
    }
    private void FaceTarget(Vector3 targetPosition)
    {
        // Flatten the target so the enemy only rotates on the Y axis
        Vector3 flatTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 direction = (flatTarget - transform.position).normalized;

        // Rotate only if the direction is valid
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.forward = direction;
        }
    }
    private float DistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        Vector3 a = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 b = new Vector3(player.position.x, 0f, player.position.z);

        return Vector3.Distance(a, b);
    }
    private void OnDrawGizmos()
    {
        // Blue circle = detection range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Red circle = attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Small sphere above enemy = current state color
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1.5f, 0.25f);
    }
}

public class QuestionNode : DecisionNode
{
    public string question;
    public Func<bool> condition;
    public DecisionNode trueNode;
    public DecisionNode falseNode;

    public QuestionNode(string questionText, Func<bool> condition)
    {
        this.question = questionText;
        this.condition = condition;
    }
}


public class ActionNode : DecisionNode
{
    public string actionName;

    public ActionNode(string actionName)
    {
        this.actionName = actionName;
        
    }
}

public abstract class DecisionNode
{
}