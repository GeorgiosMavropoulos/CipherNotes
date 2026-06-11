using Cipher_Notes.Views;
using Cipher_Notes.Views;

namespace Cipher_Notes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NoteListPage()) { Title = "Cipher Notes" };
        }
    }
}
