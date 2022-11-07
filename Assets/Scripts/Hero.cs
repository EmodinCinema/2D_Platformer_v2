using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero : Entity
{
    [SerializeField] private float speed = 3f;  //Скорость движения
    [SerializeField] private int lives = 5; //Колличество жизней
    [SerializeField] private float jumpForce = 15f; //Сила прыжка
    private bool isGrounded = false;

    //Ссылки на компоненты
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;


    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        Instance = this;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (isGrounded) State = States.Idle;

        if (Input.GetButton("Horizontal"))
        {
            Run();
        }
            
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (transform.position.y <= -17f)
        {
            Die();
            SceneManager.LoadScene(0);
        }
    }

    //Ходьба
    private void Run()
    {
        if (isGrounded) State = States.Run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }

    //Прыжок
    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    //Есть ли у нас что-то под ногами
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 2;

        if (!isGrounded) State = States.Jump;
    }

    public override void GetDamage()
    {
        lives -= 1;
        Debug.Log("Жизнь персонажа: " + lives);
    }

    //Смерть о шипы
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Respawn")
        {
            SceneManager.LoadScene(0);
        }
    }
}
public enum States
{
    Idle,
    Run,
    Jump,
    
}

