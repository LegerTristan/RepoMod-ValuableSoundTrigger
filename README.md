# ValuableSoundTrigger

**A mod for [R.E.P.O](https://store.steampowered.com/app/3241660/REPO/) introducing dynamic audio triggers for valuable objects. Built to prank your friends with fake damage sound on valuables or simply to play *Tralalero Tralala* from [SillyValuables](https://thunderstore.io/c/repo/p/Sangrento/SillyValuables/) mod, as you wish.**  
*Originally crafted as a private mod for a group of friends, I decided to open-source it for the community following the Sunstone update.*

The mod is fully updated and compatible with the **Sunstone** update (and migrated to Unity 6).

---

## 📑 Table of Contents
- [Getting Started](#-getting-started)
- [Known Limitations & Support](#-known-limitations--support)
- [License](#-license)

---

## 🚀 Getting Started

### Prerequisites

A properly modded installation of R.E.P.O with the mods [BepInEx](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) and [REPOLib](https://thunderstore.io/c/repo/p/Zehs/REPOLib/) installed. I also suggest you to use [REPOConfig](https://thunderstore.io/c/repo/p/REPOModding/REPOConfig/) for controls management.

### Installation

I will add the mod to Thunderstore in the near future, meanwhile here is the manual method :

1. Download the latest release `.dll` of `ValuableSoundTrigger` from the releases page or build it from source.
2. Place the compiled DLL into your game's `BepInEx/plugins/` directory :
   ```text
   \REPO\BepInEx\plugins\ValuableSoundTrigger.dll
   ```
Ensure you are playing in an active online session/multiplayer lobby (remember: it will not trigger in solo).

## 📝 Known Limitations & Support

Here are some known limitations :

- **Multiplayer Only:** The mod **does not work in solo mode** because it relies on Photon network views to track objects properly.
- **Input Devices:** **Gamepad is not supported** for the moment (Keyboard & Mouse only).

As this project was originally a personal mod shared among friends, no major new features are planned for the near future.
However, if there is a demand from someone for specific additions or bug fixes, feel free to open an issue or reach out, and I'll consider looking into it !

## 📄 License

Distributed under the MIT License. See LICENSE for more information.

> This repo is fully public and open-source. Anyone is free to fork, modify, or adapt it to their liking. If you use this project or build upon it, a small credit or a shoutout pointing back to this repository would be greatly appreciated !
