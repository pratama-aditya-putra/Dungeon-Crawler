using UnityEngine;

public class Portal : Collidable
{
    public string sceneName;

    protected override void OnCollide(Collider2D coll)
    {
        if(coll.name == "Player" || coll.name == "Jaka")
        {
            //Teleport Player
            GameManager.instance.SaveState();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            //GameManager.instance.deadEnemies = "";
            //GameManager.instance.collectedItems = "";
        }
    }
}
