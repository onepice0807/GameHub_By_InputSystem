using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MainInputSystem : MonoBehaviour
{
    // Input System Actions 클래스를 참조
    MainInputSystemActions inputActions;
    public float rollSpeed = 90f;  // 큐브가 회전할 속도 
    private Vector2 moveInput; // WASD 등 방향 입력 값을 저장하는 변수
    private bool isRolling = false; // 현재 큐브가 회전 중인지 여부 bool타입으로 확인
    private float cubeSize = 2f; // 큐브의 크기 (큐브가 2x2x2 크기일 경우로 선정)


    private void Awake()
    {
        inputActions = new MainInputSystemActions();  // Input Actions 인스턴스를 생성
    }

    ///  오브젝트가 활성화될 때 호출되는 메서드 (Input System을 활성화 및 입력 이벤트를 연결)
    private void OnEnable()
    {
        // Input System 활성화
        inputActions.PlayerAction.Enable();

        // 이동 입력 이벤트 연결
        inputActions.PlayerAction.Move.performed += OnMoveAction;
        inputActions.PlayerAction.Move.canceled += OnMoveCancel;

        // 점프 입력 이벤트 연결
        inputActions.PlayerAction.Jump.performed += OnJumpAction;

        // 공격 입력 이벤트 연결
        inputActions.PlayerAction.Attack.performed += OnAttackAction;
    }

    /// 오브젝트가 비활성화될 때 호출되는 메서드 (Input System을 비활성화 및 입력 이벤트를 해제)
    private void OnDisable()
    {
        // Move 이벤트 연결 해제
        inputActions.PlayerAction.Move.performed -= OnMoveAction;
        inputActions.PlayerAction.Move.canceled -= OnMoveCancel;

        // Input System 비활성화
        inputActions.PlayerAction.Disable();
    }

    // Move 액션이 수행될 때 호출 이동 입력 값을 저장하고 큐브의 회전을 시작
    private void OnMoveAction(InputAction.CallbackContext context)
    {
        // 입력된 이동 값을 Vector2로 읽어온다
        moveInput = context.ReadValue<Vector2>();

        // 이동 입력이 있는 경우 회전을 시작
        if (!isRolling)
            StartCoroutine(Roll(moveInput));
    }

    // 큐브를 주어진 방향으로 90도 회전시키는 코루틴
    private System.Collections.IEnumerator Roll(Vector2 direction)
    {
        isRolling = true; // 회전 상태를 isRolling를 활성화하여 중복 입력 방지

        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y); // 입력된 2D 방향을 3D 이동 벡터로 변환

        // 피벗 포인트 계산: 큐브의 이동 방향과 아래쪽 방향으로 큐브 절반만큼 이동
        Vector3 pivotOffset = moveDirection * (cubeSize / 2f) + Vector3.down * (cubeSize / 2f);
        Vector3 pivotPoint = transform.position + pivotOffset;

        // 회전 축 계산: 위쪽 방향(Vector3.up)과 이동 방향의 외적을 사용
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDirection);

        // 회전해야 할 총 각도 설정
        float remainingAngle = 90f;

        // 큐브가 90도 회전할 때까지 반복
        while (remainingAngle > 0)
        {
            // 현재 프레임에서 회전할 각도를 계산 (rollSpeed와 Time.deltaTime 사용)
            float rotationStep = Mathf.Min(rollSpeed * Time.deltaTime, remainingAngle);

            // 피벗 포인트를 기준으로 큐브 회전
            transform.RotateAround(pivotPoint, rotationAxis, rotationStep);

            // 남은 회전 각도를 갱신
            remainingAngle -= rotationStep;

            // 한 프레임 대기
            yield return null;
        }

        // 최종 위치를 큐브 크기에 맞게 정렬
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / cubeSize) * cubeSize,
            Mathf.Round(transform.position.y / cubeSize) * cubeSize,
            Mathf.Round(transform.position.z / cubeSize) * cubeSize
        );

        // 최종 회전값을 90도 단위로 정렬
        transform.rotation = Quaternion.Euler(
            Mathf.Round(transform.rotation.eulerAngles.x / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90
        );

        // 회전 상태를 해제하여 입력을 다시 받을 수 있도록 설정
        isRolling = false;
    }

    // Move 액션이 취소될 때 호출 이동 입력 값을 초기화
    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        // 이동 입력 값을 초기화
        moveInput = Vector2.zero;
    }

    // Jump 액션이 수행될 때 호출
    private void OnJumpAction(InputAction.CallbackContext obj)
    {
        // 점프 동작 처리 (현재 디버그 메시지 출력)
        Debug.Log("Jump");
    }

    //  Attack 액션이 수행될 때 호출
    private void OnAttackAction(InputAction.CallbackContext obj)
    {
        // 공격 동작 처리 (현재 디버그 메시지 출력)
        Debug.Log("Attack");
    }

    void Start()
    {
        // 초기화 또는 시작 설정이 필요하면 여기에 작성
    }

    void Update()
    {
        // 프레임 단위로 처리해야 할 로직 작성 가능
    }
}
