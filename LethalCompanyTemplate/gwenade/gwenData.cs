using UnityEngine;

[CreateAssetMenu(fileName = "gwenData", menuName = "gwenData")]
public class gwenData : ScriptableObject
{
    [SerializeField] public string t_name;
    [SerializeField] public AnimationCurve grenadeFallCurve;
    [SerializeField] public AnimationCurve grenadeVerticleFallCruve;
    [SerializeField] public AnimationCurve grenadeVerticalFallCruveNoBounce;
    [SerializeField] public AudioClip[] clips;
}
