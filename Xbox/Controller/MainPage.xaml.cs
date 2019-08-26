using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace XBox1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            // intercetto l'evento KeyDown
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            loadFiles();
        }

        private async void loadFiles()
        {
            // setto la cartella di partenza con la lista dei dispositivi esterni
            StorageFolder picturesFolder = KnownFolders.RemovableDevices;
            // aggiungo la cartella all'albero delle cartelle visitate
            selFolders.Add(picturesFolder);
            // popolo la ListView
            readFolder(picturesFolder);
        }

        // albero delle cartelle visitate
        private List<StorageFolder> selFolders = new List<StorageFolder>();

        private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            Debug.WriteLine(args.VirtualKey.ToString());
            // controllo il tasto premuto
            switch (args.VirtualKey)
            {
                case VirtualKey.GamepadA:
                    // e c'è un elemento selezionato
                    if (listView.SelectedItem != null)
                    {
                        // ottengo l'elemento di storage selezionato
                        IStorageItem selFolder = (IStorageItem)listView.SelectedItem;
                        // se è una cartella
                        if (selFolder.IsOfType(StorageItemTypes.Folder))
                        {
                            // aggiungo la cartella all'albero delle cartelle visitate
                            selFolders.Add((StorageFolder)selFolder);
                            // popolo la ListView
                            await readFolder((StorageFolder)selFolder);
                        }
                        else
                            // visualizzo il file
                            await showImage((StorageFile)selFolder);
                    }
                    break;

                // se è stato premuto il tasto B
                case VirtualKey.GamepadB:
                    // se non sono nella root torno alla cartella superiore
                    if (selFolders.Count > 1)
                    {
                        // elimino l'ultima cartella dalla lista
                        selFolders.RemoveAt(selFolders.Count - 1);
                        // visualizzo la cartella corrente
                        await readFolder(selFolders[selFolders.Count - 1]);
                    }
                    break;
                // se è stato premuto il tasto View
                case VirtualKey.GamepadView:
                    // se è visualizzata la lista dei file la nascondo
                    if (rightGrid.Visibility == Visibility.Visible)
                        rightGrid.Visibility = Visibility.Collapsed;
                    // altrimenti se è nascosta la visualizzo
                    else
                        rightGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        // visualizza il file corrente e controlla se è di tipo immagine o video
        private async Task showImage(StorageFile file)
        {
            // blocco eventuali video visualizzati
            me.Stop();
            // se il file è un'immagine
            if (file.Name.EndsWith(".jpg"))
            {
                // assegno l'immagine corrente al controllo Image
                BitmapImage bi = new BitmapImage();
                await bi.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                Image1.Source = bi;
                // visualizzo il controllo immagine e nascondo il controllo video
                me.Visibility = Visibility.Collapsed;
                Image1.Visibility = Visibility.Visible;
            } // se il file è un video
            if (file.Name.EndsWith(".avi") || file.Name.EndsWith(".mp4"))
            {
                // imposto il file come sorgente
                me.SetSource(await file.OpenAsync(FileAccessMode.Read), file.ContentType);
                // avvio la riproduzione
                me.Play();
                loaded = true;
                // visualizzo il controllo video e nascondo il controllo immagine
                me.Visibility = Visibility.Visible;
                Image1.Visibility = Visibility.Collapsed;
            }
        }

        private bool loaded = false;

        // legge file e cartelle presenti nella cartella passata come parametro
        private async Task readFolder(StorageFolder picturesFolder)
        {
            // creo una lista di oggetti generici di storage in modo da poter inserire sia cartelle sia file
            List<IStorageItem> storageItems = new List<IStorageItem>();
            IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();
            IReadOnlyList<StorageFolder> folderList = await picturesFolder.GetFoldersAsync();
            foreach (StorageFolder folder in folderList)
            {
                // aggiungo la cartella
                storageItems.Add(folder);
                //if(folder.DisplayName== "ioprogrammo")
                //await readFolder(folder, outputText);
            }
            foreach (StorageFile file in fileList)
            {
                // aggiungo solo i file compatibili
                storageItems.Add(file);
                if (file.Name.EndsWith(".jpg"))
                {
                    BitmapImage bi = new BitmapImage();
                    await bi.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                    Image1.Source = bi;
                }
                if (file.Name.EndsWith(".avi") && !loaded)
                {
                    me.SetSource(await file.OpenAsync(FileAccessMode.Read), file.ContentType);
                    me.Play();
                    loaded = true;
                }
            }
            // binding della lista dei file con la ListView
            listView.ItemsSource = storageItems;
        }

        private void ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            readFolder((StorageFolder)((ListView)sender).SelectedItem);
        }

        private void ListView_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            readFolder((StorageFolder)((ListView)sender).SelectedItem);
        }

        // controllo se un file è un'immagine
        public bool isPictureFile(String fileName)
        {   // estraggo l'estensione dei file
            String ext = Path.GetExtension(fileName);
            return ext == ".jpg" || ext == ".png";
        }

        // controllo se un file è un video
        public bool isVideoFile(String fileName)
        {   // estraggo l'estensione dei file
            String ext = Path.GetExtension(fileName);
            return ext == ".avi" || ext == ".mp4";
        }
    }
}