using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public Vector3 velocity;
    public Vector3 cameraOffset;
    public bool isAttack;
    public Rigidbody body;

    public InventoryData inventory;

    // Start is called before the first frame update
    void Start()
    {
        DataManager dm = DataManager.Instance; 

        cameraOffset = Camera.main.transform.position - transform.position;
        body = GetComponent<Rigidbody>();

        Item axe = new Item("axe", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/axe"));
        Item axe1 = new Item("axe", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/axe"));
        Item axe2 = new Item("axe", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/axe"));
        Item boots = new Item("boots", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/boots"));
        Item boots1 = new Item("boots", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/boots"));
        Item boots2 = new Item("boots", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/boots"));

        inventory = new InventoryData(new Vector2Int(10, 1));
        inventory.AddItem(axe);
        inventory.AddItem(axe1);
        inventory.AddItem(axe2);
        inventory.AddItem(boots);
        inventory.AddItem(boots1);
        inventory.AddItem(boots2);

        PlayerInventory.Instance.Owner = inventory;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity.x += -1f;
            velocity.z += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity.x += 1f;
            velocity.z += -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x += -1f;
            velocity.z += -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += 1f;
            velocity.z += 1f;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CookInterface.ToggleCookInventory();
        }

        if (Input.GetKey(KeyCode.Mouse0) 
            && !EventSystem.current.IsPointerOverGameObject() 
            && !MouseInventory.Instance.IsDrag)
        {
            isAttack = true;
        }
        else
        {
            //애니메이션 결과 반영후 작업
            isAttack = false;
        }

        velocity.Normalize();

        if(!isAttack && velocity.magnitude > 0.1f)
        {
            Quaternion destLook = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, destLook, 7f * Time.deltaTime);

            body.MovePosition(transform.position + 3f * Time.deltaTime * velocity);
            transform.position += 3f * Time.deltaTime * velocity;
            GetComponentInChildren<Animator>().SetBool("IsMove", true);
            GetComponentInChildren<Animator>().SetBool("IsAttack", false);
        }
        else if(isAttack)
        {
            Quaternion destLook = Quaternion.LookRotation(velocity.magnitude > 0.01f ? velocity : transform.forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, destLook, 7f * Time.deltaTime);
            GetComponentInChildren<Animator>().SetBool("IsAttack", true);
            GetComponentInChildren<Animator>().SetBool("IsMove", false);
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("IsAttack", false);
            GetComponentInChildren<Animator>().SetBool("IsMove", false);
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + cameraOffset, 5f * Time.deltaTime);
    }
}
