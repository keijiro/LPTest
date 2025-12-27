using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] PaydirtManager _paydirtManager = null;
    [Space]
    [SerializeField] Animation _bucketAnimation = null;
    [SerializeField] float _bucketCloseWait = 2;

    Button _flushButton;

    async void Start()
    {
        var root = _ui.rootVisualElement;
        _flushButton = root.Q<Button>("flush-button");
        _flushButton.clicked += OnFlushClicked;

        await Awaitable.WaitForSecondsAsync(0.1f);

        _paydirtManager.RequestInjection();
    }

    async void OnFlushClicked()
    {
        ConsoleManager.AddLine("Flush started.");

        _bucketAnimation.Play("HatchOpen");

        await Awaitable.WaitForSecondsAsync(_bucketCloseWait);

        _paydirtManager.RequestInjection();
        _bucketAnimation.Play("HatchClose");
    }
}
