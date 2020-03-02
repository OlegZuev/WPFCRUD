using System.Windows;
using WPFCRUD.ViewModels;

namespace WPFCRUD.Views {
    /// <summary>
    /// Interaction logic for CRUDWindow.xaml
    /// </summary>
    public partial class CRUDWindow : Window {
        public CRUDWindow() {
            InitializeComponent();
            DataContext = new CRUDWindowViewModel();
        }
    }
}
