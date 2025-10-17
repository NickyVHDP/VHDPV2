# VHDPV2 Roguelite Prototype

This repository contains a Unity 2022.3 project scaffold for a top-down auto-shooter roguelite inspired by Vampire Survivors and Brotato. The project is structured around feature folders, ScriptableObjects for configuration, and modular systems for combat, upgrades, spawning, meta progression, and UI.

## Contents
- `Assets/Code` contains runtime systems split by domain (Player, Weapons, Enemies, Spawning, Upgrades, Effects, UI, Save, Core utilities).
- `Assets/Prefabs` includes placeholder prefabs for the player, enemies, projectiles, and pickups.
- `Assets/Data/ScriptableObjects` ships JSON data samples that mirror the balancing format described in the design doc.
- `Assets/Scenes` includes placeholder scenes for Boot, MainMenu, Game, and Meta.
- `Assets/Tests` provides edit mode unit tests that validate XP thresholds, combat math, and weighted rarity rolls.
- `Assets/Code/Editor/BuildAutomation.cs` exposes menu items for automated Windows and WebGL builds with version stamping.

## Getting Started
1. Open the project in Unity 2022.3 or newer.
2. Load `Assets/Scenes/Boot.unity` to bootstrap the runtime systems.
3. Configure ScriptableObject assets under `Assets/Resources` and `Assets/Data/ScriptableObjects` to tune balance.
4. Use `VHD/Build/All` from the Unity editor menu to produce builds in `Builds/Windows` and `Builds/WebGL`.

## Tests
Run the edit mode tests via the Unity Test Runner to ensure deterministic XP leveling, armor math, and weighted rarity selection behave as expected.
