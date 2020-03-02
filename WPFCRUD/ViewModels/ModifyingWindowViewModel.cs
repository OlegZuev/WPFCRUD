using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Database;
using WPFCRUD.Models;

namespace WPFCRUD.ViewModels {
    public abstract class ModifyingWindowViewModel : BaseViewModel {
        public abstract string WindowTitle { get; }

        public abstract string ModifyUserCommandTitle { get; }

        protected readonly Database.DatabaseInteraction DatabaseInstance;

        private readonly Dictionary<int, string> _imgStrengthPaths = new Dictionary<int, string> {
            {0, "../Images/PasswordStrength0.png"},
            {1, "../Images/PasswordStrength1.png"},
            {2, "../Images/PasswordStrength2.png"},
            {3, "../Images/PasswordStrength3.png"},
            {4, "../Images/PasswordStrength4.png"}
        };

        public string TbLoginText { get; set; }

        public ErrorProviderViewModel LoginErrorProvider { get; set; }

        public ErrorProviderViewModel PasswordErrorProvider { get; set; }

        public string ImgPasswordStrengthPath { get; set; }

        public Visibility ImgPasswordVisibility { get; set; }

        public DateTime RegistrationDate { get; set; }

        protected ModifyingWindowViewModel() {
            try {
                DatabaseInstance = new Database.DatabaseInteraction();
            } catch (Npgsql.PostgresException e) {
                MessageBox.Show(e.Message);
                throw;
            }

            LoginErrorProvider = new ErrorProviderViewModel {BorderColor = Brushes.Gray};
            PasswordErrorProvider = new ErrorProviderViewModel {BorderColor = Brushes.Gray};
            RegistrationDate = DateTime.Now;

            LoginChangedCommand = new DelegateCommand<object>(sender => {
                DatabaseInstance.CheckLogin(TbLoginText);
            });

            DatabaseInstance.PasswordStrengthChanged += (type, o) => {
                if (type == EventStringTypes.PasswordStrengthIndex) {
                    ImgPasswordStrengthPath = _imgStrengthPaths[(int)o];
                }
            };

            PasswordChangedCommand = new DelegateCommand<PasswordBox>(sender => {
                DatabaseInstance.CheckPassword(sender.Password);
                ImgPasswordVisibility = Visibility.Visible;
            });

            PasswordLostFocusCommand = new DelegateCommand<object>(sender => {
                ImgPasswordVisibility = Visibility.Hidden;
            });
        }

        public Action CloseAction { get; set; }

        public ICommand LoginChangedCommand { get; }

        public ICommand PasswordChangedCommand { get; }

        public ICommand PasswordLostFocusCommand { get; }

        public abstract ICommand ModifyUserCommand { get; protected set; }
    }
}