using UnityEngine;

namespace StickmanTrap.Menu
{
    /// <summary>Tuning for the menu Stickman. Kept in an asset so it is safe to tweak live.</summary>
    [CreateAssetMenu(menuName = "Stickman Trap/Menu Character Config", fileName = "MenuCharacterConfig")]
    public sealed class MenuCharacterConfig : ScriptableObject
    {
        [Header("Idle")]
        public float BreathAmplitude = 0.035f;
        public float BreathSpeed = 2f;
        public float LimbSwayDegrees = 4f;

        [Header("Random actions")]
        public bool EnableRandomActions = true;
        public Vector2 ActionInterval = new Vector2(3.5f, 7f);
        public bool EnableTrollGag = true;
        [Range(0f, 1f)] public float TrollGagChance = 0.28f;

        [Header("Idle jokes (no input)")]
        public bool EnableIdleJokes = true;
        public float IdleJokeMinTime = 12f;
        public float IdleJokeSecondLine = 26f;
        public float IdleJokeVisibleTime = 4f;
        [TextArea] public string IdleLineOne = "Are you going to play?";
        [TextArea] public string IdleLineTwo = "Still here?";
    }
}
