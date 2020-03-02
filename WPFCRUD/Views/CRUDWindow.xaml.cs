using System.Windows;

namespace WPFCRUD.Views {
    /// <summary>
    /// Interaction logic for CRUDWindow.xaml
    /// </summary>
    public partial class CRUDWindow : Window {
        public static CRUDWindow Instance;

        public CRUDWindow() {
            InitializeComponent();
            Instance = this;
        }
    }
}
