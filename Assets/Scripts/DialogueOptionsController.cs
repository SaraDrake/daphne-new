using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogueOptionsController : MonoBehaviour
{
    public List<DialogueButtonController> Buttons;
    public DialogueButtonController QuestionDisplay;
    public UnityEngine.UI.Image Background;

    public float FourButtonsHeight;
    public float ThreeButtonsHeight;
    public float TwoButtonsHeight;
    public float OneButtonsHeight;

    public float ExpandDuration;
    public float ContractDuration;

    private Tween backgroundTween;
    public Transform defaultQuestionPosition;

    public Color AskMeBackgroundColor;
    public Color QuestionBackgroundColor;
    public Color ChoicesBackgroundColor;
    public Color AskMeTextColor;
    public Color QuestionTextColor;
    public Color ChoicesTextColor;

    public bool Busy()
    {
        if (backgroundTween == null)
        {
            return false;
        }
        else if(backgroundTween.IsActive())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator SetupQuestionDisplay(int num, string text)
    {
        QuestionColorTweens();
        yield return new WaitForSeconds(.3f);

        backgroundTween = Background.rectTransform.DOSizeDelta(new Vector2(Background.rectTransform.rect.width, OneButtonsHeight), ContractDuration);

        QuestionDisplay.gameObject.SetActive(true);
        QuestionDisplay.transform.position = Buttons[num].transform.position;
        QuestionDisplay.Text.text = text;
        QuestionDisplay.transform.DOMoveY(defaultQuestionPosition.transform.position.y, ContractDuration);
        

        yield return null;
    }

    public void SpreadToAllButtons(int i)
    {
        QuestionDisplay.gameObject.SetActive(false);
        switch (i)
        {
            case 1:
                backgroundTween = Background.rectTransform.DOSizeDelta(new Vector2(Background.rectTransform.rect.width, OneButtonsHeight), ExpandDuration);
                FadeInText(0);
                AskMeColorTweens();
                break;
            case 2:
                backgroundTween = Background.rectTransform.DOSizeDelta(new Vector2(Background.rectTransform.rect.width, TwoButtonsHeight), ExpandDuration);
                FadeInText(0);
                FadeInText(1);
                ChoicesColorTweens();
                break;
            case 3:
                backgroundTween = Background.rectTransform.DOSizeDelta(new Vector2(Background.rectTransform.rect.width, ThreeButtonsHeight), ExpandDuration);
                FadeInText(0);
                FadeInText(1);
                FadeInText(2);
                ChoicesColorTweens();
                break;
            case 4:
                backgroundTween = Background.rectTransform.DOSizeDelta(new Vector2(Background.rectTransform.rect.width, FourButtonsHeight), ExpandDuration);
                FadeInText(0);
                FadeInText(1);
                FadeInText(2);
                FadeInText(3);
                ChoicesColorTweens();
                break;
            default:
                backgroundTween = null;
                ChoicesColorTweens();
                break;
        }

    }

    void AskMeColorTweens()
    {
        Background.DOColor(AskMeBackgroundColor, 1f);
        Buttons[0].Text.DOColor(AskMeTextColor, 1f);
    }

    void ChoicesColorTweens()
    {
        Sequence temp = DOTween.Sequence();
        temp.Append(Background.DOColor(Color.white, .4f));
        temp.Append(Background.DOColor(ChoicesBackgroundColor, .6f));
        temp.Play(); 

        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].Alpha = 1f;
            Buttons[i].Text.DOColor(ChoicesTextColor, 1f);
        }
    }

    void QuestionColorTweens()
    {
        QuestionDisplay.BackgroundActive(true);
        QuestionDisplay.Background.color = QuestionBackgroundColor;
        Background.DOColor(Color.black, 2f).SetDelay(.3f);
        Background.DOFade(0f, 2f).SetDelay(.3f);
        QuestionDisplay.Text.DOColor(QuestionTextColor, 1f).SetDelay(.3f);
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].Text.color = Color.white;
            //Buttons[i].Cgroup.DOFade(0f, .3f);
        }
    }

    private void FadeInText(int buttonNumber)
    {
        Buttons[buttonNumber].Alpha = 0;
        DOTween.To(() => Buttons[buttonNumber].Alpha, x => Buttons[buttonNumber].Alpha = x, 1f, .6f).SetDelay(.2f);
    }

    public void MoveToQuestionSelected(int i)
    {
        StartCoroutine(SetupQuestionDisplay(i, Buttons[i].Text.text));
    }
}
