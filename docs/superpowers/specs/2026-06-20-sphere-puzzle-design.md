# Sphere Puzzle — Game Design Spec
**Date:** 2026-06-20
**Project:** Juniper-Dev-Jam-02
**Theme:** Spin to Win

---

## Concept

A glass sphere sits at the centre of the scene. Inside it is a 3D marble track made of roads, slopes, and tubes. A marble rolls on the track under gravity. The player rotates the outer sphere by click-dragging, changing which direction is "down" relative to the track, and must guide the marble to the finish.

---

## Core Decisions

| Topic | Decision |
|---|---|
| Player control | Click-and-drag (left mouse button) |
| Camera | Fixed orbit, always centred on sphere, Cinemachine 3.1.7 |
| Fail condition | Marble touches the glass shell — reset to start |
| Win condition | Marble enters FinishZone trigger |
| Level count | 2–3 scenes |
| Architecture | Option C: track is child of SphereRoot, marble is world-space Rigidbody |

---

## Section 1: Scene Hierarchy

```
SphereRoot  (Transform only — no Rigidbody, no Collider)
├── GlassShell          (MeshRenderer + trigger SphereCollider — fail detection)
├── TrackGeometry       (MeshFilter + MeshRenderer + MeshCollider per piece)
│   ├── Road_01
│   ├── Slope_01
│   ├── Tube_01
│   └── ...
├── MarbleStart         (empty Transform — defines marble reset position)
└── FinishZone          (trigger SphereCollider — win detection)

Marble      (world-space Rigidbody + SphereCollider, tagged "Marble")

CinemachineCamera   (world-space, fixed orbit around SphereRoot)
Main Camera         (CinemachineBrain)
GameManager         (singleton)
```

Key constraints:
- `SphereRoot` is a plain Transform. Rotating it moves all track colliders and the glass shell together.
- The marble is never parented to `SphereRoot` — it lives in world space so PhysX handles rolling over the moving track colliders naturally.
- `GlassShell` trigger sits slightly inset from the visual mesh so the marble triggers it before visually clipping.
- `MarbleStart` rotates with `SphereRoot`, so its world-space position is always correct relative to the current sphere orientation at reset time.

---

## Section 2: Player Input & Sphere Rotation

**Component:** `SphereRotator` MonoBehaviour on `SphereRoot`.

**Input:** Uses the existing `InputReader` ScriptableObject (passed as serialized field). No new input actions required.

- `PrimaryActionEvent` (`<Mouse>/leftButton`) — sets `bool _isDragging`.
- `LookEvent` (`<Pointer>/delta`) — fires every frame with mouse delta; rotation is only applied when `_isDragging` is true.

**Rotation:**
- `delta.x` → rotate `SphereRoot` around **world Y axis**.
- `delta.y` → rotate `SphereRoot` around the **camera's right axis** (camera-relative vertical drag).
- Scaled by `[SerializeField] float rotationSpeed`.
- Applied via `Transform.Rotate(axis, angle, Space.World)`.

Free rotation on both axes, no clamping. The sphere can spin continuously in any direction.

---

## Section 3: Marble Physics, Fail & Win Detection

**Marble:**
- World-space `Rigidbody` (non-kinematic) + `SphereCollider`, tagged `"Marble"`.
- Standard Unity gravity. No custom physics.
- `PhysicsMaterial` exposed as an Inspector field on both marble and track pieces — primary gameplay feel lever.

**Fail detection (`GlassShell`):**
- Trigger `SphereCollider` at the inner radius of the glass shell.
- `GlassShell` MonoBehaviour implements `OnTriggerEnter`, checks for `"Marble"` tag, raises `event Action OnMarbleFailed`.
- `GameManager` subscribes and handles the reset.

**Reset:**
- Zero the marble's `Rigidbody.velocity` and `angularVelocity`.
- Set marble position to `MarbleStart.position` (world space, already correct for current sphere orientation).

**Win detection (`FinishZone`):**
- Small trigger collider at the end of the track, child of `SphereRoot`.
- `FinishZone` MonoBehaviour raises `event Action OnMarbleFinished` on `"Marble"` tag entry.
- `GameManager` subscribes and triggers the win flow.

---

## Section 4: Game State & Level Flow

**`GameManager`** (inherits `Singleton<GameManager>`).

Three states: `Playing`, `Failed`, `Won`.

- **Playing** — normal gameplay, input active.
- **Failed** — on `GlassShell.OnMarbleFailed`: disable input briefly, reset marble, re-enable input, return to Playing.
- **Won** — on `FinishZone.OnMarbleFinished`: call `InputReader.DisableAllInput()`, wait a short delay, call `SceneController` to load the next scene.

**`GameManager` serialized fields:**
- `InputReader inputReader`
- `Rigidbody marble`
- `Transform marbleStart`
- `string nextSceneName`

**Level scenes:** `Level_01`, `Level_02`, `Level_03`. `SceneController` (existing) handles `SceneManager.LoadScene` calls.

No pause menu in scope for the jam.

---

## Section 5: Camera (Cinemachine 3.1.7)

Single `CinemachineCamera` in each level scene.

- **Follow & Look At:** both set to `SphereRoot`.
- **Body:** `CinemachineOrbitalFollow` — fixed radius (~8–10 units), damping 0, no player orbit input. The `CinemachineInputAxisController` component is not added, so the player cannot orbit the camera.
- **Aim:** `CinemachineHardLookAt` targeting `SphereRoot` — sphere stays perfectly centred.
- **Binding Mode:** `World Space` — camera never tilts with sphere rotation.

CM Brain lives on Main Camera. No blends or transitions needed within a level. Scene reload resets the camera naturally between levels.

---

## Out of Scope (Jam)

- Pause menu
- Level select / main menu
- Hazards (design placeholder — will use same fail event pattern as GlassShell)
- Checkpoints
- Sound / music
