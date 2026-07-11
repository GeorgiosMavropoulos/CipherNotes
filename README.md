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

| Test | Description |
|---|---|
| `EncryptNote_ValidInput_ReturnsEncryptedData` | Encrypting valid content returns a non-null cipher text, salt, and IV, and the cipher text differs from the original content |
| `Test_EmptyPassword_Throws_ValidationException` | An empty password throws `ValidationException`: *"Password cannot be empty"* |
| `Test_Encrypt_Note_Randomness` | Encrypting the same content twice produces a different salt, IV, and cipher text each time (proves randomness) |
| `Test_Decrypted_Content_Will_Be_Equals_To_The_Original_Content_Before_Being_Encrypted` | A full encrypt → decrypt round-trip returns the exact original content |
| `Test_Decryption_Will_Return_Validation_Exception_If_Password_Null` | Decrypting with an empty password throws `ValidationException`: *"Password cannot be empty"* |
| `Test_Decryption_Will_Return_InvalidPasswordException_If_Password_Is_Wrong` | Decrypting with a wrong password throws `InvalidPasswordException`: *"Wrong password"* |
| `Test_DeriveKey_Will_Return_The_Same_Key_If_The_Same_Pass_And_Salt_Are_Being_Provided` | The same password and salt always produce the same derived key |
| `Test_DeriveKey_Will_Return_Validation_Exception_If_A_Custom_Written_Salt_Has_Been_Used` | An invalid salt format throws `ValidationException`: *"Invalid salt format"* |
| `Test_GenerateSalt_Returns_A_16_Char_Sequence` | `GenerateSalt` returns a non-null Base64 string that decodes to 16 bytes |
| `Test_GenerateIV_Returns_A_16_Char_Sequence` | `GenerateIV` returns a non-null Base64 string that decodes to 16 bytes |
| `Test_EncryptNote_Can_Encrypt_Large_Content` | Encrypting 10,000 characters returns a non-empty cipher that differs from the original |
| `Test_That_EncryptNote_Can_Encrypt_Special_Chars` | Special characters are encrypted successfully |
| `Test_That_EncryptNote_Can_Encrypt_Greek_Chars` | Greek characters are encrypted successfully |
| `Test_That_DecryptNote_Can_Decrypt_Greek_Text` | Greek content survives a full encrypt → decrypt round-trip |
| `Test_That_DecryptContent_Can_Decrypt_Special_Chars` | Special characters survive a full encrypt → decrypt round-trip |
| `Test_That_DecryptContent_Can_Decrypt_Large_Text` | 10,000-character content survives a full encrypt → decrypt round-trip |

---

### `TestNoteService`

| Test | Description |
|---|---|
| `Test_CreateNote_ValidInput_Calls_Create` | `CreateNote` with valid inputs calls `DatabaseService.Create` exactly once |
| `Test_CreateNote_Returns_ValidationException_When_Title_Missing` | Missing title throws `ValidationException`: *"Title is empty"* |
| `Test_CreateNote_Returns_ValidationException_When_Content_Missing` | Missing content throws `ValidationException`: *"Content is empty"* |
| `Test_CreateNote_Returns_ValidationException_When_Password_Missing` | Missing password throws `ValidationException`: *"Password is missing"* |
| `Test_GetAllNotes_Returns_A_List_With_A_Single_Note` | Returns a list containing the expected note |
| `Test_GetAllNotes_Returns_An_Empty_List_If_DB_Is_Empty` | Returns an empty non-null list when the database is empty |
| `Test_GetAllNotes_Returns_The_Expected_Exception` | Propagates a general exception with the message *"Failed to get notes"* |
| `Test_GetNoteById_Returnes_The_Requested_Note` | Returns the correct note by ID |
| `Test_GetNoteById_Returns_NotFoundException_With_The_Appropriate_Message_When_Note_Does_Not_Exist` | Throws `NotFoundException`: *"Note not found"* when the ID does not exist |
| `Test_GetNoteById_Returns_The_Expected_General_Exception` | Propagates a general exception with the message *"Failed to get note"* |
| `Test_DecryptNote_Successfully_Decrypts_Note` | Successfully decrypts a note and returns the plain text |
| `Test_DecryptNote_Will_Return_ValidationException_If_Password_Is_Empty` | Throws `ValidationException`: *"Password is missing"* |
| `Test_DecryptNote_Returns_InvalidPasswordException_If_Password_Is_Wrong` | Throws `InvalidPasswordException`: *"Wrong Password"* |
| `Test_DecryptContent_Returns_NotFoundException` | Throws `NotFoundException` when note does not exist |
| `Test_DecryptNote_Returns_CryptographicException` | Throws `CryptographicException` containing *"Decryption error"* on cryptographic failure |
| `Test_DecryptNote_Returns_Exception` | Propagates a general exception with the message *"Unexpected error during decryption"* |
| `Test_UpdateNote_updates_note_successfully` | `UpdateNote` calls `DatabaseService.Update` exactly once on success |
| `Test_UpdateNote_Returns_ValidationException_if_Content_Is_Missing` | Throws `ValidationException`: *"Content is empty"* |
| `Test_UpdateNote_Returns_ValidationException_if_Title_Is_Missing` | Throws `ValidationException`: *"Title is empty"* |
| `Test_UpdateNote_Returns_ValidationException_if_Password_Is_Missing` | Throws `ValidationException`: *"Password is missing"* |
| `Test_UpdateNote_Returns_InvalidPasswordException_if_Password_Is_Wrong` | Throws `InvalidPasswordException`: *"Wrong password"* |
| `Test_UpdateNote_Returns_NotFoundException_If_Note_Does_Not_Exist` | Throws `NotFoundException`: *"Note not found"* |
| `Test_UpdateNote_Returns_CryptographicException_If_Decryption_Fail` | Throws `CryptographicException` when decryption fails during update |
| `Test_UpdateNote_Returns_CryptographicException_If_Encryption_Fail` | Throws `CryptographicException` when re-encryption fails during update |
| `Test_ApplyEncryption_Successfully_Encrypts_Note` | Encrypts note and updates `Encrypted_content`, `Salt`, and `IV` properties correctly |
| `Test_ApplyEncryption_Returns_ValidationException_If_Password_Is_Empty` | Throws `ValidationException`: *"Password is missing"* |
| `Test_ApplyEncryption_Returns_ValidationException_If_Content_Is_Empty` | Throws `ValidationException`: *"Content is missing"* |
| `Test_ApplyEncryption_Returns_NotFoundException_If_Note_Is_Empty` | Throws `NotFoundException`: *"Note does not exist"* |
| `Test_that_AppleyEncryption_Returns_Successfully_CryptographicException` | Throws `CryptographicException`: *"Encryption failed"* |
| `Test_Delete_Sucessfully_Deletes_Notes` | `Delete` calls `DatabaseService.Delete` exactly once on success |
| `Test_Delete_Returns_NotFoundException_If_Note_Does_Not_Exist` | Throws `NotFoundException`: *"Note not found"* |

