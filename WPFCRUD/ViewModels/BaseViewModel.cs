using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;
using WPFCRUD.Annotations;

namespace WPFCRUD.ViewModels {
    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}