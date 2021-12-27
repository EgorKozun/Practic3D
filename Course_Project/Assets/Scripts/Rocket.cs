using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rotSpeed = 100f;
    [SerializeField] float flySpeed = 100f;
    [SerializeField] AudioClip flySound;
    [SerializeField] AudioClip boomSound;
    [SerializeField] AudioClip finishSound;
    [SerializeField] ParticleSystem flyParticles;
    [SerializeField] ParticleSystem boomParticles;
    [SerializeField] ParticleSystem finishParticles;
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Playing, Dead, NextLevel};
    State state = State.Playing;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Playing)
        {
            Rotation();
            Launch();
        }
        DebugKeys();
    }

    void DebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Playing)
        {
            return;
        }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("Ok");
                break;
            case "Finish":
                Finish();
                break;
            default:
                Lose();
                break;
        }
    }

    void Lose()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(boomSound);
        boomParticles.Play();
        state = State.Dead;
        print("Boom");
        Invoke("FirstLevel", 2f);
    }
    void Finish()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(finishSound);
        finishParticles.Play();
        state = State.NextLevel;
        Invoke("LoadNextLevel", 2f);
    }
    void LoadNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;

        if(nextLevelIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 0;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }

    void FirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    void Launch()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * flySpeed * Time.deltaTime);
            if(audioSource.isPlaying == false)
            audioSource.PlayOneShot(flySound);
            flyParticles.Play();
        }
        else
        {
            audioSource.Pause();
            flyParticles.Stop();
        }

    }


    void Rotation()
    {
        float rotationSpeed = rotSpeed * Time.deltaTime;

        rigidBody.freezeRotation = true;

        if(Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        else if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rigidBody.freezeRotation = false;
    }
}
