using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class EndControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mainText;
    [SerializeField]
    private GameObject restartBtn;
    [SerializeField]
    private GameObject quitBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        mainText.text = "A RECOMEÇAR O JOGO...";

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Restart Game
        SceneManager.LoadScene("Main");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        // Reset score after restart
        MultiTargetsManager.score = 0;

        // Reset matches status
        MultiTargetsManager.foundFirstMatch = false;
        MultiTargetsManager.foundSecondMatch = false;
        MultiTargetsManager.foundThirdMatch = false;
    }

        public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    IEnumerator QuitGameCoroutine()
    {
        mainText.text = "OBRIGADO POR JOGARES!";

        yield return new WaitForSeconds(3f);

        // Quit game
        Application.Quit();
    }
}
