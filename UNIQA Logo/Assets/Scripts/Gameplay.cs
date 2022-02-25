using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Gameplay : MonoBehaviour
{
    public List<UniqaLevel> levels = new List<UniqaLevel>();

    public List<GameObject> objBeforeStart = new List<GameObject>();
    public List<GameObject> objChooseCharacter = new List<GameObject>();
    public List<GameObject> objGameplay = new List<GameObject>();
    public List<GameObject> objEnding = new List<GameObject>();

    public AudioSource startMusic, gameplayMusic;

    public Character leftCharacter, rightCharacter;
    private Character chosenCharacter;
    public Transform gameplayCharacterPosition;

    public Text countdownText;

    private bool playing, ending;
    public float speed = 4;
    private float maxSpeed;
    public GameObject camEnd1, camEnd2;

    public GameObject[] winElements, loseElements;
    
    public GameObject arrowsParent;

    private int levelCount;
    public static int collectedCollectibles;

    public MeshRenderer[] winLetters;
    public Material winMaterial;
    
    public enum State
    {
        beforeStart = 0, chooseCharacter = 1, gameplay = 2, ending = 3
    }
    private State state = State.beforeStart;

    private IEnumerator Start()
    {
        collectedCollectibles = 0;
        SetState(State.beforeStart);
        
        for (int i = 7; i < levels.Count; i++)
            levels[i].Randomize(i, levels[i-1]);

        levelCount = levels.Count;
        maxSpeed = speed * 1.5f;

        foreach (GameObject go in winElements)
            go.SetActive(false);
        foreach (GameObject go in loseElements)
            go.SetActive(false);

        Color logoColor = uniqaLogo.color;
        logoColor.a = 1;
        uniqaLogo.color = logoColor;
        uniqaLogo.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        do
        {
            logoColor.a -= Time.deltaTime;
            uniqaLogo.color = logoColor;
            yield return null;
        } while (logoColor.a > 0);

        uniqaLogo.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playing)
        {
            foreach (var level in levels)
                level.transform.position += Vector3.down * speed * Time.deltaTime;

            if (levels[0].transform.localPosition.y < -3.5f)
            {
                var bottomLevel = levels[0];
                levels.Add(bottomLevel);
                levels.RemoveAt(0);
                bottomLevel.transform.localPosition = 
                    levels[levels.Count - 2].transform.localPosition + Vector3.up * 3.5f;
                if (!ending) bottomLevel.Randomize(levelCount, levels[levels.Count - 2]);
                else playing = false;
                levelCount++;
            }
            
            speed = Mathf.Clamp(speed + Time.deltaTime / 25, speed, maxSpeed);
        }

        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetState(int state)
    {
        SetState((State)state);
    }
    
    public void SetState(State state)
    {
        this.state = state;

        foreach (GameObject obj in objBeforeStart)
            obj.SetActive(state == State.beforeStart);
        foreach (GameObject obj in objChooseCharacter)
            obj.SetActive(state == State.chooseCharacter);
        foreach (GameObject obj in objGameplay)
            obj.SetActive(state == State.gameplay);
        foreach (GameObject obj in objEnding)
            obj.SetActive(state == State.ending);

        if (state == State.beforeStart)
        {
            startMusic.Play();
            gameplayMusic.Stop();
        }
        else if (state == State.gameplay)
        {
            startMusic.Stop();
            gameplayMusic.Play();
            StartCoroutine(StartGameplay());
        }
    }

    IEnumerator StartGameplay()
    {
        countdownText.text = "";
        yield return new WaitForSeconds(1f);

        chosenCharacter.animator.SetTrigger("Gameplay");
        chosenCharacter.transform.rotation = gameplayCharacterPosition.rotation;
        do
        {
            chosenCharacter.transform.position = Vector3.MoveTowards(chosenCharacter.transform.position,
                gameplayCharacterPosition.position, Time.deltaTime * 10);
            yield return null;
        } while (Vector3.Distance(chosenCharacter.transform.position, gameplayCharacterPosition.position) > .1f);
        chosenCharacter.transform.rotation = gameplayCharacterPosition.rotation;
        
        chosenCharacter.animator.SetTrigger("OnBuilding");
        chosenCharacter.landParticles.Play();
        yield return new WaitForSeconds(1f);

        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "";
        
        chosenCharacter.animator.SetTrigger("Run");
        chosenCharacter.runParticles.Play();
        playing = true;
        chosenCharacter.StartGameplay();
        arrowsParent.SetActive(true);
    }

    public void ChooseCharacter(bool left)
    {
        SetState(State.gameplay);
        chosenCharacter = left ? leftCharacter : rightCharacter;
    }

    public void PressedMovementButton(bool right)
    {
        chosenCharacter.PressedMoveButton(right);
    }

    public void Lose(Room room)
    {
        if (!playing || ending) return;
        int numb = levels.Count - 5;
        levels[0].SetFinished();
        for (int i = numb; i < levels.Count; i++)
            levels[i].SetFinished();
        ending = true;
        speed = 20;
        chosenCharacter.OnLose(room.landPoint, collectedCollectibles >= 5);
        StartCoroutine(OnLostCoroutine(room));
        room.ShowCamera(true);
        chosenCharacter.runParticles.Stop();
    }

    private IEnumerator OnLostCoroutine(Room room)
    {
        bool won = collectedCollectibles >= 5;
        if (won) Waving.Wave();
        
        yield return new WaitForSeconds(6);
        room.ShowCamera(false);
        SetState(State.ending);
        yield return new WaitForSeconds(3);
        Material lostMaterial = winLetters[0].material;
        for (int i = 0; i < collectedCollectibles; i++)
        {
            for (int j = 0; j < Random.Range(3, 7); j++)
            {
                winLetters[i].material = winMaterial;
                yield return new WaitForSeconds(.1f);
                winLetters[i].material = lostMaterial;
                yield return new WaitForSeconds(Random.Range(0.06f, .2f));
            }
            winLetters[i].material = winMaterial;
            yield return new WaitForSeconds(.5f);
        }

        if (!won)
        {
            for (int i = 0; i < collectedCollectibles; i++)
            {
                for (int j = 0; j < Random.Range(1, 4); j++)
                {
                    winLetters[i].material = lostMaterial;
                    yield return new WaitForSeconds(.1f);
                    winLetters[i].material = winMaterial;
                    yield return new WaitForSeconds(Random.Range(0.06f, .2f));
                }
                winLetters[i].material = lostMaterial;
                yield return new WaitForSeconds(.5f);
            }
        }
        yield return new WaitForSeconds(2);
        foreach (GameObject gameObject in winElements)
            gameObject.SetActive(won);
        foreach (GameObject gameObject in loseElements)
            gameObject.SetActive(!won);
        camEnd2.SetActive(true);
        camEnd1.SetActive(false);
    }

    private bool restarting;
    public Image uniqaLogo;
    public void RestartGame()
    {
        if (restarting) return;
        restarting = true;
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        Color logoColor = uniqaLogo.color;
        logoColor.a = 0;
        uniqaLogo.color = logoColor;
        uniqaLogo.gameObject.SetActive(true);
        do
        {
            logoColor.a += Time.deltaTime;
            uniqaLogo.color = logoColor;
            yield return null;
        } while (logoColor.a < 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
