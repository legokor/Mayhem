using UnityEngine;

/// <summary>
/// Some helpers that doesn't fit anywhere.
/// </summary>
public static class Utilities {
    public static Quaternion ForwardRot = Quaternion.Euler(0, 0, 0), Backwards = Quaternion.Euler(0, 180, 0);

    /// <summary>
    /// Draws an unfilled rectangle.
    /// </summary>
    /// <param name="Left">Left margin</param>
    /// <param name="Top">Top margin</param>
    /// <param name="Width">Width</param>
    /// <param name="Height">Height</param>
    /// <param name="Thickness">Thickness</param>
    /// <param name="Tex">Texture</param>
    public static void GUIRectangle(int Left, int Top, int Width, int Height, int Thickness, Texture2D Tex) {
        GUI.DrawTexture(new Rect(Left, Top, Width, Thickness), Tex);
        GUI.DrawTexture(new Rect(Left, Top, Thickness, Height), Tex);
        GUI.DrawTexture(new Rect(Left, Top + Height - Thickness, Width, Thickness), Tex);
        GUI.DrawTexture(new Rect(Left + Width - Thickness, Top, Thickness, Height), Tex);
    }
}