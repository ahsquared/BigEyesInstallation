using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowInstructions : MonoBehaviour {

    public GameObject InstructionsImage;
    private Image _instructionsImage;
    private float _fadeInTime = 1;
    private float _fadeOutTime = 2;
    private float _fadeDelay = 5;

    // Use this for initialization
    void Start()
    {
        if (!InstructionsImage) InstructionsImage = Helpers.GetChildGameObjectByName(gameObject, "InstructionsImage");
        _instructionsImage = InstructionsImage.GetComponent<Image>();
        FadeOutInstructions();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeInInstructions()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", _fadeInTime, "onUpdate", "SetInstructionsAlpha", "onComplete", "FadeOutInstructions"));
    }

    public void FadeOutInstructions()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("delay", _fadeDelay,"from", 1, "to", 0, "time", _fadeOutTime, "onUpdate", "SetInstructionsAlpha"));

    }

    public void HideInstructions()
    {
        iTween.Stop(gameObject);
        InstructionsImage.GetComponent<CanvasRenderer>().SetAlpha(0f);
    }

    private void SetInstructionsAlpha(float alpha)
    {
        InstructionsImage.GetComponent<CanvasRenderer>().SetAlpha(alpha);
    }
}
