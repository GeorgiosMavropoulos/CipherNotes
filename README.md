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
```
┌─────────────────────────────────────────┐
│  Cipher_Notes  (MAUI Blazor Hybrid app)  │
│  Razor Pages · ViewModels · DI setup     │
└───────────────────┬─────────────────────┘
                     │ references
┌────────────────────▼────────────────────┐
│  Cipher_Notes.Core  (plain class library) │
│  Models · Services · Exceptions           │
│  — no MAUI dependency —                   │
└────────────────────┬────────────────────┘
                     │ persists to
               ┌─────▼─────┐
               │  SQLite   │
               └───────────┘

┌─────────────────────────────────────────┐
│  Cipher_Notes.Tests  (xUnit)              │
│  references Cipher_Notes.Core directly    │
└─────────────────────────────────────────┘
```
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
  - **.NET Blazor Hybrid** 
  - **Android SDK & Emulator** (installed automatically with the MAUI workload)
- **.NET 10 SDK** (or the version targeted by the project)
- An **Android Emulator** configured (Pixel device recommended, API 34+, 4GB+ RAM)

---

## 🚀 How to Run (Visual Studio)

1. **Clone the repository**
   
  git clone https://github.com/SupremeEngineer98/CipherNotes
  cd cipher_notes
  

2. **Open the solution**
   - Open Cipher_Notes.sln in Visual Studio.

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
```

cipher_notes/
├── Cipher_Notes/                     # MAUI Blazor Hybrid app
│   ├── Pages/ (Razor Components)
│   │   ├── Home.razor               # Notes list / dashboard
│   │   ├── CreateNote.razor         # Create a new encrypted note
│   │   ├── ViewNote.razor           # Decrypt & view a note
│   │   └── UpdateNote.razor         # Decrypt & edit an existing note
│   ├── wwwroot/
│   │   ├── css/app.css              # App styling
│   │   └── images/                  # Logo & icons
│   ├── MauiProgram.cs               # DI container configuration
│   └── App.xaml.cs                  # App entry point
│
├── Cipher_Notes.Core/                # Plain class library (no MAUI dependency)
│   ├── Models/
│   │   └── SecureNotes.cs            # Note entity (Id, Title, Encrypted_content, Salt, IV, dates)
│   │   ├── ViewModels/
│   │   ├── NoteListViewModel.cs
│   │   ├── DecryptNoteViewModel.cs
│   │   └── UpdateNoteViewModel.cs
    ├── Services/
│   │   ├── DatabaseService.cs       # SQLite CRUD operations
│   │   ├── EncryptionService.cs     # AES encryption/decryption + PBKDF2 key derivation
│   │   └── NoteService.cs           # Business logic connecting UI to DB & encryption
│   └── Exceptions/
│       ├── InvalidPasswordException.cs  # Thrown when decryption fails due to a wrong password
│       ├── NotFoundException.cs         # Thrown when a requested note does not exist
│       └── ValidationException.cs       # Thrown on invalid/empty input
│
└── Cipher_Notes.Tests/                # xUnit test project

    └── TestEncryptionService.cs       # Unit tests for AES encryption/decryption logic
    └── TestDatabaseService.cs         # Unit tests for crud database logic
    └── TestNoteService.cs             # Unit tests for NoteService's methods


    └── TestEncryptionService.cs      # Unit tests for AES encryption/decryption logic
```


🧪 Testing

Cipher_Notes.Tests is an xUnit project covering EncryptionService in Cipher_Notes.Core. Tests run instantly since the Core library has no MAUI/platform dependency.

Run from Visual Studio: open Test Explorer → Run All Tests.
If prompted "There were build errors. Would you like to continue and run tests from the last successful build?" — click Yes. This happens because the MAUI app is part of the same solution but isn't required for the tests themselves.

Run from the CLI:

bashdotnet test cipher_notes/Cipher_Notes.Tests/Cipher_Notes.Tests.csproj

### 🧪 Test Coverage — `EncryptionService`

| Test                                       
|
|EncryptNote_ValidInput_ReturnsEncryptedData: Encrypting valid content returns a non-null cipher text, salt, and IV, and the cipher text differs from the original content 
|
|Test_EmptyPassword_Throws_ValidationException: An empty password throws a ValidationException with the message *"Password cannot be empty"* 
|
|Test_Encrypt_Note_Randomness: Encrypting the same content twice produces a different salt, IV, and cipher text each time (proves randomness) 
|
|Test_Decrypted_Content_Will_Be_Equals_To_The_Original_Content_Before_Being_Encrypted: A full encrypt → decrypt round-trip returns the exact original content 
|
|Test_Decryption_Will_Return_Validation_Exception_If_Password_Null: Decrypting with an empty password throws a ValidationException|
   
## 🔒 Security Notes

- Each note is encrypted individually using **AES** with a unique **salt** and **IV**.
- The encryption key is derived from the user's password using **PBKDF2**, never stored directly.
- If a password is lost, the corresponding note **cannot be recovered** — this is by design.
- All data remains **on-device**; the app makes no network requests.

---

## 📌 Notes / Limitations

- Currently tested and supported on **Android** only.
- Forgotten passwords cannot be reset — encrypted content will be permanently inaccessible.
