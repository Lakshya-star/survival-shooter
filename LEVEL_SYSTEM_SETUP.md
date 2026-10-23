# Level System Setup Instructions

This document explains the automatic level progression system in Unity.

## New Features Added

1. **Level System**: Levels increase every 100 points
2. **High Score Tracking**: Saves highest score using PlayerPrefs
3. **Progressive Difficulty**:
   - Enemy health increases by 20% per level
   - Enemy damage increases by 15% per level
   - Spawn rate decreases by 10% per level (faster spawns)
4. **Enemy Weapons**: Enemies get ranged weapons starting at Level 3
5. **Enemy Blasters**: At Level 5+, enemies get powerful blasters with cyan laser beams
6. **Variable Scoring**: Small enemies = 10 points, Big enemies = 20 points
7. **Shooting Animations**: Enemies animate when shooting
8. **Weapon Effects**: Muzzle flash, laser beams, and sound effects
9. **Automatic UI**: Level and high score displays are created automatically

## IMPORTANT: Automatic Setup

**NO MANUAL SETUP REQUIRED IN UNITY EDITOR!**

All the scripts have been modified to automatically create everything when you run the game:

- ✅ LevelManager is created automatically
- ✅ Level and High Score UI texts are created automatically
- ✅ Game Over score texts are created automatically
- ✅ All references are connected automatically
- ✅ Enemy weapon effects are added automatically

Just open your project in Unity and press Play - everything will work!

## What Will Appear When You Play

### During Gameplay:
- **Score**: Your existing score display (top-left)
- **Level**: New text showing current level (top-left, below score)
- **High Score**: New text showing your best score (top-left, below level)

### When You Die:
- **Game Over**: Your existing game over screen
- **Final Score**: Shows your score for that run
- **High Score**: Shows your all-time best score

## Level Progression Details

- **Level 1**: Base difficulty (100 HP enemies, 10 damage, 3s spawn)
  - Score: Small enemies = 10pts, Big enemies = 20pts
- **Level 2** (100+ score): 120 HP enemies, 11.5 damage, 2.7s spawn
- **Level 3** (200+ score): 144 HP enemies, 13 damage, 2.4s spawn + **ENEMIES GET BASIC RANGED WEAPONS**
  - Red laser beams
  - 1.5s fire rate
  - 15 damage per shot
- **Level 4** (300+ score): 173 HP enemies, 15 damage, 2.1s spawn
  - Faster shooting (1.4s fire rate)
  - More damage (20 per shot)
- **Level 5** (400+ score): 207 HP enemies, 17 damage, 1.9s spawn + **ENEMIES GET BLASTERS**
  - Cyan laser beams (thicker, brighter)
  - Faster shooting (1.0s fire rate)
  - More damage (25 per shot)
  - Intense muzzle flash effects
- **Level 6+** (500+ score): Even harder enemies with blasters

## Enemy Types

- **Small Enemies** (Zombunny, ZomBear): 10 points, easier to kill
- **Big Enemy** (Hellephant): 20 points, more health, more dangerous

## Weapon Progression

### Level 1-2: Melee Only
- Enemies only attack by touching the player

### Level 3-4: Basic Ranged Weapons
- Red laser beams
- Muzzle flash effects
- Shooting animations
- Moderate fire rate

### Level 5+: Blasters
- Cyan laser beams (thicker, brighter)
- Intense muzzle flash
- Faster fire rate
- Higher damage
- More dangerous!

## Configuration Options

If you want to adjust difficulty, edit `LevelManager.cs`:
- `scorePerLevel`: Points needed to reach next level (default: 100)
- Multipliers in `GetEnemyHealthMultiplier()`, `GetEnemyDamageMultiplier()`, `GetSpawnRateMultiplier()`
- Level when enemies get weapons in `EnemyHasWeapon()` (default: 3)
- Level when enemies get blasters in `EnemyHasBlaster()` (default: 5)

## Testing

1. Open Unity
2. Open your scene
3. Click Play
4. Kill enemies and watch:
   - Score increasing (10 for small, 20 for big enemies)
   - Level going up at 100, 200, 300+ points
   - High score updating
   - Enemies getting harder
   - At Level 3+, enemies start shooting red lasers at you
   - At Level 5+, enemies get cyan blasters with intense effects
5. Die and check game over screen shows scores

## Troubleshooting

- **UI not appearing?** Check Console for errors - the scripts create UI automatically
- **Level not updating?** The LevelManager should be created automatically - check Console
- **High score not saving?** PlayerPrefs works in game builds, may have issues in editor
- **Enemies not shooting?** At Level 3+, the EnemyShooting script is added automatically
- **Blaster not appearing?** At Level 5+, enemies get blaster effects automatically
- **Weapon effects not showing?** The EnemyWeaponEffects script adds muzzle flash and laser effects

## Files Modified/Created

- **Created**: `Assets/Scripts/Manager/LevelManager.cs` - Core level management (auto-creates itself)
- **Created**: `Assets/Scripts/Enemy/EnemyShooting.cs` - Enemy ranged weapon system with blasters
- **Created**: `Assets/Scripts/Enemy/EnemyWeaponEffects.cs` - Automatic muzzle flash and laser effects
- **Modified**: `Assets/Scripts/Factory/EnemyFactory.cs` - Applies level-based stats and weapon types
- **Modified**: `Assets/Scripts/Manager/EnemyManager.cs` - Dynamic spawn rates
- **Modified**: `Assets/Scripts/Manager/ScoreManager.cs` - Auto-creates level/high score UI with game font
- **Modified**: `Assets/Scripts/Manager/GameOverManager.cs` - Auto-creates game over score UI with game font
