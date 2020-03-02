using System.Windows.Media;

namespace WPFCRUD.ViewModels {
    public class ErrorProviderViewModel : BaseViewModel {
        private string _errorName;
        public string ErrorName {
            get => _errorName;
            set {
                _errorName = value;
                if (value == string.Empty) {
                    ToolTipNameEnable = false;
                    BorderColor = Brushes.Gray;
                } else {
                    ToolTipNameEnable = true;
                    BorderColor = Brushes.Red;
                }
                OnPropertyChanged(nameof(ErrorName));
            }
        }

        public Brush BorderColor { get; set; }

        public bool ToolTipNameEnable { get; set; }
    }
}