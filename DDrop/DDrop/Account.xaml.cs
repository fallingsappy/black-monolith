using DDrop.BE.Models;
using DDrop.Utility.ImageOperations;
using Microsoft.Win32;
using System.Windows;

namespace DDrop
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(User), typeof(Account));

        public User User
        {
            get { return (User)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public Account(User user)
        {
            InitializeComponent();

            User = user;

            ProfilePicture.Source = ImageInterpreter.LoadImage(User.UserPhoto);
        }

        private void ChooseProfilePicture_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = true;
            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.Reference = openFileDialog.FileName;
                User.UserPhoto = ImageInterpreter.FileToByteArray(openFileDialog.FileName);
                ProfilePicture.Source = ImageInterpreter.LoadImage(User.UserPhoto);
            }
        }
    }
}
