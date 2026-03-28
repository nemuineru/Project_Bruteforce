using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// Tracks how many consecutive hits the player lands and displays the count via TextMeshPro.
/// Plug into Status_MainUI (or any MonoBehaviour) by calling SetComponent(playerEntity).
///
/// Expansion hooks:
///   OnComboHit(int comboCount)  — fires every time a new hit is added
///   OnComboReset(int finalCount) — fires with the final count when the combo ends
/// Both are plain C# Actions, so subscribe from any other script:
///   comboShowing.OnComboHit += MyMethod;
/// </summary>
public class ComboShowing : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  Inspector
    // ------------------------------------------------------------------ //
    [Header("References")]
    [Tooltip("TextMeshProUGUI that displays the combo counter.")]
    public TextMeshProUGUI comboText;

    [Header("Settings")]
    [Tooltip("Seconds of no-hit silence before the combo resets.")]
    public float comboResetTime = 2f;

    [Tooltip("Minimum hits before the counter becomes visible.")]
    public int showThreshold = 2;

    // ------------------------------------------------------------------ //
    //  Public state  (read from other scripts as needed)
    // ------------------------------------------------------------------ //
    public int ComboCount { get; private set; }
    public bool IsComboActive { get; private set; }

    // ------------------------------------------------------------------ //
    //  Events  (subscribe to expand with your own systems)
    // ------------------------------------------------------------------ //

    /// <summary>Fired with the new combo count whenever a hit is registered.</summary>
    public System.Action<int> OnComboHit;

    /// <summary>Fired with the final count when an active combo ends.</summary>
    public System.Action<int> OnComboReset;

    // ------------------------------------------------------------------ //
    //  Private
    // ------------------------------------------------------------------ //
    Entity _player;
    gameState _gs;
    float _idleTimer;

    bool isShivering = false;


    //null at first, if registered, tries to find not same.
    //it resets at ResetCombo.
    hitDefParams atRegisteredParam;

    // Per-entity hit count cache to detect new hits each frame
    readonly Dictionary<Entity, int> _prevHitCounts = new();

    // ------------------------------------------------------------------ //
    //  Setup  (call from Status_MainUI or any initialiser)
    // ------------------------------------------------------------------ //
    public void SetComponent(Entity playerEntity)
    {
        _player = playerEntity;
        _gs = gameState.self;
        ResetCombo(silent: true);
    }

    // ------------------------------------------------------------------ //
    //  Unity loop
    // ------------------------------------------------------------------ //
    void Update()
    {
        if (_player == null || _gs == null) {
            
            Debug.Log("not finding player");
            _player = gameState.self.Player;
            _gs = gameState.self;
            return;
            }

        bool hitThisFrame = false;//CheckForNewHits();

        if (hitThisFrame)
        {
            _idleTimer = 0f;
            IsComboActive = true;
        }
        else if (IsComboActive)
        {
            _idleTimer += Time.deltaTime;
            isShivering = _idleTimer + 1.0f >= comboResetTime ? !isShivering : false;
            if (_idleTimer >= comboResetTime)
                ResetCombo();
        }

        RefreshUI();
    }

    // ------------------------------------------------------------------ //
    //  Hit detection
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Scans all living non-player entities for new hits whose owner is the player.
    /// Returns true if at least one new hit was found this frame.
    /// </summary>
    bool CheckForNewHits()
    {
        bool newHit = false;

        foreach (Entity entity in _gs.entityList)
        {
            // Skip the player itself and dead enemies
            if (entity == _player || !entity.attrs.alive) continue;

            int currentCount = CountPlayerHitsOn(entity);
            Debug.Log("calcCombos " + currentCount);

            if (!_prevHitCounts.TryGetValue(entity, out int prevCount))
            {
                _prevHitCounts[entity] = currentCount;
                continue;
            }

            if (currentCount > prevCount)
            {
                int delta = currentCount - prevCount;
                for (int i = 0; i < delta; i++)
                    RegisterHit();

                newHit = true;
            }

            _prevHitCounts[entity] = currentCount;
        }

        return newHit;
    }

    /// <summary>Counts registeredHitDefs on an entity where the player is the attacker.</summary>
    int CountPlayerHitsOn(Entity entity)
    {
        int n = ComboCount;
        List<hitDefParams> ownerDefs = entity.registeredHitDefs.FindAll(et => et.ownerEntity == _player);
        if(ownerDefs.Count != 0)
        {
            hitDefParams getLatest = ownerDefs.Last();
            //最新のRegisteredParamが同じ登録値でなければ.. 1ヒットとする.
            if(getLatest != atRegisteredParam)
            {
                foreach(hitDefParams hit in ownerDefs)
                {
                    atRegisteredParam = hit;
                }
                n++;
            }
        }
        // int _HitCount = 0;
        // foreach (hitDefParams hit in entity.registeredHitDefs)
        //     if (hit.ownerEntity == _player) n++;
        return n;
    }

    // ------------------------------------------------------------------ //
    //  Combo state helpers
    // ------------------------------------------------------------------ //

    void RegisterHit()
    {
        ComboCount++;
        OnComboHit?.Invoke(ComboCount);
    }

    /// <summary>
    /// Manually resets the combo.
    /// Call from outside (e.g. when the player is hit) if you want hard resets.
    /// </summary>
    public void ResetCombo(bool silent = false)
    {
        if (!silent && ComboCount > 0)
            OnComboReset?.Invoke(ComboCount);

        ComboCount = 0;
        _idleTimer = 0f;
        IsComboActive = false;
        _prevHitCounts.Clear();
        atRegisteredParam = null;
        RefreshUI();
    }

    // ------------------------------------------------------------------ //
    //  UI
    // ------------------------------------------------------------------ //

    void RefreshUI()
    {
        if (comboText == null) return;

        //コンボ消えかけなら消す..
        bool show = IsComboActive && ComboCount >= showThreshold && (isShivering == false);
        comboText.gameObject.SetActive(show);

        if (show){
            string x = 
            string.Format("<size=5em>{0}</size><line-height=120%>\n<size=2em>CHAIN!<line-height=30%>\n<size=0.7em>",ComboCount);
            comboText.text = x;
            comboText.color = Color.Lerp(Color.white,Color.gray, Mathf.Pow(_idleTimer / comboResetTime,0.75f));
            }
    }
}