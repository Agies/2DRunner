using UnityEngine;
using System.Collections;

public class ChangeBehaviorGUI : MonoBehaviour {
    public Enemy[] enemies;
    private Rect btn1Rect = new Rect(100, 20, 250, 30);
    private Rect btn2Rect = new Rect(100, 50, 250, 30);

    void OnGUI()
    {

        if (GUI.Button(btn1Rect, enemies[0].gameObject.name + ": " + enemies[0].enemyAI))
        {
            if (enemies[0].enemyAI == EnemyAI.AstarToPlayer)
                enemies[0].enemyAI = EnemyAI.Runner;
            else
                enemies[0].enemyAI = EnemyAI.AstarToPlayer;
        }
        if (GUI.Button(btn2Rect, enemies[1].gameObject.name + ": " + enemies[1].enemyAI))
        {
            if (enemies[1].enemyAI == EnemyAI.AstarToPlayer)
                enemies[1].enemyAI = EnemyAI.Runner;
            else
                enemies[1].enemyAI = EnemyAI.AstarToPlayer;
        }
    }
}
