using UnityEngine;
using System;

using Cavern;
using LeapVR;

using Helpers;
using Menus;
using Menus.Customization;
using Pickups;
using Weapons;

[AddComponentMenu("Entities / Player")]
public class PlayerEntity : Singleton<PlayerEntity> {
    [Header("Resources")]
    [Tooltip("Experience drop.")]
    public GameObject XPPickupObject;
    public AudioClip AudioPhoton, AudioScatter, AudioBeam;
    public GameObject ProjectileEntity, BeamEntity, PlayerBody, DeathEffect;
    [Header("Settings")]
    [Tooltip("Fire automatically.")]
    public bool AutoFire = true;
    [Header("References")]
    public GameObject GameOverScreen;
    [Tooltip("Map rotator to destroy at death.")]
    public ObjectRotator Rotator;

    AudioSource3D Source;
    WeaponBase Weapon;

    float Health = 100,
        MovePos = -35,
        SidePos = 0,
        SinceSpawn = 3;

    int Lives = 3, Score = -50;

    Texture2D GUIColor, GUITransparency;

    const float AreaWidth = 160, AreaHeight = 110, AreaWMax = AreaWidth * .5f, AreaWMin = -AreaWMax, AreaHMax = AreaHeight * .5f, AreaHMin = -AreaHMax;

    public void AwardScore(int Score) {
        this.Score += Score;
    }

    public void AwardExperience() {
        Weapon.AddExperience();
        Score += 25;
    }

    public void PlaySound(AudioClip Sound, float Volume = -1, bool Static = false) {
        Source.PlayOneShot(Sound, Volume == -1 ? Source.Volume : Volume, Static);
    }

    public void WeaponPickup(WeaponKinds Kind) {
        if (Weapon.Kind != Kind) {
            // Set new weapon
            int Level = Math.Max(1, Weapon.Level - 1);
            Destroy(Weapon);
            (Weapon = WeaponBase.AttachWeapon(Kind, gameObject)).Level = Level;
            // Repaint GUI
            if (GUIColor)
                Destroy(GUIColor);
            if (GUITransparency)
                Destroy(GUITransparency);
            GUIColor = new Texture2D(1, 1);
            GUIColor.SetPixel(0, 0, Weapon.DisplayColor);
            GUIColor.Apply();
            GUITransparency = new Texture2D(1, 1);
            GUITransparency.SetPixel(0, 0, new Color(Weapon.DisplayColor.r, Weapon.DisplayColor.g, Weapon.DisplayColor.b, .5f));
            GUITransparency.Apply();
        } else
            Weapon.AddLevel();
        Score += 50;
    }

    void OnTriggerEnter(Collider col) {
        PickupBase Pickup = col.gameObject.GetComponent<PickupBase>();
        if (Pickup)
            Pickup.PickedUp();
        Projectile proj = col.gameObject.GetComponentInParent<Projectile>();
        if (proj && SinceSpawn >= 3) {
            Health -= proj.Damage - (proj.WeaponKind == Weapon.Kind ? 1 : 0);
            if (Health <= 0) {
                Health += 100;
                Lives--;
                if (Lives == 0) {
                    Instantiate(DeathEffect, transform.position, transform.rotation);
                    PlayerBody.SetActive(false);
                    Weapon.Firing = false;
                    MapHandler.Instance.enabled = false;
                }
                SinceSpawn = 0;
            }
            Destroy(col.gameObject);
        }
    }

    void Awake() {
        Weapon = WeaponBase.AttachWeapon(WeaponKinds.Unassigned, gameObject);
        WeaponPickup(WeaponKinds.Photon);
        Source = GetComponent<AudioSource3D>();
        Customize.DeserializeTo(transform.GetChild(0).gameObject);
    }

    void OnGUI() {
        if (Lives == 0) // Game over
            return;
        // Weapon and health bars
        GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
        GUI.skin.label.fontSize = Screen.height / 32;
        int Margin = Screen.height / 20, BarLeft = Screen.width - 8 * Margin, BarTop = Screen.height - 2 * Margin, BarWidth = 7 * Margin,
        Thickness = Screen.height / 200, MpT = Margin + Thickness;
        Utilities.GUIRectangle(Margin, BarTop, BarWidth, Margin, Thickness, GUIColor);
        Utilities.GUIRectangle(BarLeft, BarTop, BarWidth, Margin, Thickness, GUIColor);
        BarWidth -= 2 * Thickness;
        SBS.StereoTexture(new Rect(MpT, BarTop, Weapon.Level == 5 ? BarWidth : (BarWidth * Weapon.XP / 25), Margin), GUITransparency);
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        SBS.StereoLabel(new Rect(MpT + 10, BarTop, BarWidth, Margin), "LEVEL " + Weapon.Level + " " + Weapon.DisplayName);
        SBS.StereoTexture(new Rect(BarLeft + Thickness, BarTop, BarWidth * Health / 100, Margin), GUITransparency);
        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        SBS.StereoLabel(new Rect(BarLeft - 10, BarTop, BarWidth, Margin), "LIVES: " + Lives);
        // Score
        GUI.skin.label.fontSize *= 2;
        GUI.skin.label.alignment = TextAnchor.LowerCenter;
        SBS.StereoLabel(new Rect(0, 0, Screen.width, BarTop + Margin), Score.ToString());
    }

    void Update() {
        if (Time.deltaTime == 0)
            return;
        if (Lives == 0) {
            if (!GameOverScreen.activeInHierarchy) {
                GameOverScreen.SetActive(true);
                GameOverScreen.GetComponent<GameOverMenu>().DisplayScore(Score);
                if (Rotator)
                    Destroy(Rotator);
            }
            return;
        }
        Weapon.Firing = AutoFire; // Firing to default
        Health = Mathf.Min(Health + Time.deltaTime * 3f, 100); // Refill health
        // Flashing after death
        SinceSpawn += Time.deltaTime;
        bool ShowBody = SinceSpawn < 3 ? Mathf.FloorToInt(SinceSpawn * 4) % 2 == 0 : true;
        if (ShowBody != PlayerBody.activeSelf)
            PlayerBody.SetActive(ShowBody);
        // Movement
        Vector2 LeapPosition = LeapMotion.Instance.PalmOnViewportXZ();
        float HorizontalMovement;
        if (LeapPosition != LeapMotion.NotAvailable) {
            MovePos = (.5f - LeapPosition.y) * AreaHeight;
            float OldSidePos = SidePos;
            SidePos = (LeapPosition.x - .5f) * AreaWidth;
            HorizontalMovement = Mathf.Clamp(SidePos - OldSidePos, -1, 1);
            Weapon.Firing = true;
        } else {
            MovePos = Mathf.Clamp(MovePos + ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1 : 0) -
                (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? 1 : 0)) * Time.deltaTime * 100f, AreaHMin, AreaHMax);
            HorizontalMovement = ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1 : 0) -
                (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? 1 : 0));
            SidePos = Mathf.Clamp(SidePos + HorizontalMovement * Time.deltaTime * 100f, AreaWMin, AreaWMax);
            Weapon.Firing |= Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        }
        transform.position = new Vector3(SidePos, 25, MovePos + MapHandler.Instance.MapPos);
        transform.rotation = Quaternion.Euler(0, 0, -HorizontalMovement * 15f);
    }
}