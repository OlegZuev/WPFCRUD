using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCRUD.Models;
using WPFCRUD.Views;

namespace WPFCRUD.ViewModels {
    public class CRUDWindowViewModel : BaseViewModel {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public CRUDWindowViewModel() {
            DatabaseInteraction.InitializeToUsers(Users);

            AddUserCommand = new DelegateCommand<object>(sender => {
                new ModifyingWindow(new RegistrationWindowViewModel()).ShowDialog();
            });

            ResizeListView = new DelegateCommand<ListView>(sender => {
                var gv = (GridView) sender.View;
                foreach (GridViewColumn gridViewColumn in gv.Columns) {
                    if (double.IsNaN(gridViewColumn.Width)) {
                        gridViewColumn.Width = gridViewColumn.ActualWidth;
                    }

                    gridViewColumn.Width = double.NaN; //-V3008
                }
            });

            Database.DatabaseInteraction.NewUserAdded += id => { DatabaseInteraction.ShowNewUser(Users, id); };

            ChangeUserCommand = new DelegateCommand<User>(currentUser => {
                new ModifyingWindow(new EditingWindowViewModel(currentUser)).ShowDialog();
            }, currentUser => currentUser != null);

            Database.DatabaseInteraction.SomeUserChanged += id => {
                DatabaseInteraction.LoadUserInfo(Users, id);
                OnPropertyChanged(nameof(Users));
            };

            DeleteUserCommand = new DelegateCommand<User>(
                currentUser => { DatabaseInteraction.DeleteUser(Users, currentUser); },
                currentUser => currentUser != null);
        }

        public ICommand AddUserCommand { get; set; }

        public ICommand ResizeListView { get; set; }

        public ICommand ChangeUserCommand { get; set; }

        public ICommand DeleteUserCommand { get; set; }
    }
}