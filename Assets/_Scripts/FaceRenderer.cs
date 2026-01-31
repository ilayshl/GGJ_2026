using UnityEngine;

public class FaceRenderer : MonoBehaviour
{
    [SerializeField] private Sprite normal, soapy, masked, bloody, bruised;
    [SerializeField] private Sprite maskedHalfPulledBloody, maskedHalfPulledNormal;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetNormal()
    {
        sr.sprite = normal;
    }

    public void SetSoapy()
    {
        sr.sprite = soapy;
    }

    public void SetMasked()
    {
        sr.sprite = masked;
    }

    public void SetBloody()
    {
        sr.sprite = bloody;
    }

    public void SetHalfMaskedBloody()
    {
        sr.sprite = maskedHalfPulledBloody;
    }

    public void SetHalfMaskedNormal()
    {
        sr.sprite = maskedHalfPulledNormal;
    }
}
