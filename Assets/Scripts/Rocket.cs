using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    private bool m_isAxisInUse = false;
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float thrustPower = 20f;
    [SerializeField] AudioClip engineSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip levelCompleteSound;

    [SerializeField] ParticleSystem engineParticle;
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] ParticleSystem levelCompleteParticle;


    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionDisabled = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrust();
            RespondToRotate();
            
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {

            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosionSound);
        explosionParticle.Play();
        Invoke("LoadDefaultScene", 1f);
    }

    void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleteSound);
        levelCompleteParticle.Play();
        Invoke("LoadNextScene", 1f);
    }

    void LoadDefaultScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    void LoadNextScene()
    {
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void RespondToThrust()
    {
        float thrust = Input.GetAxis("Vertical") * thrustPower;
        rigidBody.AddRelativeForce(Vector3.up * thrust);
        if ((Input.GetAxis("Vertical") != 0 || (Input.GetAxis("Strafe")) != 0))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(engineSound);
            }
            engineParticle.Play();
        }
        else
        {
            audioSource.Stop();
            engineParticle.Stop();
        }
    }



    void RespondToRotate()
    {
        rigidBody.freezeRotation = true;

        float rotationSpeed = rcsThrust * Time.deltaTime;

        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        transform.Rotate(0, 0, -rotation);
    }

    void RepsondToStrafe()
    {
        float strafe = Input.GetAxis("Strafe") * thrustPower;
        rigidBody.AddRelativeForce(Vector3.right * strafe);
        
    }

    
   
}
