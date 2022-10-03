using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraPos;
    [SerializeField] private LayerMask whatIsAGridLayer;
    [SerializeField] private GameLogic _gameLogic;
    
    private static float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {

        GridCell cellisMouseOver = isMouseOver();
        if (cellisMouseOver != null)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (cellisMouseOver.CanPlayerSelect())
                {
                    _gameLogic.PlayerMoved(cellisMouseOver);
                }
                
                cellisMouseOver.Select();
                
            }
        }


        processKeyInput();

    }


    private void processKeyInput()
    {
        if (Input.GetKey("up"))
        {
            cameraPos.transform.Translate(new Vector3(0.0f, 0.0f, speed));
        }

        if (Input.GetKey("down"))
        {
            cameraPos.transform.Translate(new Vector3(0.0f, 0.0f , -speed));
        }

        if (Input.GetKey("left"))
        {
            cameraPos.transform.Translate(new Vector3(-speed, 0.0f, 0.0f));

        }

        if (Input.GetKey("right"))
        {
            cameraPos.transform.Translate(new Vector3(speed, 0.0f, 0.0f));
        }


        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            cameraPos.transform.Translate(new Vector3(0.0f, -speed, 0.0f));
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            cameraPos.transform.Translate(new Vector3(0.0f, speed, 0.0f));
        }

    }

    private GridCell isMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, whatIsAGridLayer))
        {
            return hitInfo.transform.GetComponent<GridCell>();
        }

        return null;
    }
}
