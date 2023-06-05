using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> destroyedObject;
    public static GameManager instance;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(transition);
            return;
        }
        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Awake");
    }

    private bool paused;

    //Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> expTable;
    public Inventory inventory;

    //References
    public Player player;
    public Weapon weapon;
    public Animator deathMenuAnim;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitPointBar;
    public RectTransform manaPointBar;
    public GameObject hud;
    public GameObject menu;
    public GameObject transition;
    public Button pauseButton;
    public bool alive;
    public bool hasFound = false;

    public string deadEnemies = "";
    public string collectedItems = "";

    private void Start()
    {
        if (GameObject.Find("SpawnPoint") == null)
            Debug.Log("NOT FOUND");
        else
        {
            player.transform.position = GameObject.Find("SpawnPoint").transform.position;
            Debug.Log("FOUND");
        }
    }

    private void Update()
    {

    }

    //Floating text manager
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg,  fontSize,  color,  position,  motion,  duration);
    }

    //Try update weapon
    public bool TryUpdateWeapon()
    {
        //Is weapon is the max level?
        if(weaponPrices.Count <= weapon.weaponLevel)
            return false;
        if (peso >= weaponPrices[weapon.weaponLevel])
        {
            peso -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            Debug.Log("Berhasil upgrade senjata");
            return true;
        }

        return false;
    }

    //Hitpoint bar
    public void OnHitPointChange()
    {
        float ratio = (float)player.hitPoints / (float)player.maxHitpoints;
        hitPointBar.localScale = new Vector3(ratio,1,1);
    }

    public bool characterStatus()
    {
        return player.isAlive;
    }

    //Manapoint bar
    public void OnManaPointChange()
    {
        float ratio = (float)player.Mana / (float)player.maxMana;
        manaPointBar.localScale = new Vector3(ratio, 1, 1);
    }

    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;
        
        while (experience >= add)
        {
            add += expTable[r];
            r++;

            if(r == expTable.Count)//Max level
            {
                return r;
            }
        }

        return r;
    }

    public int GetXptoLevel(int level)
    {
        int r = 0;
        int xp = 0;
        while (r < level)
        {
            xp += expTable[r];
            r++;
        }

        return xp;
    }

    public void GrantXp(int xp)
    {
        int currentLevel = GetCurrentLevel();
        experience += xp;
        if(currentLevel < GetCurrentLevel())
        {
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {
        player.OnLevelUp();
        GameManager.instance.OnHitPointChange();
        GameManager.instance.OnManaPointChange();
    }

    //Logic
    public int peso;
    public int experience;

    //On scene loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
    }

    public void PauseGame()
    {
        pauseButton.interactable = false;
        Time.timeScale = 0.0f;

    }
    public void ResumeGame()
    {
        pauseButton.interactable = true;
        Time.timeScale = 1.0f;
    }
    public void QuitGame()
    {
        GameManager.instance.SaveState();
        Application.Quit();
    }

    public void UsePotion()
    {
        if (inventory.potion.itemId == 2001)
            player.Heal(3);
        if (inventory.potion.itemId == 2002)
            player.Heal(6);
        if (inventory.potion.itemId == 2003)
        {
            if (player.maxMana - player.Mana < 4)
                player.Mana += player.maxMana;
            else
                player.Mana += 4;
        }
        if (inventory.potion.itemId == 2004)
        {
            if (player.maxMana - player.Mana < 8)
                player.Mana += player.maxMana;
            else
                player.Mana += 8;
        }
        inventory.potion.amount--;
        if (inventory.potion.amount <= 0)
        {
            inventory.potion.itemId = 0;
            inventory.potion.itemName = "";
            inventory.potion.amount = 0;
            inventory.potion.gameObject.SetActive(false);
            //menu.GetComponent<CharacterMenu>().UpdateMenu();
        }
        return;
    }

    //Death menu & respawn
    public void Respawn()
    {
        deathMenuAnim.SetTrigger("Hide");
        Time.timeScale = 1.0f;
        transition.GetComponent<Animator>().SetTrigger("In");
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Jungle" || scene.name == "Jungle 1")
            UnityEngine.SceneManagement.SceneManager.LoadScene("Jungle");
        else if (scene.name == "MainDungeon" || scene.name == "Dungeon1")
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainDungeon");
        else if (scene.name == "Underwater 1" || scene.name == "Underwater")
            UnityEngine.SceneManagement.SceneManager.LoadScene("Underwater");
        player.Respawn();
    }

    public void SaveState()
    {
        transition.GetComponent<Animator>().SetTrigger("Out");

        string s = "";

        s += "0" + "|";
        s += peso.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString() + "|";
        //s += "1000|";
        //s += "500|";
        //s += "6|";
        if(inventory.potion.itemId != 0)
        {
            PlayerPrefs.SetString("EquippedPotion", inventory.potion.itemId.ToString() + inventory.potion.amount.ToString());
        }
        else
        {
            PlayerPrefs.SetString("EquippedPotion", "");
        }

        foreach (Item item in inventory.GetItemList())
        {
            s += item.itemId.ToString();
            s += item.amount.ToString() + "|";
        }

        string temp;
        if(deadEnemies != "")
        {
            if (!PlayerPrefs.HasKey("DeadEnemies"))
                PlayerPrefs.SetString("DeadEnemies", deadEnemies);
            else
            {
                //Debug.Log(PlayerPrefs.GetString("DeadEnemies"));
                temp = PlayerPrefs.GetString("DeadEnemies");
                deadEnemies = temp + deadEnemies;
                PlayerPrefs.SetString("DeadEnemies", deadEnemies);
                deadEnemies = "";
            }
        }

        string temp1;
        if (collectedItems != "")
        {
            if (!PlayerPrefs.HasKey("CollectedItems"))
                PlayerPrefs.SetString("CollectedItems", collectedItems);
            else
            {
                //Debug.Log(PlayerPrefs.GetString("CollectedItems"));
                temp1 = PlayerPrefs.GetString("CollectedItems");
                collectedItems = temp1 + collectedItems;
                PlayerPrefs.SetString("CollectedItems", collectedItems);
                collectedItems = "";
            }
        }
        PlayerPrefs.SetString("SaveState", s);
        Debug.Log("SaveState");
    }
    public void LoadState(Scene s, LoadSceneMode mode)
    {
        Debug.Log("Loaded");
        //PlayerPrefs.SetString("DeadEnemies", "");
        //deadEnemies = PlayerPrefs.GetString("DeadEnemies");
        //Debug.Log(PlayerPrefs.GetString("DeadEnemies"));
        //PlayerPrefs.SetString("CollectedItems", "");
        //PlayerPrefs.SetString("SaveState", "0|0|0|0|");
        //collectedItems = PlayerPrefs.GetString("CollectedItems");
        //Debug.Log(PlayerPrefs.GetString("CollectedItems"));
        deadEnemies = "";
        collectedItems = "";
        transition.GetComponent<Animator>().SetTrigger("In");
        string temp = "";
        SceneManager.sceneLoaded -= LoadState;
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        //
        peso = int.Parse(data[1]);

        //Experience logic
        experience = int.Parse(data[2]);
        player.SetLevel(GetCurrentLevel());

        //Change weapon skin
        weapon.SetWeaponLevel(int.Parse(data[3]));

        //Inventory
        int itemId;
        int itemAmount;
        if((data.Length >= 4)&&(data[4] != ""))
        {
            Debug.Log(data.Length);
            for (int i = 4; i < data.Length - 1; i++)
            {
                itemId = int.Parse(data[i].Substring(0, 4));
                itemAmount = int.Parse(data[i].Substring(4, 1));
                inventory.AddItem(new Item { itemId = itemId, amount = itemAmount, itemName = inventory.GetItemName(itemId) });
                Debug.Log(itemAmount + " " + itemId);

            }
        }

        if (!PlayerPrefs.HasKey("EquippedPotion"))
            return;

        if(PlayerPrefs.GetString("EquippedPotion") != "")
        {
            itemId = int.Parse(PlayerPrefs.GetString("EquippedPotion").Substring(0, 4));
            itemAmount = int.Parse(PlayerPrefs.GetString("EquippedPotion").Substring(4, 1));
            inventory.potion = new Item { itemId = itemId, amount = itemAmount, itemName = inventory.GetItemName(itemId) };
            menu.GetComponent<CharacterMenu>().SetPotion(inventory.potion, false);
        }

        /*
        foreach (Item item in itemsPrefabs)
        {
            if (item.itemId == int.Parse(data[5]))
            {
                potion = item;
                Debug.Log("Potion = " + potion.itemId.ToString());
            }
        }*/

        Debug.Log("LoadState Weapon Level = " + int.Parse(data[3]));

    }

    public void resetScene()
    {
        Debug.Log("NEW GAME");
        if(destroyedObject != null)
        {
            for(int i = 0; i < destroyedObject.Count; i++)
            {
                Destroy(destroyedObject[i]);
            }
        }
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MenuUtama");
    }
}
