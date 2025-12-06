using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 20f;
    private Vector3 dragOrigin;

    [Header("SINIRLAR (Kýrmýzý Kutuya Bak)")]
    public float minX = -50f;
    public float maxX = 1200f;
    public float minZ = -50f;
    public float maxZ = 50f;

    private Camera myCam;

    void Start()
    {
        myCam = GetComponent<Camera>();
    }

    void Update()
    {
        // UI Açýkken hareket etme
        if (UIManager.Instance != null && UIManager.Instance.IsUIOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Camera targetCam = (myCam != null) ? myCam : Camera.main;
        if (targetCam == null) return;

        Vector3 pos = targetCam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

        // Hedef pozisyonu hesapla
        Vector3 targetPosition = transform.position - move;

        // SINIRLAMA (Clamp)
        // Kamerayý aniden fýrlatmamasý için, clamp iþlemini yeni hedef pozisyona uygula
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        transform.position = targetPosition;
    }

    // BU KISIM SANA EDÝTÖRDE YARDIMCI OLACAK
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Sýnýrlarý temsil eden bir kutu çiz
        float width = maxX - minX;
        float depth = maxZ - minZ;
        float centerX = minX + (width / 2);
        float centerZ = minZ + (depth / 2);

        Vector3 center = new Vector3(centerX, transform.position.y, centerZ);
        Vector3 size = new Vector3(width, 10, depth);

        Gizmos.DrawWireCube(center, size);
    }
}