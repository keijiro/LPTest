using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    [Space]
    [SerializeField] UIDocument _ui = null;
    [SerializeField] PaydirtManager _paydirtManager = null;
    [SerializeField] ScoopController _scoopController = null;
    [SerializeField] TrayController _trayController = null;
    [SerializeField] ItemDetector _itemDetector = null;
    [Space]
    [SerializeField] Animation _bucketAnimation = null;
    [SerializeField] float _bucketCloseWait = 2;

    Button _flushButton;

    async Awaitable StartInjection()
    {
        _paydirtManager.RequestInjection();

        var spawner = GetComponent<ItemSpawner>();
        spawner.StartSpawnBombs(2, 2).Forget();
        spawner.StartSpawnGems(0, 3, 2).Forget();
        spawner.StartSpawnGems(1, 3, 2).Forget();

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

        _trayController.SpawnCandid(0);
        _trayController.MoveIn();
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

            _trayController.MoveOut();
            await Awaitable.WaitForSecondsAsync(1);

            var item = _itemDetector.DetectedItem;
            if (item != null) Destroy(item.gameObject);

            _trayController.DestroyCandid();
            _itemDetector.ResetDetection();

            _trayController.SpawnCandid(Random.Range(0, 2));
            _trayController.MoveIn();
            await Awaitable.WaitForSecondsAsync(1);
        }
    }
}
