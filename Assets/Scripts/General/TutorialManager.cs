using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> listTutorial;
    [SerializeField]
    public List<GameObject> listButton;
    [SerializeField]
    public Sprite pressedButton;
    [SerializeField]
    public Sprite upButton;
    private int currentPage;
    void Start()
    {
        currentPage = 1;
        activatePage(currentPage);
    }

    protected void activatePage(int i)
    {
        listTutorial[i-1].active = true;
        listButton[i - 1].GetComponent<Image>().sprite = pressedButton;
        currentPage = i;
    }
    protected void deactivatePage(int i)
    {
        listTutorial[i-1].active = false;
        listButton[i - 1].GetComponent<Image>().sprite = upButton;
    }

    public void activatePageString(string temp)
    {
        Debug.Log("Pressed");
        changePage(currentPage,int.Parse(temp));
    }

    protected void changePage(int i, int j)
    {
        deactivatePage(i);
        activatePage(j);
    }

    public void nextPage()
    {
        int temp = currentPage;
        if (currentPage < 3)
            currentPage++;
        else if (currentPage >= 3)
            currentPage = 1;
        changePage(temp, currentPage);
    }
    public void prevPage()
    {
        int temp = currentPage;
        if (currentPage >= 1)
            currentPage--;
        else if (currentPage < 1)
            currentPage = 3;
        changePage(temp, currentPage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
