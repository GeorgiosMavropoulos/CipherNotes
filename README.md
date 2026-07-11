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
│   ├── ViewModels/
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
    └── Test_ViewNoteViewModel.cs      # Unit tests for ViewNoteViewModel
    └── TestCreateNoteViewModel.cs     # Unit tests for CreateNoteViewModel
    └── TestDecryptNoteViewModel.cs    # Unit tests for DecryptNoteViewModel
    └── TestNoteListViewModel.cs       # Unit tests for NoteListViewModel
    └── TestUpdateNoteViewModel.cs     # Unit tests for UpdateNoteViewModel
```


🧪 Testing

Cipher_Notes.Tests is an xUnit project covering EncryptionService in Cipher_Notes.Core. Tests run instantly since the Core library has no MAUI/platform dependency.

Run from Visual Studio: open Test Explorer → Run All Tests.
If prompted "There were build errors. Would you like to continue and run tests from the last successful build?" — click Yes. This happens because the MAUI app is part of the same solution but isn't required for the tests themselves.

Run from the CLI:

bashdotnet test cipher_notes/Cipher_Notes.Tests/Cipher_Notes.Tests.csproj

### 🧪 Test Coverage 

```
### `TestEncryptionService`

| # | Test | Description |
|---|---|---|
| 1 | `EncryptNote_ValidInput_ReturnsEncryptedData` | Valid content returns non-null cipher, salt, and IV — cipher differs from original |
| 2 | `Test_EmptyPassword_Throws_ValidationException` | Empty password → `ValidationException`: *"Password cannot be empty"* |
| 3 | `Test_Encrypt_Note_Randomness` | Same content encrypted twice produces different salt, IV, and cipher (proves randomness) |
| 4 | `Test_Decrypted_Content_Will_Be_Equals_To_The_Original_Content_Before_Being_Encrypted` | Full encrypt → decrypt round-trip returns exact original content |
| 5 | `Test_Decryption_Will_Return_Validation_Exception_If_Password_Null` | Empty password on decrypt → `ValidationException`: *"Password cannot be empty"* |
| 6 | `Test_Decryption_Will_Return_InvalidPasswordException_If_Password_Is_Wrong` | Wrong password → `InvalidPasswordException`: *"Wrong password"* |
| 7 | `Test_DeriveKey_Will_Return_The_Same_Key_If_The_Same_Pass_And_Salt_Are_Being_Provided` | Same password + salt always produce the same derived key |
| 8 | `Test_DeriveKey_Will_Return_Validation_Exception_If_A_Custom_Written_Salt_Has_Been_Used` | Invalid salt → `ValidationException`: *"Invalid salt format"* |
| 9 | `Test_GenerateSalt_Returns_A_16_Char_Sequence` | Returns non-null Base64 string decoding to 16 bytes |
| 10 | `Test_GenerateIV_Returns_A_16_Char_Sequence` | Returns non-null Base64 string decoding to 16 bytes |
| 11 | `Test_EncryptNote_Can_Encrypt_Large_Content` | 10,000-character content encrypts successfully |
| 12 | `Test_That_EncryptNote_Can_Encrypt_Special_Chars` | Special characters encrypt successfully |
| 13 | `Test_That_EncryptNote_Can_Encrypt_Greek_Chars` | Greek characters encrypt successfully |
| 14 | `Test_That_DecryptNote_Can_Decrypt_Greek_Text` | Greek content survives encrypt → decrypt round-trip |
| 15 | `Test_That_DecryptContent_Can_Decrypt_Special_Chars` | Special characters survive encrypt → decrypt round-trip |
| 16 | `Test_That_DecryptContent_Can_Decrypt_Large_Text` | 10,000-character content survives encrypt → decrypt round-trip |

---

### `TestNoteService`

