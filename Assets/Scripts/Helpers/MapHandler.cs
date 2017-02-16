using UnityEngine;
using System.Collections.Generic;

using Enemies;

namespace Helpers {
    /// <summary>
    /// Handles map scrolling and spawning of environment and enemies. Attach to the ground.
    /// </summary>
    [AddComponentMenu("Helpers / Map Handler")]
    class MapHandler : Singleton<MapHandler> {
        [Tooltip("Map scrolling speed.")]
        public float ScrollingSpeed = 25;
        [Tooltip("Objects to spawn on the ground.")]
        public GameObject[] GroundObjects;
        [Tooltip("The land size for each ground object. Must be divided by 5.")]
        public Vector2[] GroundObjectSizes;
        [Tooltip("How many turrets should be spawned on each land block.")]
        public int TurretsPerBlock = 1;
        [Tooltip("Vegetation objects.")]
        public GameObject[] Vegetation;
        [Tooltip("Enemy figter spawn pool.")]
        public GameObject[] EnemyFighters;
        [Tooltip("This many fighters to spawn per wave.")]
        public int FightersPerWave = 20;
        [Tooltip("Dronw spawn. Don't set if you don't want drones on this level.")]
        public GameObject EnemyDrone;
        [Tooltip("This many drones to spawn per wave.")]
        public int DronesPerWave = 6;
        [Tooltip("Time between creating new waves.")]
        public float WaveCountdown = 8;

        /// <summary>
        /// Scrolled distance.
        /// </summary>
        [System.NonSerialized] public float MapPos = 0;

        /// <summary>
        /// Previous environment filling.
        /// </summary>
        bool[,] LastFilling = new bool[40, 30];
        /// <summary>
        /// Current spawn block.
        /// </summary>
        int LastGroundStep = -1;
        /// <summary>
        /// Spawn next wave of enemies after this time (seconds).
        /// </summary>
        float NextWave = 3;
        /// <summary>
        /// Difficulty scale.
        /// </summary>
        float Difficulty = 0;

        /// <summary>
        /// The damage of an enemy on the current difficulty.
        /// </summary>
        public int EnemyDamage {
            get {
                return 3 + Mathf.FloorToInt(Difficulty / 2f);
            }
        }
        /// <summary>
        /// The health of an enemy on the current difficulty.
        /// </summary>
        public int EnemyHealth {
            get {
                return 10 + Mathf.FloorToInt(Difficulty);
            }
        }

