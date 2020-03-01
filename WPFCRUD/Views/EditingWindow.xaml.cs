using System.Windows;
using System.Windows.Input;
using WPFCRUD.Models;
using WPFCRUD.ViewModels;

namespace WPFCRUD.Views {
    /// <summary>
    /// Interaction logic for EditingWindow.xaml
    /// </summary>
    public partial class EditingWindow : Window {
        public static EditingWindow Instance;
        public User CurrentUser;

        public EditingWindow(User user) {
            CurrentUser = user;
            Instance = this;
            InitializeComponent();
            TbPassword.LostFocus += VmMainWindow.TbPassword_OnLostFocus;
            TbPassword.PasswordChanged += VmMainWindow.TbPassword_OnPasswordChanged;

            PreviewKeyDown += (sender, e) => {
                if (e.Key == Key.Escape)
                    Close();
            };
        }
    }
}