| # | Test | Description |
|---|---|---|
| 1 | `Test_CreateNote_ValidInput_Calls_Create` | Valid inputs → `DatabaseService.Create` called once |
| 2 | `Test_CreateNote_Returns_ValidationException_When_Title_Missing` | Missing title → `ValidationException`: *"Title is empty"* |
| 3 | `Test_CreateNote_Returns_ValidationException_When_Content_Missing` | Missing content → `ValidationException`: *"Content is empty"* |
| 4 | `Test_CreateNote_Returns_ValidationException_When_Password_Missing` | Missing password → `ValidationException`: *"Password is missing"* |
| 5 | `Test_GetAllNotes_Returns_A_List_With_A_Single_Note` | Returns list with the expected note |
| 6 | `Test_GetAllNotes_Returns_An_Empty_List_If_DB_Is_Empty` | Returns empty non-null list when DB is empty |
| 7 | `Test_GetAllNotes_Returns_The_Expected_Exception` | Propagates exception: *"Failed to get notes"* |
| 8 | `Test_GetNoteById_Returnes_The_Requested_Note` | Returns correct note by ID |
| 9 | `Test_GetNoteById_Returns_NotFoundException_With_The_Appropriate_Message_When_Note_Does_Not_Exist` | ID not found → `NotFoundException`: *"Note not found"* |
| 10 | `Test_GetNoteById_Returns_The_Expected_General_Exception` | Propagates exception: *"Failed to get note"* |
| 11 | `Test_DecryptNote_Successfully_Decrypts_Note` | Successfully decrypts and returns plain text |
| 12 | `Test_DecryptNote_Will_Return_ValidationException_If_Password_Is_Empty` | Empty password → `ValidationException`: *"Password is missing"* |
| 13 | `Test_DecryptNote_Returns_InvalidPasswordException_If_Password_Is_Wrong` | Wrong password → `InvalidPasswordException`: *"Wrong Password"* |
| 14 | `Test_DecryptContent_Returns_NotFoundException` | Note not found → `NotFoundException` |
| 15 | `Test_DecryptNote_Returns_CryptographicException` | Crypto failure → `CryptographicException`: *"Decryption error"* |
| 16 | `Test_DecryptNote_Returns_Exception` | Propagates exception: *"Unexpected error during decryption"* |
| 17 | `Test_UpdateNote_updates_note_successfully` | `DatabaseService.Update` called once on success |
| 18 | `Test_UpdateNote_Returns_ValidationException_if_Content_Is_Missing` | Missing content → `ValidationException`: *"Content is empty"* |
| 19 | `Test_UpdateNote_Returns_ValidationException_if_Title_Is_Missing` | Missing title → `ValidationException`: *"Title is empty"* |
| 20 | `Test_UpdateNote_Returns_ValidationException_if_Password_Is_Missing` | Missing password → `ValidationException`: *"Password is missing"* |
| 21 | `Test_UpdateNote_Returns_InvalidPasswordException_if_Password_Is_Wrong` | Wrong password → `InvalidPasswordException`: *"Wrong password"* |
| 22 | `Test_UpdateNote_Returns_NotFoundException_If_Note_Does_Not_Exist` | Note not found → `NotFoundException`: *"Note not found"* |
| 23 | `Test_UpdateNote_Returns_CryptographicException_If_Decryption_Fail` | Decryption failure → `CryptographicException` |
| 24 | `Test_UpdateNote_Returns_CryptographicException_If_Encryption_Fail` | Re-encryption failure → `CryptographicException` |
| 25 | `Test_ApplyEncryption_Successfully_Encrypts_Note` | Updates `Encrypted_content`, `Salt`, and `IV` correctly |
| 26 | `Test_ApplyEncryption_Returns_ValidationException_If_Password_Is_Empty` | Empty password → `ValidationException`: *"Password is missing"* |
| 27 | `Test_ApplyEncryption_Returns_ValidationException_If_Content_Is_Empty` | Empty content → `ValidationException`: *"Content is missing"* |
| 28 | `Test_ApplyEncryption_Returns_NotFoundException_If_Note_Is_Empty` | Null note → `NotFoundException`: *"Note does not exist"* |
| 29 | `Test_that_AppleyEncryption_Returns_Successfully_CryptographicException` | Crypto failure → `CryptographicException`: *"Encryption failed"* |
| 30 | `Test_Delete_Sucessfully_Deletes_Notes` | `DatabaseService.Delete` called once on success |
| 31 | `Test_Delete_Returns_NotFoundException_If_Note_Does_Not_Exist` | Note not found → `NotFoundException`: *"Note not found"* |

---

