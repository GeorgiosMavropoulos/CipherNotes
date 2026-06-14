# 🔐 CipherNotes — Secure Offline Notes Vault

An offline-first secure notes mobile application built with **.NET MAUI Blazor Hybrid** that allows users to store sensitive notes locally using strong encryption.

The app ensures full privacy by encrypting all data on-device using **AES encryption** with **password-based key derivation (PBKDF2)**. No backend, no cloud, no data leakage.

---

## 📱 Features

- 📝 Create secure encrypted notes
- 🔓 Decrypt notes using password
- 📂 View list of saved notes (titles only)
- ✏️ Update existing notes (after decryption)
- 🗑️ Delete notes locally
- 🔐 AES encryption for all stored content
- 🔑 Password-based key derivation (PBKDF2)
- 💾 Local storage using SQLite
- 🚫 Fully offline (no network calls)

---

## 🏗️ Architecture

```text
UI Layer (Blazor .razor Pages)
        ↓
ViewModels (State & Logic - CommunityToolkit.Mvvm)
        ↓
Service Layer
   ├── EncryptionService
   ├── NoteService
   └── DatabaseService
        ↓
SQLite (Local Storage)
```

---

## 🛠️ Tech Stack

- **.NET 10 / .NET MAUI** — cross-platform app framework
- **Blazor Hybrid** — UI built with Razor components rendered in a native WebView
- **CommunityToolkit.Mvvm** — simplifies MVVM with `[ObservableProperty]` and `[RelayCommand]`
- **SQLite-net** — local, embedded database for persistent storage
- **AES (Advanced Encryption Standard)** — symmetric encryption for note content
- **PBKDF2** — derives encryption keys from user passwords with salt

---

## ✅ Prerequisites

Before running the project, make sure you have:

- **Visual Studio 2022** (17.8 or later) with the following workloads installed:
  - **.NET Multi-platform App UI development** (MAUI)
  - **Android SDK & Emulator** (installed automatically with the MAUI workload)
- **.NET 10 SDK** (or the version targeted by the project)
- An **Android Emulator** configured (Pixel device recommended, API 34+, 4GB+ RAM)

---

## 🚀 How to Run (Visual Studio)

1. **Clone the repository**
   ```bash
   git clone https://github.com/SupremeEngineer98/CipherNotes
   cd cipher_notes
   ```

2. **Open the solution**
   - Open `Cipher_Notes.sln` in Visual Studio.

3. **Restore NuGet packages**
   - Visual Studio should restore them automatically on load.
   - If not, right-click the solution → **Restore NuGet Packages**.

4. **Select the target**
   - In the toolbar, set the run target to **Android Emulator** (e.g. `Pixel_7_-_api_36`).

5. **Build the solution**
   - `Build` → `Build Solution` (or `Ctrl+Shift+B`).

6. **Run the app**
   - Press **F5** or click the **Run** (▶) button.
   - The Android Emulator will launch and the app will install and start automatically.

> ⚠️ First launch may take a while since the Blazor WebView and emulator need to initialize.

---

---

## 📂 Project Structure

```text
Cipher_Notes/
├── Models/
│   └── SecureNotes.cs          # Note entity (Id, Title, Encrypted_content, Salt, IV, dates)
├── Services/
│   ├── DatabaseService.cs      # SQLite CRUD operations
│   ├── EncryptionService.cs    # AES encryption/decryption + PBKDF2 key derivation
│   └── NoteService.cs          # Business logic connecting UI to DB & encryption
├── ViewModels/
│   ├── NoteListViewModel.cs
│   ├── DecryptNoteViewModel.cs
│   └── UpdateNoteViewModel.cs
├── Pages/ (Razor Components)
│   ├── Home.razor              # Notes list / dashboard
│   ├── CreateNote.razor         # Create a new encrypted note
│   ├── ViewNote.razor           # Decrypt & view a note
│   └── UpdateNote.razor         # Decrypt & edit an existing note
│
│── Exceptions/(Exception Classes) 
│   ├── InvalidPasswordException.cs #Custom exception for invalid password
│   ├── NotFoundException.cs        #Custom exception for not found exception messages
│   ├── ValidationException.cs      #Custom exception for empty inputs or other validation errors
├── wwwroot/
│   ├── css/app.css              # App styling
│   └── images/                  # Logo & icons
├── MauiProgram.cs               # DI container configuration
└── App.xaml.cs                  # App entry point
```

---

## 🔒 Security Notes

- Each note is encrypted individually using **AES** with a unique **salt** and **IV**.
- The encryption key is derived from the user's password using **PBKDF2**, never stored directly.
- If a password is lost, the corresponding note **cannot be recovered** — this is by design.
- All data remains **on-device**; the app makes no network requests.

---

## 📌 Notes / Limitations

- Currently tested and supported on **Android** only.
- Forgotten passwords cannot be reset — encrypted content will be permanently inaccessible.
