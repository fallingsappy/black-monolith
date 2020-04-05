using DDrop.BE.Models;
using DDrop.Utility.Cryptography;
using DDrop.Utility.ImageOperations;
using System;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DDrop.BL.Series;
using DDrop.DAL;
using DDrop.Utility.Mappers;
using ToastNotifications;
using ToastNotifications.Messages;
using System.Windows.Controls;
using System.Reflection;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static readonly DependencyProperty LocalStoredUsersProperty = DependencyProperty.Register("LocalStoredUsers", typeof(LocalStoredUsers), typeof(Login));
        public LocalStoredUsers LocalStoredUsers
        {
            get => (LocalStoredUsers)GetValue(LocalStoredUsersProperty);
            set => SetValue(LocalStoredUsersProperty, value);
        }
        
        public static readonly DependencyProperty UserLoginProperty = DependencyProperty.Register("UserLogin", typeof(User), typeof(Login));
        private IDDropRepository _dDropRepository;
        private readonly Notifier _notifier;
        public bool LoginSucceeded;
        public User UserLogin
        {
            get { return (User)GetValue(UserLoginProperty); }
            set
            {
                SetValue(UserLoginProperty, value);
            }
        }

        public Login(IDDropRepository dDropRepository, Notifier notifier)
        {
            _dDropRepository = dDropRepository;
            _notifier = notifier;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.StoredUsers))
            {
                LocalStoredUsers = JsonSerializeProvider.DeserializeFromString<LocalStoredUsers>(Properties.Settings.Default.StoredUsers);
            }
            
            InitializeComponent();

            DataContext = this;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";
            if (TextBoxEmail.Text.Length == 0)
            {
                ErrorMessage.Text = "Введите электронную почту.";
                TextBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(TextBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                ErrorMessage.Text = "Электронная почта имеет не верный формат.";
                TextBoxEmail.Select(0, TextBoxEmail.Text.Length);
                TextBoxEmail.Focus();
            }
            else
            {
                var email = TextBoxEmail.Text;
                var password = LoginPasswordBox.Password;

                await TryLogin(email, password, true);
            }
        }

        private async Task TryLogin(string email, string password, bool shouldRemember)
        {
            try
            {
                LoginWindowLoading();
                var user = await Task.Run(() => _dDropRepository.GetUserByLogin(email));

                if (user != null && PasswordOperations.PasswordsMatch(password, user.Password))
                {
                    UserLogin = DDropDbEntitiesMapper.DbUserToUser(user);
                    UserLogin.UserSeries = DDropDbEntitiesMapper.DbSeriesToSeries(user.UserSeries, UserLogin);

                    LoginSucceeded = true;

                    if (RememberMe.IsChecked == true && shouldRemember)
                    {
                        StoreUserLocal(email, password);
                    }

                    _notifier.ShowSuccess($"Пользователь {user.Email} авторизован.");
                    LoginWindowLoading();
                    Close();
                }
                else
                {
                    ErrorMessage.Text = "Неверный логин или пароль.";
                    LoginSucceeded = false;
                    LoginWindowLoading();
                }
            }
            catch
            {
                LoginWindowLoading();
                _notifier.ShowError("Не удалось установить соединение. Проверьте интернет подключение.");
            }
        }

        private void StoreUserLocal(string email, string password)
        {
            bool newUser = true;

            if (LocalStoredUsers?.Users != null)
            {
                foreach (var localUser in LocalStoredUsers.Users)
                {
                    if (localUser.Login == email)
                    {
                        newUser = false;

                        if (localUser.Password != PasswordOperations.HashPassword(password))
                        {
                            localUser.Password = password;

                            Properties.Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString<LocalStoredUsers>(LocalStoredUsers);

                            Properties.Settings.Default.Save();
                        }


                        break;
                    }
                }
            }
            else
            {
                LocalStoredUsers = new LocalStoredUsers();
                LocalStoredUsers.Users = new System.Collections.ObjectModel.ObservableCollection<LocalStoredUser>();
            }

            if (newUser)
            {
                LocalStoredUsers.Users.Add(new LocalStoredUser
                {
                    Login = email,
                    Password = password
                });

                Properties.Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString<LocalStoredUsers>(LocalStoredUsers);

                Properties.Settings.Default.Save();
            }
        }

        private void LoginWindowLoading()
        {
            LoginButton.IsEnabled = !LoginButton.IsEnabled;
            RegistrationButton.IsEnabled = !RegistrationButton.IsEnabled;
            LoadingAdorner.IsAdornerVisible = !LoadingAdorner.IsAdornerVisible;
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Registration registrationWindow = new Registration(_dDropRepository, _notifier)
            {
                Owner = this
            };
            registrationWindow.ShowDialog();
            Visibility = Visibility.Visible;

            if (registrationWindow.RegistrationSucceeded)
            {
                UserLogin = registrationWindow.UserLogin;
                LoginSucceeded = true;
                Close();
            }
            else
            {
                LoginSucceeded = false;
                ShowDialog();
            }
        }

        private void TextBoxLogin_OnTextChanged(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            ErrorMessage.Text = "";
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBox.Visibility = Visibility.Hidden;
            PasswordUnmask.Visibility = Visibility.Visible;
            PasswordUnmask.Text = LoginPasswordBox.Password;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBox.Visibility = Visibility.Visible;
            PasswordUnmask.Visibility = Visibility.Hidden;
            LoginPasswordBox.Password = PasswordUnmask.Text;
        }

        private void LoginOfflineButton_Click(object sender, RoutedEventArgs e)
        {
            UserLogin = new User()
            {
                Email = "anonymousUser@anonymousUser.com",
                UserPhoto = ImageInterpreter.ImageToByteArray(Properties.Resources.cool_profile_picture_300x219_vectorized__1_),
                FirstName = "Неизвестно",
                LastName = "Неизвестно",
                IsLoggedIn = false,
                Password = PasswordOperations.HashPassword("1q2w3e4r5t6y"),
                UserId = Guid.NewGuid()
            };
            LoginSucceeded = true;

            Close();
        }

        private void LoginPasswordBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void PasswordUnmask_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox1)
            {
                ErrorMessage.Text = "";

                if (string.IsNullOrWhiteSpace(passwordBox1.Password.Trim()))
                {
                    PasswordUnmask.Text = passwordBox1.Password;
                    SetSelection(passwordBox1, passwordBox1.Password.Length, passwordBox1.Password.Length);
                }
            }
        }

        private void PasswordUnmask_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Text = "";

            var textBox1 = sender as TextBox;
            LoginPasswordBox.Password = textBox1?.Text != null ? textBox1.Text : "";
        }

        private async void LocalUsersCombobox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;

            if (comboBox.SelectedIndex > -1)
            {
                var storedUser = (LocalStoredUser)comboBox.SelectedItem;

                LoginPasswordBox.Password = storedUser.Password;
                TextBoxEmail.Text = storedUser.Login;
                PasswordUnmask.Text = storedUser.Password;

                await TryLogin(storedUser.Login, storedUser.Password, false);
            }
        }
    }
}
