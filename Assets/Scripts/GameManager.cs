using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    // [SerializeField] private Transform player1SpawnPoint;
    // [SerializeField] private Transform player2SpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayerTakeDamage(bool isPlayer1, int dmgType)
    {
        PlayerController targetPlayer = isPlayer1
            ? player1Prefab.GetComponent<PlayerController>()
            : player2Prefab.GetComponent<PlayerController>();

        float damage = 0f;

        switch (dmgType)
        {
            case damageType.BASIC:
                damage = isPlayer1
                    ? player2Prefab.GetComponent<PlayerController>().atk
                    : 10f;
                break;
            case damageType.ENV:
                damage = 5f;
                break;
            case damageType.ITEM:
                damage = 20f;
                break;
        }

        targetPlayer.maxHealth -= damage;
        Debug.Log($"Player {(isPlayer1 ? "1" : "2")} took {damage} damage. Remaining health: {targetPlayer.maxHealth}");
    }

    public class damageType
    {
        public const int BASIC = 0;
        public const int ENV = 1;
        public const int ITEM = 2;
    }
}
