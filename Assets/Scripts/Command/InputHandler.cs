using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    //Queue untuk menyimpan list command
    Queue<Command> commands = new Queue<Command>();

    void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>() ?? FindObjectOfType<PlayerMovement>();
        }
        if (playerShooting == null)
        {
            playerShooting = GetComponentInChildren<PlayerShooting>() ?? FindObjectOfType<PlayerShooting>();
        }
    }

    void FixedUpdate()
    {
        if (playerMovement == null) return;

        //Menghandle input movement
        Command moveCommand = InputMovementHandling();
        if (moveCommand != null)
        {
            commands.Enqueue(moveCommand);

            moveCommand.Execute();
        }
    }

    void Update()
    {
        if (playerShooting == null) return;

        // PlayerShooting owns sustained fire on mobile and applies its normal
        // timeBetweenBullets cooldown.  Calling Shoot here as well caused
        // rapid-fire whenever a joystick was touched.
        if (Application.isMobilePlatform) return;

        //Mengahndle shoot
        Command shootCommand = InputShootHandling();
        if (shootCommand != null)
        {
            shootCommand.Execute();
        }
    }

    Command InputMovementHandling()
    {
#if UNITY_ANDROID || UNITY_IOS
        return new MoveCommand(playerMovement, MobileInputTracker.MovementInput.x, MobileInputTracker.MovementInput.y);
#else
        //Check jika movement sesuai dengan key nya
        if (Input.GetKey(KeyCode.D))
        {
            return new MoveCommand(playerMovement, 1, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            return new MoveCommand(playerMovement, -1, 0);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            return new MoveCommand(playerMovement, 0, 1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            return new MoveCommand(playerMovement, 0, -1);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            //Undo movement
            return Undo();
        }
        else
        {
            return new MoveCommand(playerMovement, 0, 0);
        }
#endif
    }

    Command Undo()
    {
        //Jika Queue command tidak kosong, lakukan perintah undo
        if (commands.Count > 0)
        {
            Command undoCommand = commands.Dequeue();
            undoCommand.UnExecute();
        }
        return null;
    }

    Command InputShootHandling()
    {
        if (Input.touchCount > 0)
        {
            if (MobileInputTracker.IsShooting)
            {
                return new ShootCommand(playerShooting);
            }
        }
        else
        {
            if (MobileInputTracker.IsShooting || Input.GetButtonDown("Fire1"))
            {
                return new ShootCommand(playerShooting);
            }
        }
        return null;
    }
}
