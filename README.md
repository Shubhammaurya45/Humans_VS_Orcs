# Human vs Orcs

A 2D strategy / tower-defense game built in **Unity (C#)**, where humans defend their base against waves of orcs using towers, unit placement, and tactical positioning.

> 🎮 Playable build available — see [Getting Started](#getting-started) below.

---

## 🧩 Features

- **Building & Placement System** — Grid-based, center-anchored placement with multi-cell support and real-time valid/invalid placement feedback
- **Unit AI** — State-machine-driven enemy units (Idle / Moving / Attacking) with detection and combat logic
- **Custom A\* Pathfinding** — Tilemap-based pathfinding shared across orcs and builder units
- **Tower Defense Mechanics** — Tower construction, per-tower build timers, and projectile-based combat with object pooling
- **Unit Separation** — Physics-based push force to prevent units from stacking on each other
- **Camera Controls** — WASD movement, right-click drag, scroll zoom, and full touch support (pan + pinch-to-zoom)
- **Audio System** — Separate Music/SFX mixer groups with persistent volume and mute settings
- **PlayFab Backend Integration** — Guest login, Google Sign-In, and cross-scene player identity
- **Settings & Persistence** — Resolution, V-Sync, and brightness settings saved across sessions

---

## 🛠️ Built With

- [Unity](https://unity.com/) (2D)
- C#

- TextMeshPro — UI

---

🚀 Getting Started




## 🎮 Controls

 Move Camera       -->   `W A S D` 
 Drag Camera       -->    Right-click + drag 
 Zoom              -->    Scroll wheel / Pinch (touch) 
 Place Building    -->    Left-click 

---

## 📂 Project Structure

```
Assets/
├── Scripts/
│   ├── Buildings/       # Placement, construction, validation
│   ├── Units/           # Unit AI, movement, separation
│   ├── Pathfinding/     # A* implementation (Node, GridSystem, Pathfinder)
│   ├── Combat/          # Projectiles, detection, towers
│   ├── UI/              # UI management, timers
│   ├── Audio/           # Audio mixer control
│   └── Core/            # Singleton managers, GameManager
├── Scenes/
└── Prefabs/
```

---

## 🗺️ Roadmap

- [ ] Shared alert system — patrol/tower units raise an alert to trigger all units into combat on enemy detection
- [ ] Additional enemy types and wave scaling
- [ ] More building/tower variety

---

## 📄 License

License to be decided.

---

## 🙋 Author

**Shubham Maurya** — [@Shubhammaurya45](https://github.com/Shubhammaurya45)
