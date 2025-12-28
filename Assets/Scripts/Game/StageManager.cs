using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    [Space]
    [SerializeField] UIDocument _ui = null;
    [SerializeField] PaydirtManager _paydirtManager = null;
    [SerializeField] ScoopController _scoopController = null;
    [SerializeField] TrayController _trayPrefab = null;
    [SerializeField] ItemDetector _itemDetector = null;
    [Space]
    [SerializeField] Animation _bucketAnimation = null;
    [SerializeField] float _bucketCloseWait = 2;

    Button _flushButton;
    TrayController _tray;

    async Awaitable StartInjection()
    {
        _paydirtManager.RequestInjection();

        var spawner = GetComponent<ItemSpawner>();
        spawner.StartSpawnBombs(2, 2).Forget();
        spawner.StartSpawnGems(6, 2).Forget();

        await Awaitable.WaitForSecondsAsync(2);

        _scoopController.SpawnScoopInstance();
    }

    async void Start()
    {
        var root = _ui.rootVisualElement;
        _flushButton = root.Q<Button>("flush-button");
        _flushButton.clicked += OnFlushClicked;

        RunItemDetectionLoop().Forget();

        await Awaitable.WaitForSecondsAsync(0.1f);

        StartInjection().Forget();

        await Awaitable.WaitForSecondsAsync(1);

        _tray = Instantiate(_trayPrefab);
    }

    async void OnFlushClicked()
    {
        _bucketAnimation.Play("HatchOpen");
        _scoopController.ThrowScoopInstance();

        await Awaitable.WaitForSecondsAsync(_bucketCloseWait);

        StartInjection().Forget();
        _bucketAnimation.Play("HatchClose");
    }

    async Awaitable RunItemDetectionLoop()
    {
        while (true)
        {
            await Awaitable.FixedUpdateAsync();

            if (_itemDetector.DetectedItem == null) continue;
            Debug.Log("Item Detected");

            _tray.StartExit();
            await Awaitable.WaitForSecondsAsync(1);
            Destroy(_tray.gameObject);

            await Awaitable.WaitForSecondsAsync(1);

            var item = _itemDetector.DetectedItem;
            if (item != null) Destroy(item.gameObject);

            _itemDetector.ResetDetection();

            _tray = Instantiate(_trayPrefab);
            await Awaitable.WaitForSecondsAsync(1);
        }
    }
}
