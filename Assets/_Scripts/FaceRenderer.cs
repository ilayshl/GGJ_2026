using UnityEngine;
using UnityEngine.UI;

public class FaceRenderer : MonoBehaviour
{
    [SerializeField] private Sprite normal, soapy, masked, bloody, bruised;
    [SerializeField] private Sprite maskedHalfPulledBloody, maskedHalfPulledNormal;

    private Image sr;

    void Awake()
    {
        sr = GetComponent<Image>();
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

    public void SetBruised()
    {
        sr.sprite = bruised;
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
