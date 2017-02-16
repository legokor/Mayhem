using UnityEngine;

public static class Utilities {
    public static Quaternion ForwardRot = Quaternion.Euler(0, 0, 0), Backwards = Quaternion.Euler(0, 180, 0);

    public static void GUIRectangle(int Left, int Top, int Width, int Height, int Thickness, Texture2D Tex) {
        GUI.DrawTexture(new Rect(Left, Top, Width, Thickness), Tex);
        GUI.DrawTexture(new Rect(Left, Top, Thickness, Height), Tex);
        GUI.DrawTexture(new Rect(Left, Top + Height - Thickness, Width, Thickness), Tex);
        GUI.DrawTexture(new Rect(Left + Width - Thickness, Top, Thickness, Height), Tex);
    }
}