using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteractObjectAR : MonoBehaviour
{
    public float scaleSpeed = 0.1f; // Kecepatan skala
    public float minScale = 0.1f;   // Skala minimum
    public float maxScale = 3.0f;   // Skala maksimum
    public float rotationSpeed = 10f; // Kecepatan rotasi
    
    public bool rotationYAxis;
    public bool rotationXAxis;

    //default scale and rotasi
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private bool isScaling = false; // Apakah objek sedang dalam animasi perubahan skala
    public float scaleAmount = 0.5f; // Jumlah perubahan skala
    public float scaleFactor = 1.5f; // Faktor perubahan skala
    public float scaleDuration = 0.5f; // Durasi animasi perubahan skala

    public bool activeAudioPopUp;
    public AudioClip popUpAudioClip; // Audio yang akan diputar

    public bool simpleScreenGestureMode = false;

    private AudioSource audioSource; // Komponen AudioSource untuk memutar audio
    private void Start()
    {
        // Simpan skala dan rotasi awal saat dimulai
        initialScale = transform.localScale;
        initialRotation = transform.rotation;

        // Dapatkan komponen AudioSource
        audioSource = GetComponent<AudioSource>();

        // Tetapkan audioClip ke AudioSource
        audioSource.clip = popUpAudioClip;
    }

    void Update()
    {
        // Deteksi sentuhan pada objek
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Lakukan raycast untuk menemukan collision dengan objek
            if (Physics.Raycast(ray, out hit) && !simpleScreenGestureMode)
            {
                if (hit.transform == transform)
                {
                    InteractionGameobject(touch);
                }
                
            }
            else if(simpleScreenGestureMode) {
                InteractionGameobject(touch);
            }
        }
    }
    void InteractionGameobject(Touch touch)
    {
        // Objek yang tersentuh adalah objek ini
        // Lakukan kontrol skala hanya jika collision terjadi pada objek ini

        // Kontrol rotasi untuk perangkat mobile dengan dua jari (rotate gesture)
        if (Input.touchCount == 1 && touch.phase == TouchPhase.Moved)
        {
            // Dapatkan perubahan posisi sentuhan
            Vector2 deltaPosition = touch.deltaPosition;

            // Hitung rotasi berdasarkan perubahan posisi sentuhan
            float rotationX = deltaPosition.y * rotationSpeed * Mathf.Deg2Rad;
            float rotationY = -deltaPosition.x * rotationSpeed * Mathf.Deg2Rad;

            // Terapkan rotasi ke objek
            if (rotationXAxis) transform.Rotate(Vector3.right, rotationX);
            if (rotationYAxis) transform.Rotate(Vector3.up, rotationY);
        }


        // Kontrol skala untuk perangkat mobile dengan dua jari (pinch gesture)
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Dapatkan posisi masing-masing jari di frame ini dan frame sebelumnya
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Hitung jarak masing-masing frame (sebelumnya dan sekarang)
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Hitung perubahan jarak antara frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Ubah skala objek berdasarkan perubahan jarak
            float newScale = transform.localScale.x - deltaMagnitudeDiff * scaleSpeed;
            newScale = Mathf.Clamp(newScale, minScale, maxScale);
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    // Method untuk mereset skala dan rotasi ke nilai awal
    public void ResetObject()
    {
        transform.localScale = initialScale;
        transform.rotation = initialRotation;
    }

    //Method pop up
    public void PopUpScale()
    {
        if (activeAudioPopUp)
        {
            PlayAudioPopUp();
        }
        
        StartCoroutine(ScaleObject());
    }
    IEnumerator ScaleObject()
    {
        // Tandai bahwa objek sedang dalam proses animasi perubahan skala
        isScaling = true;

        // Perubahan skala
        Vector3 targetScale = transform.localScale * scaleFactor;
        while (transform.localScale.magnitude < targetScale.magnitude)
        {
            transform.localScale += Vector3.one * scaleAmount * Time.deltaTime;
            yield return null;
        }

        // Tunggu beberapa saat
        yield return new WaitForSeconds(scaleDuration);

        

        // Kembalikan ke ukuran asli
        transform.localScale = initialScale;

        // Hentikan proses animasi perubahan skala dan kembalikan ke nilai awal
        isScaling = false;
    }

    //audio
    // Method untuk memutar audio
    public void PlayAudioPopUp()
    {
        // Periksa apakah audioClip telah ditetapkan
        if (popUpAudioClip != null)
        {
            // Memutar audio
            audioSource.Play();

           
        }
        else
        {
            // Tampilkan pesan kesalahan jika audioClip belum ditetapkan
            Debug.Log("AudioClip belum ditetapkan!");
        }
    }
}
