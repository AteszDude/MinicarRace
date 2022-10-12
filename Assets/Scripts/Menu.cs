using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    private void Awake() {
        SceneInfo.AI = 0;
        SceneInfo.Map = 0;
    }

    public void startPressed() {
        SceneInfo.CustomAINUM = true;
        SceneManager.LoadScene("Game");
    }

    public void setMap(TMP_Dropdown dropdown) {
        SceneInfo.Map = dropdown.value;
    }

    public void setAI(TMP_Dropdown dropdown) {
        SceneInfo.AI = dropdown.value;
    }

    public void exitPressed() {
        Application.Quit();
    }
}