using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider sliderTopPart;
    [SerializeField] private Slider sliderBottomPart;
    [SerializeField] private GameObject darkTint;
    [SerializeField] private float slideDuration;

    [SerializeField] private RectTransform loadingTransform;
    [SerializeField] private Image dogHead;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float shadeDuration;

    [SerializeField] private RectTransform loadingMessageTransform;
    [SerializeField] private TextMeshProUGUI message;

    [SerializeField] private List<string> messageList;
    [SerializeField] private float messageStayDuration;
    [SerializeField] private float shortMessageStayDuration;
    [SerializeField] private float messageSlideDuration;
    [SerializeField] private Vector2 targetMessageOffset;

    private bool isOpen = true;
    private bool wasOpen = true;
    private bool slideInProgress;
    private float slideTimer;
    private bool slidingDown;
    private float shadeTimer;
    private enum MessageState { hidden, movingIn, stay, movingOut }
    private MessageState messageState;
    private float messageTimer;
    private int messageIndex;
    private static readonly Color Transparent = new Color(1, 1, 1, 0);
    public void SetUpOnAwake()
    {
        OpenSlider();
    }

    public void AdjustToSceneLoaded(Scene scene)
    {
        if (scene.buildIndex == 0)
            SetSliderOpen(true);
    }

    public void SetSliderOpen(bool open)
    {
        isOpen = open;
    }

    private void Update()
    {
        if (isOpen)
        {
            if (!wasOpen) OpenSlider();
        }
        else
        {
            if (wasOpen) CloseSlider();
            if (!slideInProgress) ProcessLoadingSign();
        }

        if (slideInProgress) ProcessSlide();
        wasOpen = isOpen;
    }

    private void OpenSlider()
    {
        slidingDown = false;
        slideInProgress = true;
        darkTint.SetActive(false);
        loadingTransform.gameObject.SetActive(false);
        message.gameObject.SetActive(false);
        slideTimer = 0;
        sliderTopPart.gameObject.SetActive(true);
        sliderBottomPart.gameObject.SetActive(true);
        sliderTopPart.value = 0.5f;
        sliderBottomPart.value = 0.5f;
    }

    private void CloseSlider()
    {
        slidingDown = true;
        slideInProgress = true;
        darkTint.SetActive(false);
        loadingTransform.gameObject.SetActive(false);
        slideTimer = 0;
        sliderTopPart.gameObject.SetActive(true);
        sliderBottomPart.gameObject.SetActive(true);
        sliderTopPart.value = 0;
        sliderBottomPart.value = 0;
    }

    private void ProcessSlide()
    {
        slideTimer += Time.deltaTime;
        float slidePercent = slideTimer / slideDuration;
        if (slidePercent > 1)
        {
            slideInProgress = false;
            darkTint.SetActive(slidingDown);
            sliderTopPart.gameObject.SetActive(false);
            sliderBottomPart.gameObject.SetActive(false);
            loadingTransform.gameObject.SetActive(slidingDown);
            if (slidingDown)
            {
                dogHead.color = Transparent;
                shadeTimer = 0;
                loadingTransform.localScale = Vector3.zero;
            }
            return;
        }

        if (!slidingDown) slidePercent = 1 - slidePercent;
        sliderTopPart.value = slidePercent / 2;
        sliderBottomPart.value = slidePercent / 2;
    }

    private void ProcessLoadingSign()
    {
        loadingTransform.eulerAngles = new Vector3(0, 0, loadingTransform.eulerAngles.z - rotationSpeed * Time.deltaTime);
        if (shadeTimer < shadeDuration)
        {
            shadeTimer += Time.deltaTime;
            if (shadeTimer > shadeDuration)
            {
                shadeTimer = shadeDuration;
                message.gameObject.SetActive(true);
                messageTimer = 0;
            }
            dogHead.color = new Color(1, 1, 1, shadeTimer / shadeDuration);
            loadingTransform.localScale = Vector3.one * shadeTimer / shadeDuration;
        }
        else ProcessLoadingMessage();
    }

    private void ProcessLoadingMessage()
    {
        switch (messageState)
        {
            case MessageState.hidden:
                SpawnMessage();
                break;
            case MessageState.movingIn:
                ProcessMessageMovingIn();
                break;
            case MessageState.stay:
                ProcessMessageStay();
                break;
            case MessageState.movingOut:
                ProcessMessageMovingOut();
                break;
        }
    }
    private void SpawnMessage()
    {
        message.text = messageList[messageIndex];
        messageIndex++;
        if (messageIndex == messageList.Count) messageIndex = 0;
        loadingMessageTransform.anchoredPosition = new Vector2(Screen.width / 2 + targetMessageOffset.x, targetMessageOffset.y);
        message.alpha = 0;
        messageState = MessageState.movingIn;
    }

    private void ProcessMessageMovingIn()
    {
        messageTimer += Time.deltaTime;
        float slideProgress = messageTimer / messageSlideDuration;
        if (slideProgress > 1) slideProgress = 1;
        loadingMessageTransform.anchoredPosition = new Vector2(Screen.width / 2 * (1 - slideProgress) + targetMessageOffset.x, targetMessageOffset.y);
        message.alpha = slideProgress;

        if (slideProgress == 1)
        {
            messageTimer = 0;
            messageState = MessageState.stay;
        }
    }

    private void ProcessMessageStay()
    {
        messageTimer += Time.deltaTime;
        float stayDuration = (messageIndex > 2) ? shortMessageStayDuration : messageStayDuration;
        if (messageTimer > stayDuration)
        {
            messageTimer = 0;
            messageState = MessageState.movingOut;
        }
    }
    private void ProcessMessageMovingOut()
    {
        messageTimer += Time.deltaTime;
        float slideProgress = 1 - (messageTimer / messageSlideDuration);
        if (slideProgress < 0) slideProgress = 0;
        loadingMessageTransform.anchoredPosition = new Vector2(-Screen.width / 2 * (1 - slideProgress) + targetMessageOffset.x, targetMessageOffset.y);
        message.alpha = slideProgress;

        if (slideProgress == 0)
        {
            messageTimer = 0;
            messageState = MessageState.hidden;
        }
    }
}
