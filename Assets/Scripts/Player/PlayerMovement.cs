using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;
    float camRayLength = 100f;

    private void Awake()
    {
        //mendapatkan nilai mask dari layer yang bernama Floor
        floorMask = LayerMask.GetMask("Floor");
        if (floorMask == 0)
        {
            floorMask = ~LayerMask.GetMask("Ignore Raycast");
        }

        //Mendapatkan komponen Animator
        anim = GetComponent<Animator>();

        //Mendapatkan komponen Rigidbody
        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            playerRigidbody = gameObject.AddComponent<Rigidbody>();
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }
    }

    private void Update()
    {
        MobileInputTracker.Update();
    }

    private void FixedUpdate()
    {
        float h = MobileInputTracker.MovementInput.x;
        float v = MobileInputTracker.MovementInput.y;

        // If no touch movement, fallback to keyboard
        if (h == 0f && v == 0f)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        Move(h, v);
        Turning();
        Animating(h, v);
    }

    public void Animating(float h, float v)
    {
        if (anim != null)
        {
            bool walking = h != 0f || v != 0f;
            anim.SetBool("IsWalking", walking);
        }
    }

    void Turning()
    {
        // Touch aim for mobile / touch devices
        if (MobileInputTracker.AimDirection != Vector3.zero && playerRigidbody != null)
        {
            Quaternion newRotation = Quaternion.LookRotation(MobileInputTracker.AimDirection);
            playerRigidbody.MoveRotation(newRotation);
            return;
        }

        // Mouse aim for desktop
        Camera cam = Camera.main ?? FindObjectOfType<Camera>();
        if (cam == null) return;

        //Buat Ray dari posisi mouse di layar
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        //Buat raycast untuk floorHit
        RaycastHit floorHit;

        if (floorMask == 0)
        {
            floorMask = LayerMask.GetMask("Floor");
            if (floorMask == 0) floorMask = ~LayerMask.GetMask("Ignore Raycast");
        }

        //Lakukan raycast
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            //Mendapatkan vector daro posisi player dan posisi floorHit
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            if (playerToMouse != Vector3.zero && playerRigidbody != null)
            {
                //Mendapatkan look rotation baru ke hit position
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

                //Rotasi player
                playerRigidbody.MoveRotation(newRotation);
            }
        }
    }

    //Method player dapat berjalan
    public void Move(float h, float v)
    {
        //Set nilai x dan y
        movement.Set(h, 0f, v);

        //Menormalisasi nilai vector agar total panjang dari vector adalah 1
        movement = movement.normalized * speed * Time.deltaTime;

        //Move to position
        if (playerRigidbody != null)
        {
            playerRigidbody.MovePosition(transform.position + movement);
        }
        else
        {
            transform.position += movement;
        }
    }

    // Method player berjalan lebih cepat
    public void SpeedUp()
    {
        speed = 10f;
        Invoke("ResetSpeed", 10);
    }

    // Mengembalikan ke speed semula
    public void ResetSpeed()
    {
        speed = 6f;
    }
}