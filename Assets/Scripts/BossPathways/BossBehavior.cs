using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BossBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<Transform> waypoints;

    [Header("BossStats")]
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackRange = 5f;
    public float attackRate = 1f;

    [Header("Decision Range")]
    public float detectionRange = 10f;
    public float attackThreshold = 15f;
    public float lowHealthThreshold = 30f;

    private DecisionNode rootNode;               
    private int patrolIndex;                     
    private float attackTimer;                   
    private float reloadTimer;                   
    private bool isReloading; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void BuildDecisionTree()
    {
        // Create decision nodes
        ActionNode attackNode = new ActionNode("Attack");
        ActionNode patrolNode = new ActionNode("Patrol");
        ActionNode reloadNode = new ActionNode("Reload");

        QuestionNode playerInAttackRangeNode = new QuestionNode(
            "Is player in attack range?",
            () => Vector3.Distance(transform.position, player.position) <= attackRange);

        QuestionNode lowHealthNode = new QuestionNode(
            "Is health low?",
             () => currentHealth < lowHealthThreshold);

        QuestionNode playerDetectedNode = new QuestionNode(
            "Is player detected?",
             () => Vector3.Distance(transform.position, player.position) <= detectionRange);
        

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