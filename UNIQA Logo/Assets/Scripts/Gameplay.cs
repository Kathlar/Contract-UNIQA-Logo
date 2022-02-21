using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector3 startPositionOfTopLevel;
    public GameObject camEnd1, camEnd2;
    
    public GameObject arrowsParent;

    private int levelCount;
    
    public enum State
    {
        beforeStart = 0, chooseCharacter = 1, gameplay = 2, ending = 3
    }
    private State state = State.beforeStart;

    private void Start()
    {
        SetState(State.beforeStart);
        
        for (int i = 7; i < levels.Count; i++)
            levels[i].Randomize(i, levels[i-1]);

        startPositionOfTopLevel = levels[levels.Count - 1].transform.localPosition;
        levelCount = levels.Count;
        maxSpeed = speed * 1.5f;
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
                bottomLevel.Randomize(levelCount, levels[levels.Count - 2]);
                levelCount++;
                if (ending) playing = false;
            }
            
            if (Input.GetKeyDown(KeyCode.G)) Finish();

            speed = Mathf.Clamp(speed + Time.deltaTime / 25, speed, maxSpeed);
        }
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
        do
        {
            chosenCharacter.transform.position = Vector3.MoveTowards(chosenCharacter.transform.position,
                gameplayCharacterPosition.position, Time.deltaTime * 10);
            chosenCharacter.transform.rotation = gameplayCharacterPosition.rotation;
            yield return null;
        } while (Vector3.Distance(chosenCharacter.transform.position, gameplayCharacterPosition.position) > .1f);

        chosenCharacter.animator.SetTrigger("OnBuilding");
        yield return new WaitForSeconds(1f);

        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "";
        
        chosenCharacter.animator.SetTrigger("Run");
        playing = true;
        chosenCharacter.StartGameplay();
        arrowsParent.SetActive(true);
    }

    public void ChooseCharacter(bool left)
    {
        SetState(State.gameplay);
        chosenCharacter = left ? leftCharacter : rightCharacter;
    }

    public void Finish()
    {
        int numb = levels.Count - 5;
        levels[0].SetFinished();
        for (int i = numb; i < levels.Count; i++)
            levels[i].SetFinished();
        SetState(State.ending);
        ending = true;
        speed = 20;

        StartCoroutine(FinishCoroutine());
    }
    
    private IEnumerator FinishCoroutine()
    {
        yield return new WaitForSeconds(7);
        camEnd2.SetActive(true);
        camEnd1.SetActive(false);
    }

    public void PressedMovementButton(bool right)
    {
        chosenCharacter.PressedMoveButton(right);
    }
}
