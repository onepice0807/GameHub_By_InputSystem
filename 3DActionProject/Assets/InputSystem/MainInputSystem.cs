using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MainInputSystem : MonoBehaviour
{
    // Input System Actions Ŭ������ ����
    MainInputSystemActions inputActions;
    public float rollSpeed = 90f;  // ť�갡 ȸ���� �ӵ� 
    private Vector2 moveInput; // WASD �� ���� �Է� ���� �����ϴ� ����
    private bool isRolling = false; // ���� ť�갡 ȸ�� ������ ���� boolŸ������ Ȯ��
    private float cubeSize = 2f; // ť���� ũ�� (ť�갡 2x2x2 ũ���� ���� ����)


    private void Awake()
    {
        inputActions = new MainInputSystemActions();  // Input Actions �ν��Ͻ��� ����
    }

    ///  ������Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �޼��� (Input System�� Ȱ��ȭ �� �Է� �̺�Ʈ�� ����)
    private void OnEnable()
    {
        // Input System Ȱ��ȭ
        inputActions.PlayerAction.Enable();

        // �̵� �Է� �̺�Ʈ ����
        inputActions.PlayerAction.Move.performed += OnMoveAction;
        inputActions.PlayerAction.Move.canceled += OnMoveCancel;

        // ���� �Է� �̺�Ʈ ����
        inputActions.PlayerAction.Jump.performed += OnJumpAction;

        // ���� �Է� �̺�Ʈ ����
        inputActions.PlayerAction.Attack.performed += OnAttackAction;
    }

    /// ������Ʈ�� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼��� (Input System�� ��Ȱ��ȭ �� �Է� �̺�Ʈ�� ����)
    private void OnDisable()
    {
        // Move �̺�Ʈ ���� ����
        inputActions.PlayerAction.Move.performed -= OnMoveAction;
        inputActions.PlayerAction.Move.canceled -= OnMoveCancel;

        // Input System ��Ȱ��ȭ
        inputActions.PlayerAction.Disable();
    }

    // Move �׼��� ����� �� ȣ�� �̵� �Է� ���� �����ϰ� ť���� ȸ���� ����
    private void OnMoveAction(InputAction.CallbackContext context)
    {
        // �Էµ� �̵� ���� Vector2�� �о�´�
        moveInput = context.ReadValue<Vector2>();

        // �̵� �Է��� �ִ� ��� ȸ���� ����
        if (!isRolling)
            StartCoroutine(Roll(moveInput));
    }

    // ť�긦 �־��� �������� 90�� ȸ����Ű�� �ڷ�ƾ
    private System.Collections.IEnumerator Roll(Vector2 direction)
    {
        isRolling = true; // ȸ�� ���¸� isRolling�� Ȱ��ȭ�Ͽ� �ߺ� �Է� ����

        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y); // �Էµ� 2D ������ 3D �̵� ���ͷ� ��ȯ

        // �ǹ� ����Ʈ ���: ť���� �̵� ����� �Ʒ��� �������� ť�� ���ݸ�ŭ �̵�
        Vector3 pivotOffset = moveDirection * (cubeSize / 2f) + Vector3.down * (cubeSize / 2f);
        Vector3 pivotPoint = transform.position + pivotOffset;

        // ȸ�� �� ���: ���� ����(Vector3.up)�� �̵� ������ ������ ���
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDirection);

        // ȸ���ؾ� �� �� ���� ����
        float remainingAngle = 90f;

        // ť�갡 90�� ȸ���� ������ �ݺ�
        while (remainingAngle > 0)
        {
            // ���� �����ӿ��� ȸ���� ������ ��� (rollSpeed�� Time.deltaTime ���)
            float rotationStep = Mathf.Min(rollSpeed * Time.deltaTime, remainingAngle);

            // �ǹ� ����Ʈ�� �������� ť�� ȸ��
            transform.RotateAround(pivotPoint, rotationAxis, rotationStep);

            // ���� ȸ�� ������ ����
            remainingAngle -= rotationStep;

            // �� ������ ���
            yield return null;
        }

        // ���� ��ġ�� ť�� ũ�⿡ �°� ����
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / cubeSize) * cubeSize,
            Mathf.Round(transform.position.y / cubeSize) * cubeSize,
            Mathf.Round(transform.position.z / cubeSize) * cubeSize
        );

        // ���� ȸ������ 90�� ������ ����
        transform.rotation = Quaternion.Euler(
            Mathf.Round(transform.rotation.eulerAngles.x / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90
        );

        // ȸ�� ���¸� �����Ͽ� �Է��� �ٽ� ���� �� �ֵ��� ����
        isRolling = false;
    }

    // Move �׼��� ��ҵ� �� ȣ�� �̵� �Է� ���� �ʱ�ȭ
    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        // �̵� �Է� ���� �ʱ�ȭ
        moveInput = Vector2.zero;
    }

    // Jump �׼��� ����� �� ȣ��
    private void OnJumpAction(InputAction.CallbackContext obj)
    {
        // ���� ���� ó�� (���� ����� �޽��� ���)
        Debug.Log("Jump");
    }

    //  Attack �׼��� ����� �� ȣ��
    private void OnAttackAction(InputAction.CallbackContext obj)
    {
        // ���� ���� ó�� (���� ����� �޽��� ���)
        Debug.Log("Attack");
    }

    void Start()
    {
        // �ʱ�ȭ �Ǵ� ���� ������ �ʿ��ϸ� ���⿡ �ۼ�
    }

    void Update()
    {
        // ������ ������ ó���ؾ� �� ���� �ۼ� ����
    }
}