---

### `TestDatabaseService` (Integration Tests)

| Test | Description |
|---|---|
| `Test_Create_Successfully_Creates_Note_and_GetById_Successfully_Returns_The_Created_Note` | Creates a note and retrieves it by ID — verifies title, salt, and IV match |
| `Test_Create_Method_Returns_DatabaseException` | Throws `DatabaseException`: *"Failed to save note in the Database"* on failure |
| `Test_Update_Successfully_Updates_Notes` | Updates title and content and verifies the changes are persisted |
| `Test_Update_Returns_ValidationException_If_Note_Is_Null` | Throws `ValidationException`: *"Note does not exist"* when note is null |
| `Test_Update_Returns_NotFoundException_If_DB_Query_Return_0_Rows_Changed` | Throws `NotFoundException` when no rows are affected |
| `Test_Delete_Successfully_Delete_Notes` | Deletes a note and confirms `GetById` returns null afterwards |
| `Test_Delete_Returns_NotFoundException_If_Id_Does_Not_Exists` | Throws `NotFoundException`: *"Note does not exist"* |

---

### `TestCreateNoteViewModel`

| Test | Description |
|---|---|
| `Test_CreateNote_Successfully_Creates_Notes` | Calls `NoteService.CreateNote` with the correct title, content, and password exactly once |
| `Test_CreateNote_Returns_General_Exception` | Propagates a general exception from the service to the caller |

---

### `TestDecryptNoteViewModel`

| Test | Description |
|---|---|
| `Test_DecryptNote_Successfully_Decrypt_Notes` | Calls `NoteService.DecryptNote` and stores the result in `Decrypted_content` |
| `Test_DecryptNote_Successfully_Returns_General_Exception` | Propagates a general exception with the message *"Unexpected error during decryption"* |

---

### `TestUpdateNoteViewModel`

| Test | Description |
|---|---|
| `Test_LoadNote_Successfully_Returns_The_Requested_Note` | Calls `NoteService.GetNoteById` exactly once and stores the note |

---

### `TestNoteListViewModel`

| Test | Description |
|---|---|
| `Test_LoadNotes_Successfully_Loads_Existing_Notes` | Loads 2 notes and verifies titles match |
| `Test_LoadNotes_Returns_General_Exception` | Propagates a general exception with the message *"Failed to get notes"* |
| `Test_DeleteNote_Deletes_Note_With_Success` | Calls `NoteService.Delete` exactly once on success |
| `Test_DeleteNote_Returns_Exception` | Propagates a general exception with the message *"Error.Deletion failed"* |
| `Test_DeleteNote_Removes_Deleted_Notes_From_The_List` | Removes the deleted note from the `Notes` collection and verifies count is 0 |
| `Test_FindNoteByTitle_Successfully_Returns_Title` | Returns the correct note when the title matches |
| `Test_FindNoteByTitle_Returns_NotFoundException` | Throws `NotFoundException`: *"Note does not exist"* when the notes collection is empty |

---

### `Test_ViewNoteViewModel`

| Test | Description |
|---|---|
| `Test_LoadNote_Successfully_Returns_The_Requested_Note` | Calls `NoteService.GetNoteById` and stores the note in the `Note` property |
| `Test_LoadNote_Successfully_Returns_General_Exception` | Propagates a general exception with the message *"Failed to get note"* |
| `Test_Decrypt_Success_Decrypts_Note` | Calls `NoteService.DecryptNote` and stores the result in `DecryptedContent` |
| `Test_Decrypt_Returns_General_Exception_With_Success` | Propagates a general exception with the message *"Unexpected error during decryption"* |
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
