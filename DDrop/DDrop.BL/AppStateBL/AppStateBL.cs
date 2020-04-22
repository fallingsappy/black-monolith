using DDrop.Controls.LoadingSpinner;

namespace DDrop.BL.AppStateBL
{
    public class AppStateBL : IAppStateBL
    {
        public void ShowAdorner(AdornedControl adornedControl)
        {
            adornedControl.IsAdornerVisible = true;
        }

        public void HideAdorner(AdornedControl adornedControl)
        {
            adornedControl.IsAdornerVisible = false;
        }
    }
}