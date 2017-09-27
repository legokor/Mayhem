using UnityEngine;
using System;

using Cavern;

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
    [Tooltip("Don't drag the player entity as fast as the user moves their hand.")]
    public bool LeapSpeedLimit = false;
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

    public void AwardScore(int Score) {
        this.Score += Score;
    }

    public void AwardExperience() {
        Weapon.AddExperience();
        Score += 25;
    }

    public void PlaySound(AudioClip Sound, float Volume = 1, bool Static = false) {
        Source.PlayOneShot(Sound, Volume, Static);
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
        Settings.LeapSetupXZ();
        Customize.DeserializeTo(transform.GetChild(0).gameObject);
    }

    void Start() {
        AudioListener3D.EnvironmentSize *= 10;
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
        GUI.DrawTexture(new Rect(MpT, BarTop, Weapon.Level == 5 ? BarWidth : (BarWidth * Weapon.XP / 25), Margin), GUITransparency);
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(MpT + 10, BarTop, BarWidth, Margin), "LEVEL " + Weapon.Level + " " + Weapon.DisplayName);
        GUI.DrawTexture(new Rect(BarLeft + Thickness, BarTop, BarWidth * Health / 100, Margin), GUITransparency);
        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        GUI.Label(new Rect(BarLeft - 10, BarTop, BarWidth, Margin), "LIVES: " + Lives);
        // Score
        GUI.skin.label.fontSize *= 2;
        GUI.skin.label.alignment = TextAnchor.LowerCenter;
        GUI.Label(new Rect(0, 0, Screen.width, BarTop + Margin), Score.ToString());
    }

    void Update() {
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
        Vector2 LeapTarget = LeapMotion.Instance.PalmOnScreenXZ();
        float VerticalMovement = 0, HorizontalMovement = 0;
        if (LeapTarget.x != -1 && LeapTarget.y != -1) {
            Vector3 CurrentPosition = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 LeapDirection = new Vector3(LeapTarget.x - CurrentPosition.x, (Screen.height - LeapTarget.y) - CurrentPosition.y);
            if (LeapSpeedLimit) {
                if (Math.Abs(LeapDirection.y) > Screen.width / 100f)
                    VerticalMovement = Math.Sign(LeapDirection.y);
                if (Math.Abs(LeapDirection.x) > Screen.height / 100f)
                    HorizontalMovement = Math.Sign(LeapDirection.x);
            } else {
                VerticalMovement = LeapDirection.y / 20f;
                HorizontalMovement = LeapDirection.x / 20f;
            }
            Weapon.Firing = true;
        } else {
            VerticalMovement = Convert.ToSingle(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) -
                Convert.ToSingle(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
            HorizontalMovement = ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1 : 0) -
                ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? 1 : 0);
            Weapon.Firing |= Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        }
        MovePos = Mathf.Clamp(MovePos + VerticalMovement * Time.deltaTime * 100f, -55, 55);
        SidePos = Mathf.Clamp(SidePos + HorizontalMovement * Time.deltaTime * 100f, -80, 80);
        transform.position = new Vector3(SidePos, 25, MovePos + MapHandler.Instance.MapPos);
        transform.rotation = Quaternion.Euler(0, 0, -HorizontalMovement * 15f);
    }
}