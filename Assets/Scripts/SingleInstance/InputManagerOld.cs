using UnityEngine;


public class InputManagerOld : MonoBehaviour
{
    public enum Device
    {
        Touch = 0,
        PC = 1,
        GamepadDualShock = 2,
        GamepadXboxOne = 3,
        GamepadXbox360 = 4,
        GamepadNintendoSwitch = 5,
        Custom = 6
    }

    public Device inputDevice;
    

    [Header("Read-only")] public Vector3 move;
    public float moveInputSpeedModificator;
    public Vector3 look;

    public bool isCrouchPressed;
    public bool isJumpPressed;
    public bool isInteractPressed;


    //TOUCH 
    /*public OffsetJoystick joystick;
    public LookPanel lookPanel;
    public ActionButton actionButton;
    public JumpButton jumpButton;*/

    void Start()
    {
        /*joystick = GameObject.Find("Offset Joystick Panel").GetComponent<OffsetJoystick>();
        lookPanel = GameObject.Find("Look Panel").GetComponent<LookPanel>();*/
    }

    void Update()
    {
        Control(inputDevice);
    }

    void Control(Device device)
    {
        Vector3 moveVector;
        Vector3 lookVector;
        float moveVertical = 0.0f, moveHorizontal = 0.0f;
        float lookVertical = 0.0f, lookHorizontal = 0.0f;

        switch (inputDevice)
        {
            //TODO: make touch control
            case Device.Touch:
            {
                //move = joystick.joystickValue;
                //look = lookPanel.deltaLook;
                /*moveHorizontal = joystick.joystickValue.x;
                moveVertical = joystick.joystickValue.y;

                lookHorizontal = lookPanel.deltaLook.x;
                lookVertical = lookPanel.deltaLook.y;
                isJumpPressed = jumpButton.touchedInThisFrame;

                moveInputSpeedModificator = joystick.joustickProgress;*/
                break;
            }
            case Device.PC:
            {
                moveVertical = Input.GetAxis("Vertical");
                moveHorizontal = Input.GetAxis("Horizontal");
                lookVertical = Input.GetAxis("Mouse Y");
                lookHorizontal = Input.GetAxis("Mouse X");
                if (Input.GetAxis("Jump") > 0.5f)
                    isJumpPressed = true;
                else
                    isJumpPressed = false;

                if (Input.GetAxis("Crouch") > 0.5f)
                    isCrouchPressed = true;
                else
                    isCrouchPressed = false;

                if (Input.GetAxis("Interact") > 0.5f)
                    isInteractPressed = true;
                else
                    isInteractPressed = false;

                moveInputSpeedModificator = 1.0f;
                break;
            }
            case Device.GamepadXbox360:
            {
                break;
            }
            //TODO: добавить поддержку геймпадов
        }


        move = new Vector3(moveHorizontal, moveVertical);
        look = new Vector3(lookHorizontal, lookVertical);
    }
}