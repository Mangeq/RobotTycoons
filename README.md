# 🤖 Robot Tycoon - Mobile Idle Simulation Game

**Robot Tycoon** is a comprehensive mobile idle simulation game developed with **Unity (C#)**. The game challenges players to build and manage a robot manufacturing empire that spans across two planets: Earth and Mars.

This project demonstrates advanced mobile game development concepts including Cloud Database integration, Ad monetization, multi-scene management, and scalable software architecture.

## 🎮 Key Features

### 🌍 Dual-World Economy (Earth & Mars)
* **Dynamic Context Switching:** Seamless transition between Earth (Gold Currency) and Mars (Plasma Currency) scenes using camera manipulation and state management.
* **Production Lines:** Upgradable machines with exponential cost/income algorithms.
* **Assembly System:** Collecting parts (Heads, Arms, Legs) to assemble robots for prestige bonuses.

### ☁️ Backend & Data Management
* **Firebase Realtime Database:** Implemented a robust cloud save system. Player data (Currency, Inventory, Upgrades) is serialized to JSON and synchronized via REST API.
* **Data Hygiene:** Custom logic to automatically wipe user data if inactive for more than **90 days** to optimize database storage.
* **Offline Calculation:** (Planned) System to calculate income generated while the player was away.

### 💰 Monetization (AdMob)
* **Interstitial Ads:** Auto-triggered ads every 5 minutes.
* **Rewarded Ads:** "2x Income Boost" mechanic allowing players to watch ads for temporary gameplay advantages.

---

## 🛠️ Technical Stack & Architecture

* **Engine:** Unity 2022.3 (LTS)
* **Language:** C#
* **Platform:** Android (IL2CPP / ARM64)
* **Version Control:** Git & GitHub (LFS configured for large plugins)
* **3rd Party SDKs:** * Google Mobile Ads (AdMob)
    * Firebase SDK (Auth & Database)
    * TextMeshPro

### 🏗️ Code Architecture
The project follows a **Modular Monolithic** approach with **Singleton Managers** to handle core systems:

* **`EconomyManager`:** Handles all currency transactions, "Gold per Second" calculations, and inflation logic.
* **`FactoryManager`:** Manages production lines, unlock states, and visual updates for both planets.
* **`SaveManager`:** Handles JSON serialization, `UnityWebRequest` for Firebase communication, and timestamp checks.
* **`UIManager`:** Event-driven UI updates to minimize performance overhead (Update only when data changes).
* **`AdManager`:** Centralized controller for AdMob logic and reward callbacks.

---

## 🚀 Installation & Setup

1.  **Clone the repo:**
    ```bash
    git clone [https://github.com/YourUsername/RobotTycoon.git](https://github.com/YourUsername/RobotTycoon.git)
    ```
2.  **Open in Unity:**
    * Launch Unity Hub.
    * Add the project folder.
    * Open with Unity Version **2022.3.x**.
3.  **Setup SDKs:**
    * *Note: Firebase and AdMob API keys have been removed for security. You must import your own `google-services.json` and set up AdMob App IDs in `Assets > Google Mobile Ads > Settings`.*
4.  **Build:**
    * Go to `File > Build Settings`.
    * Switch Platform to **Android**.
    * Build APK.

---

## 👨‍💻 Developer Notes
This project was built as a solo development effort to master the full lifecycle of a mobile game, from architecture design to deployment.

* **Challenge:** Integrating Firebase without using heavy SDKs initially.
* **Solution:** Used `UnityWebRequest` for lightweight REST API calls.
* **Challenge:** Managing huge texture files in Git.
* **Solution:** Configured **Git LFS** to track `.bundle`, `.so`, and `.psd` files.

---

### 📧 Contact
**Emir** - [https://www.linkedin.com/in/emir-%C3%A7-061011267/] - [emircaliskan01@gmail.com]

*Project created in 2025.*