### `TestDatabaseService` — Integration Tests

| # | Test | Description |
|---|---|---|
| 1 | `Test_Create_Successfully_Creates_Note_and_GetById_Successfully_Returns_The_Created_Note` | Creates note and retrieves it — verifies title, salt, and IV |
| 2 | `Test_Create_Method_Returns_DatabaseException` | Failure → `DatabaseException`: *"Failed to save note in the Database"* |
| 3 | `Test_Update_Successfully_Updates_Notes` | Updates title and content — verifies changes are persisted |
| 4 | `Test_Update_Returns_ValidationException_If_Note_Is_Null` | Null note → `ValidationException`: *"Note does not exist"* |
| 5 | `Test_Update_Returns_NotFoundException_If_DB_Query_Return_0_Rows_Changed` | 0 rows affected → `NotFoundException` |
| 6 | `Test_Delete_Successfully_Delete_Notes` | Deletes note — `GetById` returns null afterwards |
| 7 | `Test_Delete_Returns_NotFoundException_If_Id_Does_Not_Exists` | ID not found → `NotFoundException`: *"Note does not exist"* |

---

### `TestCreateNoteViewModel`

| # | Test | Description |
|---|---|---|
| 1 | `Test_CreateNote_Successfully_Creates_Notes` | `NoteService.CreateNote` called with correct values exactly once |
| 2 | `Test_CreateNote_Returns_General_Exception` | Propagates general exception from service to caller |

---

### `TestDecryptNoteViewModel`

| # | Test | Description |
|---|---|---|
| 1 | `Test_DecryptNote_Successfully_Decrypt_Notes` | `NoteService.DecryptNote` called — result stored in `Decrypted_content` |
| 2 | `Test_DecryptNote_Successfully_Returns_General_Exception` | Propagates exception: *"Unexpected error during decryption"* |

---

### `TestUpdateNoteViewModel`

| # | Test | Description |
|---|---|---|
| 1 | `Test_LoadNote_Successfully_Returns_The_Requested_Note` | `NoteService.GetNoteById` called once — note stored in ViewModel |

---

### `TestNoteListViewModel`

| # | Test | Description |
|---|---|---|
| 1 | `Test_LoadNotes_Successfully_Loads_Existing_Notes` | Loads 2 notes — titles verified |
| 2 | `Test_LoadNotes_Returns_General_Exception` | Propagates exception: *"Failed to get notes"* |
| 3 | `Test_DeleteNote_Deletes_Note_With_Success` | `NoteService.Delete` called once on success |
| 4 | `Test_DeleteNote_Returns_Exception` | Propagates exception: *"Error.Deletion failed"* |
| 5 | `Test_DeleteNote_Removes_Deleted_Notes_From_The_List` | Deleted note removed from `Notes` collection — count verified as 0 |
| 6 | `Test_FindNoteByTitle_Successfully_Returns_Title` | Returns correct note when title matches |
| 7 | `Test_FindNoteByTitle_Returns_NotFoundException` | Empty collection → `NotFoundException`: *"Note does not exist"* |

---

### `Test_ViewNoteViewModel`

| # | Test | Description |
|---|---|---|
| 1 | `Test_LoadNote_Successfully_Returns_The_Requested_Note` | `NoteService.GetNoteById` called — note stored in `Note` property |
| 2 | `Test_LoadNote_Successfully_Returns_General_Exception` | Propagates exception: *"Failed to get note"* |
| 3 | `Test_Decrypt_Success_Decrypts_Note` | `NoteService.DecryptNote` called — result stored in `DecryptedContent` |
| 4 | `Test_Decrypt_Returns_General_Exception_With_Success` | Propagates exception: *"Unexpected error during decryption"* |
```
   
## 🔒 Security Notes

- Each note is encrypted individually using **AES** with a unique **salt** and **IV**.
- The encryption key is derived from the user's password using **PBKDF2**, never stored directly.
- If a password is lost, the corresponding note **cannot be recovered** — this is by design.
- All data remains **on-device**; the app makes no network requests.

---

## 📌 Notes / Limitations

- Currently tested and supported on **Android** only.
- Forgotten passwords cannot be reset — encrypted content will be permanently inaccessible.
