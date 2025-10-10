using UnityEngine;

public abstract class RevealerShapeAsset : ScriptableObject
{
    /// <summary>Evaluate weight in [0..1] for a given local point (shape space). 'forward' is provided if needed.</summary>
    public abstract float Evaluate(Vector2 localPoint, Vector2 forward);


    /// <summary>Local-space bounding box (used for tiling range). Keep it tight.</summary>
    public abstract Bounds GetLocalBounds();
}