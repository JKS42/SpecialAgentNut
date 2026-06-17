using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
public class BossBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Animator animator;
    [SerializeField] private string animatorSpeedParameter = "currentSpeed";
    [SerializeField] private string attackTriggerParameter = "isAttacking";
    [SerializeField] private GameObject VictoryPanel;
    

    [Header("BossStats")]
    public float patrolSpeed = 4f;
    public float moveSpeed = 8f;
    public float rotationSpeed = 720f;
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

    [Header("Graph Patrol")]
    public bool useGraphRouteForPatrol = true;
    public GraphRoute graphRoute;
    public GraphNode graphPatrolStartNode;
    public GraphNode graphPatrolGoalNode;

    [Header("State Debug")]
    public string currentDecision;               
    public Color gizmoColor = Color.white;

    private DecisionNode rootNode;               
    private int patrolIndex;                     
    private float attackTimer;                   
    private float reloadTimer;                   
    private bool isReloading; 
    private List<GraphNode> graphPatrolPath = new List<GraphNode>();
    private int graphPathIndex;
    private NavMeshAgent navAgent;
    private Vector3 lastFramePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = patrolPointStopDistance;
            navAgent.autoTraverseOffMeshLink = true;
            navAgent.updateRotation = false;

            if (!navAgent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                navAgent.Warp(hit.position);
            }
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        currentHealth = maxHealth;
        lastFramePosition = transform.position;
        BuildDecisionTree();
        RefreshGraphPatrolPath();
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null || string.IsNullOrEmpty(parameterName))
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
                return true;
        }

        return false;
    }

    private void LateUpdate()
    {
        UpdateAnimatorSpeed();
        FaceNavAgentMovementDirection();
        lastFramePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlayerDetected())
        {
            ReturnToPatrolState();
            currentDecision = "Patrol";
            gizmoColor = Color.green;
            Patrol();
            return;
        }

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
    private bool IsPlayerDetected()
    {
        return player != null && DistanceToPlayer() <= detectionRange;
    }
    private void ReturnToPatrolState()
    {
        attackTimer = 0f;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = false;
            navAgent.stoppingDistance = patrolPointStopDistance;
            navAgent.ResetPath();
        }
    }
    private void UpdateAnimatorSpeed()
    {
        if (animator == null || string.IsNullOrEmpty(animatorSpeedParameter))
            return;

        float speed = 0f;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            speed = navAgent.velocity.magnitude;
        }
        else
        {
            speed = (transform.position - lastFramePosition).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        animator.SetFloat(animatorSpeedParameter, speed);
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
            () => player != null && DistanceToPlayer() <= attackRange);

        QuestionNode lowHealthNode = new QuestionNode(
            "Is health low?",
             () => currentHealth < lowHealthThreshold);

        QuestionNode playerDetectedNode = new QuestionNode(
            "Is player detected?",
             () => player != null && DistanceToPlayer() <= detectionRange);

        playerDetectedNode.falseNode = patrolNode;
        playerDetectedNode.trueNode = lowHealthNode;

        lowHealthNode.trueNode = fleeNode;
        lowHealthNode.falseNode = playerInAttackRangeNode;

        playerInAttackRangeNode.trueNode = attackNode;
        playerInAttackRangeNode.falseNode = chaseNode;

        // Set the first question as the root of the tree
        rootNode = playerDetectedNode;
        

    }
    private void Patrol()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (useGraphRouteForPatrol && PatrolUsingGraphRoute())
                return;

            PatrolUsingNavMeshWaypoints();
            return;
        }

        if (useGraphRouteForPatrol && PatrolUsingGraphRoute())
            return;

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
        MoveTowards(targetPosition, patrolSpeed);
    }
    private void PatrolUsingNavMeshWaypoints()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        if (patrolIndex < 0 || patrolIndex >= waypoints.Count)
            patrolIndex = 0;

        Transform target = waypoints[patrolIndex];
        if (target == null)
            return;

        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, patrolPointStopDistance * 4f, NavMesh.AllAreas))
        {
            navAgent.stoppingDistance = patrolPointStopDistance;
            navAgent.isStopped = false;
            navAgent.SetDestination(hit.position);

            if (!navAgent.pathPending && navAgent.remainingDistance <= patrolPointStopDistance)
            {
                patrolIndex = (patrolIndex + 1) % waypoints.Count;
            }
        }
    }
    private bool PatrolUsingGraphRoute()
    {
        if (graphRoute == null)
            return false;

        if (graphPatrolPath == null || graphPatrolPath.Count < 2)
        {
            RefreshGraphPatrolPath();
            if (graphPatrolPath == null || graphPatrolPath.Count < 2)
                return false;
        }

        graphPathIndex = Mathf.Clamp(graphPathIndex, 0, graphPatrolPath.Count - 1);
        GraphNode targetNode = graphPatrolPath[graphPathIndex];
        if (targetNode == null)
            return false;

        Vector3 targetPosition = new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z);
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= patrolPointStopDistance)
        {
            graphPathIndex++;

            if (graphPathIndex >= graphPatrolPath.Count)
            {
                // Reached goal: swap endpoints and go back to start
                SwapGraphPatrolEndpoints();
                RefreshGraphPatrolPath();
                if (graphPatrolPath == null || graphPatrolPath.Count == 0)
                    return false;
                
                // Skip the current waypoint (it's the old goal) and move to the next one
                if (graphPatrolPath.Count > 1)
                    graphPathIndex = 1;
                else
                    graphPathIndex = 0;
            }

            graphPathIndex = Mathf.Clamp(graphPathIndex, 0, graphPatrolPath.Count - 1);
            targetNode = graphPatrolPath[graphPathIndex];
            if (targetNode == null)
                return false;

            targetPosition = new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z);
        }

        MoveTowards(targetPosition, patrolSpeed);
        return true;
    }
    private void RefreshGraphPatrolPath()
    {
        graphPatrolPath.Clear();
        graphPathIndex = 0;

        if (graphRoute == null)
            return;

        graphPatrolPath = graphRoute.FindShortestPathBFS(graphPatrolStartNode, graphPatrolGoalNode);
    }
    private void SwapGraphPatrolEndpoints()
    {
        // Swap start and goal so the boss patrols in reverse on the way back
        GraphNode previousStart = graphPatrolStartNode;
        graphPatrolStartNode = graphPatrolGoalNode;
        graphPatrolGoalNode = previousStart;
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
        if (player == null)
            return;

        if (navAgent != null && navAgent.isOnNavMesh)
            navAgent.isStopped = true;

        // Face the player while attacking
        FaceTarget(player.position);

        // Handle attack cooldown
        if (attackTimer <= 0f)
        {
            if (HasAnimatorParameter(attackTriggerParameter, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(attackTriggerParameter);
            }

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
        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.stoppingDistance = attackRange;
            navAgent.isStopped = false;

            if (player != null)
                navAgent.SetDestination(player.position);

            return;
        }

        MoveTowards(player.position, moveSpeed);
    }
    private void Flee()
    {
        if (player == null)
            return;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            Vector3 away = (transform.position - player.position).normalized;
            Vector3 flee = transform.position + away * 3f;

            if (NavMesh.SamplePosition(flee, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                navAgent.stoppingDistance = 0f;
                navAgent.isStopped = false;
                navAgent.SetDestination(hit.position);
                return;
            }
        }

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

    private void FaceNavAgentMovementDirection()
    {
        if (navAgent == null || !navAgent.isOnNavMesh || navAgent.isStopped)
        {
            return;
        }

        Vector3 direction = navAgent.desiredVelocity.sqrMagnitude > 0.01f
            ? navAgent.desiredVelocity
            : navAgent.velocity;

        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private float DistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        Vector3 a = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 b = new Vector3(player.position.x, 0f, player.position.z);

        return Vector3.Distance(a, b);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss takes " + amount + " damage!");

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
            VictoryPanel.SetActive(true);
        }
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
