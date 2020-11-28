using UnityEngine;

namespace WizardsCode.Controller
{
    [System.Serializable]
    public class FlyCameraController : MonoBehaviour
    {

        /*
            WASD/Arrows:    Movement
                      Q:    Drop
                      E:    Climb
                  Shift:    Move faster
                Control:    Move slower
            RMB + Mouse:    Rotate camera
                  Space:    Reset camera rotation so it is looking at the object stored in LookAt
        */

        public float cameraRotationSpeed = 90;
        public float climbSpeed = 4;
        public float normalMoveSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3;
        public GameObject lookAt;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        public GameObject LookAt
        {
            get { return lookAt; }
            set {
                lookAt = value;
                ResetRotation();
            }
        }

        private void Awake()
        {
            ResetRotation();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ResetRotation();
                return;
            }

            if (Input.GetMouseButton(1))
            {
                rotationX += Input.GetAxis("Mouse X") * cameraRotationSpeed * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * cameraRotationSpeed * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -90, 90);

                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
            }

            Vector3 pos = transform.position;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                pos += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                pos += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                pos += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                pos += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else
            {
                pos += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                pos += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.E)) { pos += transform.up * climbSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Q)) { pos -= transform.up * climbSpeed * Time.deltaTime; }

            RaycastHit hit;
            Vector3 direction;
            if (Input.GetAxis("Vertical") >= 0) {
               direction = transform.TransformDirection(Vector3.forward);
            }
            else
            {
                direction = transform.forward * normalMoveSpeed * Time.deltaTime;
            }

            if (!Physics.Raycast(transform.position, direction, out hit, 0.2f))
            {
                transform.position = pos;
            }
        }

        internal void ResetRotation()
        {
            if (lookAt != null)
            {
                transform.LookAt(lookAt.transform);
            }
            rotationX = this.transform.eulerAngles.y;
            rotationY = -this.transform.eulerAngles.x;
        }
    }
}