        /// <summary>
        /// Is space available for a new object in a given layout?
        /// </summary>
        /// <param name="Sizes">The size of the object that needs to be placed</param>
        /// <param name="Filling">Current layout</param>
        /// <returns>If the object can be placed</returns>
        bool Fits(Vector2 Sizes, bool[,] Filling) {
            Sizes = new Vector2(Sizes.x / 5, Sizes.y / 5);
            for (int i = 0; i < 40 - Sizes.x; i++) {
                for (int j = 0; j < 30 - Sizes.y; j++) {
                    bool FitsHere = true;
                    for (int x = i; x < i + Sizes.x; x++)
                        for (int y = j; y < j + Sizes.y; y++)
                            if (Filling[x, y])
                                FitsHere = false;
                    if (FitsHere)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Give the player score for a kill.
        /// </summary>
        public void AwardKillScore() {
            PlayerEntity.Instance.AwardScore(100 + Mathf.FloorToInt(Difficulty));
        }

        /// <summary>
        /// Scroll and spawn environment and enemies each frame.
        /// </summary>
        void Update() {
            MapPos += ScrollingSpeed * Time.deltaTime; // Scrolling
            Camera.main.transform.position = new Vector3(0, 150, MapPos); // Camera movement
            transform.position = new Vector3(0, 0, Mathf.Round(MapPos / 100) * 100); // Drag the ground along, don't spawn new lands
            Difficulty += Time.deltaTime / 10f; // Increase difficulty
            // Environment spawning
            int GroundStep = (int)(MapPos / 100);
            while (LastGroundStep != GroundStep) {
                ++LastGroundStep;
                float SpawnDistance = GroundStep * 100 + 100;
                // Inner area
                bool NoneFits = false;
                bool[,] Filling = new bool[40, 30];
                for (int OverlappingRow = 0; OverlappingRow < 10; ++OverlappingRow)
                    for (int Column = 0; Column < 40; ++Column)
                        Filling[Column, OverlappingRow] = LastFilling[Column, OverlappingRow + 20];
                int TurretsRemaining = 1;
                while (!NoneFits) {
                    NoneFits = true;
                    int Available = 0;
                    bool[] Fitting = new bool[GroundObjects.Length];
                    for (int i = 0; i < Fitting.Length; i++) {
                        Fitting[i] = !(GroundObjects[i].name == "Turret" && TurretsRemaining == 0) && Fits(GroundObjectSizes[i], Filling);
                        if (Fitting[i]) {
                            NoneFits = false;
                            Available++;
                        }
                    }
                    if (!NoneFits) {
                        int Place = Random.Range(0, Available) + 1, ObjID = -1;
                        while (Place > 0) {
                            ObjID = (ObjID + 1) % GroundObjects.Length;
                            if (Fitting[ObjID])
                                --Place;
                        }
                        List<Vector2> PlausiblePlaces = new List<Vector2>();
                        int PlaceCount = 0;
                        Vector2 Sizes = new Vector2(GroundObjectSizes[ObjID].x / 5, GroundObjectSizes[ObjID].y / 5);
                        for (int i = 0; i < 40 - Sizes.x; i++) {
                            for (int j = 0; j < 30 - Sizes.y; j++) {
                                bool FitsHere = true;
                                for (int x = i; x < i + Sizes.x; x++)
                                    for (int y = j; y < j + Sizes.y; y++)
                                        if (Filling[x, y])
                                            FitsHere = false;
                                if (FitsHere) {
                                    PlausiblePlaces.Add(new Vector2(i, j));
                                    PlaceCount++;
                                }
                            }
                        }
                        int Placing = Random.Range(0, PlaceCount);
                        Instantiate(GroundObjects[ObjID], new Vector3(PlausiblePlaces[Placing].x * 5 + GroundObjectSizes[ObjID].x / 2 - 100, 0,
                            PlausiblePlaces[Placing].y * 5 + GroundObjectSizes[ObjID].y / 2 + SpawnDistance), Random.value < .5f ? Utilities.ForwardRot : Utilities.Backwards);
                        for (int i = (int)PlausiblePlaces[Placing].x; i < (int)(PlausiblePlaces[Placing].x + Sizes.x); i++)
                            for (int j = (int)PlausiblePlaces[Placing].y; j < (int)(PlausiblePlaces[Placing].y + Sizes.y); j++)
                                Filling[i, j] = true;
                        if (GroundObjects[ObjID].name == "Turret")
                            TurretsRemaining--;
                    }
                }
                LastFilling = Filling;
                // Vegetation
                if (Vegetation != null && Vegetation.Length != 0) {
                    for (int i = 0; i < 50; ++i) {
                        float WidthPos = Random.Range(-60f, 60f);
                        Instantiate(Vegetation[Random.Range(0, Vegetation.Length)], new Vector3(WidthPos + Mathf.Sign(WidthPos) * 100, 0, SpawnDistance + Random.value * 100),
                            Quaternion.Euler(0, Random.value * 360, 0));
                    }
                }
            }
            // Create enemies
            NextWave -= Time.deltaTime;
            if (NextWave <= 0) {
                if (EnemyFighters.Length != 0) { // Fighters
                    int Distance = 100;
                    int WaveKind = Random.Range(0, 5);
                    GameObject FighterSpawn = EnemyFighters[Random.Range(0, EnemyFighters.Length)];
                    for (int i = 0; i < FightersPerWave; i++) {
                        Distance += 10;
                        switch (WaveKind) {
                            case 0: // Group from left
                                Instantiate(FighterSpawn, new Vector3(-Distance, 25, MapPos + Distance), Utilities.Backwards).GetComponent<Fighter>().Movement.x = -35;
                                break;
                            case 1: // Group from right
                                Instantiate(FighterSpawn, new Vector3(Distance, 25, MapPos + Distance), Utilities.Backwards).GetComponent<Fighter>().Movement.x = 35;
                                break;
                            default: // Random formation with the highest chance
                                Instantiate(FighterSpawn, new Vector3(Random.Range(-75, 76), 25, MapPos + Distance), Utilities.Backwards);
                                break;
                        }
                    }
                }
                if (EnemyDrone) { // Drones
                    int Distance = 100 + (FightersPerWave - DronesPerWave) * 10;
                    int WaveKind = Random.Range(0, 2);
                    for (int i = 0; i < DronesPerWave; i++) {
                        Distance += 10;
                        switch (WaveKind) {
                            case 0: // Random formation
                                Instantiate(EnemyDrone, new Vector3(Random.Range(-75, 76), 25, MapPos + Distance), Utilities.Backwards);
                                break;
                            case 1: // Two columns
                                Instantiate(EnemyDrone, new Vector3(i % 2 == 1 ? -50 : 50, 25, MapPos + Distance + (i % 2 == 1 ? -25 : 0)), Utilities.Backwards);
                                break;
                        }
                    }
                }
                NextWave += WaveCountdown;
            }
        }
    }
}