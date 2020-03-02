using System;
using System.Windows;
using WPFCRUD.ViewModels;

namespace WPFCRUD.Views {
    /// <summary>
    /// Interaction logic for ModifyingWindow.xaml
    /// </summary>
    public partial class ModifyingWindow : Window {

        public ModifyingWindow(ModifyingWindowViewModel dataContext) {
            InitializeComponent();
            DataContext = dataContext;
            dataContext.CloseAction = Close;
        }
    }
}
