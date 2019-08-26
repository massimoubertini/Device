using System;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Primo_Progetto_SD
{
    public sealed partial class MainPage : Page
    {
        private RadialController controller;

        public MainPage()
        {
            this.InitializeComponent();
            // inizializzo l'oggetto
            controller = RadialController.CreateForCurrentView();
            // creo una icona per lo strumento personalizzato
            RandomAccessStreamReference icon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/StoreLogo.png"));
            // creo un menu item per lo strumento personalizzato
            RadialControllerMenuItem myItem = RadialControllerMenuItem.CreateFromIcon("Sample", icon);
            // aggiungo lo strumento personalizzato al menu di RadialController
            controller.Menu.Items.Add(myItem);
            // aggiungo gli handler per gli eventi del RadialController
            controller.ButtonClicked += Controller_ButtonClicked;
            controller.RotationChanged += Controller_RotationChanged;
        }

        // Handler per la rotazione del RadialController
        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (RotationSlider.Value + args.RotationDeltaInDegrees > 100)
            {
                RotationSlider.Value = 100;
                return;
            }
            else if (RotationSlider.Value + args.RotationDeltaInDegrees < 0)
            {
                RotationSlider.Value = 0;
                return;
            }
            RotationSlider.Value += args.RotationDeltaInDegrees;
        }

        // Handler il clic del RadialController.
        private void Controller_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        { ButtonToggle.IsOn = !ButtonToggle.IsOn; }
    }
}