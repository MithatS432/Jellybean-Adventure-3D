using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    private Animator anim;
    private AudioSource audioSource;

    [Header("Sounds")]
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip sweetSound;

    [Header("Player Settings")]
    public float speed = 10f;
    public float jumpSpeed = 8f;
    public bool isGrounded = false;
    public bool isAlive = true;

    public GameObject winEffect;
    public GameObject loseEffect;

    public bool getSweet = false;
    public GameObject sweetEffect;
    public bool isRed;
    public bool isBlue;
    public bool isYellow;
    public bool isSpecial;
    private bool originalInvincible = false;

    private bool redTaken = false;
    private bool blueTaken = false;
    private bool yellowTaken = false;
    private bool specialTaken = false;



    public Joystick joystick;

    [Header("UI Settings")]
    public Image levelProgress;
    private float startZ;
    private float finishZ;
    public Transform finalProgress;

    public TextMeshProUGUI countDown;
    public AudioClip countSound;
    private bool isSound;

    private float countdownTimer = 3.001f;
    public bool isGameStart = false;

    private Vector3 moveDirection;

    [Header("Buttons")]
    public Button pause;
    public Button continueB;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.freezeRotation = true;

        countDown.text = "3";

        pause.interactable = false;
        continueB.interactable = false;

        pause.onClick.AddListener(PauseGame);
        continueB.onClick.AddListener(ContinueGame);

        startZ = transform.position.z;
        finishZ = finalProgress.position.z;
    }

    void Update()
    {
        if (!isGameStart)
        {
            RunCountdown();
            return;
        }

        if (isAlive)
        {
            float h = joystick.Horizontal;
            moveDirection = new Vector3(h * speed, 0, speed);
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (isGameStart && isAlive)
        {
            float distance = transform.position.z - startZ;
            float total = finishZ - startZ;

            float progress = Mathf.Clamp01(distance / total);
            levelProgress.fillAmount = progress;
        }
    }

    void RunCountdown()
    {
        countdownTimer -= Time.deltaTime;

        if (!isSound)
        {
            isSound = true;
            AudioSource.PlayClipAtPoint(countSound, transform.position);
        }

        if (countdownTimer <= 3f && countdownTimer > 2f)
            countDown.text = "3";
        else if (countdownTimer <= 2f && countdownTimer > 1f)
            countDown.text = "2";
        else if (countdownTimer <= 1f && countdownTimer > 0f)
            countDown.text = "1";
        else if (countdownTimer <= 0f)
        {
            countDown.text = "GO";
            isGameStart = true;
            pause.interactable = true;
            Invoke(nameof(HideCountdown), 0.5f);
        }
    }

    void HideCountdown()
    {
        countDown.gameObject.SetActive(false);
    }

    void PauseGame()
    {
        if (!isAlive) return;

        isGameStart = false;
        Time.timeScale = 0f;
        continueB.interactable = true;
        pause.interactable = false;
    }

    void ContinueGame()
    {
        Time.timeScale = 1f;
        isGameStart = true;
        continueB.interactable = false;
        pause.interactable = true;
    }

    void FixedUpdate()
    {
        if (isGameStart && isAlive)
        {
            rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

            float x = Mathf.Clamp(rb.position.x, -4.36f, 4.36f);
            rb.position = new Vector3(x, rb.position.y, rb.position.z);
            rb.rotation = Quaternion.identity;
        }
    }
    public void FootStep()
    {
        if (!isAlive || !isGameStart)
            return;

        if (runSound != null)
            audioSource.PlayOneShot(runSound);
    }


    public void Jump()
    {
        if (isGrounded && isGameStart && isAlive)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            if (jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Win();
            return;
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }

        if (other.CompareTag("Red") && !redTaken)
        {
            AudioSource.PlayClipAtPoint(sweetSound, transform.position);
            redTaken = true;
            isRed = true;
            StartCoroutine(GetSweet(3));
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Blue") && !blueTaken)
        {
            AudioSource.PlayClipAtPoint(sweetSound, transform.position);
            blueTaken = true;
            isBlue = true;
            StartCoroutine(GetSweet(3));
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Yellow") && !yellowTaken)
        {
            AudioSource.PlayClipAtPoint(sweetSound, transform.position);
            yellowTaken = true;
            isYellow = true;
            StartCoroutine(GetSweet(3));
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Special") && !specialTaken)
        {
            AudioSource.PlayClipAtPoint(sweetSound, transform.position);
            specialTaken = true;
            isSpecial = true;
            StartCoroutine(GetSweet(3));
            Destroy(other.gameObject);
        }
    }

    void Win()
    {
        if (!isAlive) return;

        isAlive = false;
        isGameStart = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (winEffect != null)
            Instantiate(winEffect, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(winSound, transform.position);

        Invoke(nameof(LoadNextLevel), 2f);
    }
    void Die()
    {
        if (!isAlive || originalInvincible) return;

        isAlive = false;
        isGameStart = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        anim.SetTrigger("Die");

        if (loseEffect != null)
            Instantiate(loseEffect, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(loseSound, transform.position);

        Invoke(nameof(RestartLevel), 2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator GetSweet(int duration)
    {
        if (getSweet) yield break; // Zaten güç aktifse tekrar başlatma

        getSweet = true;

        GameObject effect = Instantiate(sweetEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3f);


        // Orijinal değerleri sakla
        float originalSpeed = speed;
        float originalJump = jumpSpeed;

        if (isRed || isSpecial)
        {
            speed *= 2f; // Hızı 2 kat artır
        }

        if (isBlue || isSpecial)
        {
            jumpSpeed *= 2f; // Zıplama yüksekliğini 2 kat artır
        }

        if (isYellow || isSpecial)
        {
            originalInvincible = true; // Engellerden hasar alma
        }

        // Süre boyunca bekle
        yield return new WaitForSeconds(duration);

        // Değerleri eski hâline getir
        if (isRed || isSpecial)
            speed = originalSpeed;

        if (isBlue || isSpecial)
            jumpSpeed = originalJump;

        if (isYellow || isSpecial)
            originalInvincible = false;

        // Flagleri sıfırla
        isRed = false;
        isBlue = false;
        isYellow = false;
        isSpecial = false;
        getSweet = false;
    }

}
