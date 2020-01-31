using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Базовые параметры 
    public float movementSpeed = 5f;

    // Вспомогрательные параметры
    private int jumpCount = 0;
    private Vector3 touchPos; //Координаты тапа
    private Vector3 velocity; //Для движения по оси Y
    private float jumpHeight;
    private Vector3 jumpTarget;
    private bool isJumpZone;
    private float gravityForce = -20f;
    private int i = 0; //Для перебора прыжков

    // Компоненты
    public DataManager dataManager;
    public Jump[] jumps;
    public Transform jumpZone;

    private Touch touch;
    private Animator animator;
    private CharacterController ch_controller;


    void Start()
    {
        jumpCount = dataManager.jumpCount;
        animator = GetComponent<Animator>();
        ch_controller = GetComponent<CharacterController>();

        JumpSetUp(); // Выставление парамметров первого прыжка (высота прыжка и размер диска)
    }

    void Update()
    {
        // Движение игрока по оси X
        PlayerMove();

        // Движение игрока по оси Y
        Gravity();

        // Спавн при выходе за границы экрана
        Spawn();

        // Если было нажатие на экран, вызов функции прыжка
        if (Input.touchCount > 0)
        {
            PlayerJump();
        }
    }

    // Постоянное движение по оси X
    private void PlayerMove()
    {
        // Если игрок на земле
        if (ch_controller.isGrounded)
            ch_controller.Move(Vector3.right * movementSpeed * Time.deltaTime); // Постоянное движение вправо
        else
            MoveToPoint(); // Движение к точке тапа
    }

    // Гравитация и движение по оси Y
    private void Gravity()
    {
        // Прижемаем игрока по полу
        if (ch_controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravityForce * Time.deltaTime;
        ch_controller.Move(velocity * Time.deltaTime);

        // Флип реализован через изменение времени анимации в зависимости от положения по оси Y
        animator.speed = 1 / transform.position.y;
    }

    private void Spawn()
    {
        // Спавн в случае выхода за границы экрана
        if (transform.position.x > 16f)
        {
            transform.position = new Vector3(-16f, 0, 0);
        }
    }

    // Перемещение по оси X к точке тапа
    void MoveToPoint()
    {
        if (jumpTarget == transform.position)
            return;

        float distance = transform.position.x - jumpTarget.x;
        ch_controller.Move(Vector3.left * distance * Time.deltaTime * 1.5f);
    }

    // Подготовка нового прыжка
    private void JumpSetUp()
    {
        jumpZone.localScale = new Vector3(jumps[i].jumpZoneRadius, 0.01f, jumps[i].jumpZoneRadius);
        jumpHeight = jumps[i].jumpHeith;
        Debug.Log("Anim speed = " + animator.speed);

        Debug.Log("Jump " + i);
        i++;
        if (i >= jumps.Length)
            i = 0;
    }

    // Счетчик прыжков
    private void JumpCounter()
    {
        dataManager.jumpCount++;
        dataManager.DisplayJumpCount(); // Показываем результат
        dataManager.SaveData(); // Сохраняем прогресс
    }


    // Метод прыжка
    private void PlayerJump()
    {
        // Если на земле и в зоне прыжка
        if (ch_controller.isGrounded && isJumpZone)
        {
            GetTouchPosition();
            // Если тап был левее игрока
            if (touchPos.x < transform.position.x)
            {
                jumpTarget = new Vector3(touchPos.x, jumpHeight, 0);
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce); // Установка прыжка
                Debug.Log("Jump!");

                animator.SetTrigger("isJump");

                JumpCounter(); // Увеличиваем счетчик прыжков
                JumpSetUp(); // Готовим настройки следующего прыжка и нового диска
            }
        }
    }

    // Метод находит точку тапа относительно мира
    private void GetTouchPosition()
    {
        touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
            touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -Camera.main.transform.position.z));
    }

    // Определение зоны прыжка по тригеру коллайдера
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "JumpZone")
            isJumpZone = true;
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "JumpZone")
            isJumpZone = false;
    }
}