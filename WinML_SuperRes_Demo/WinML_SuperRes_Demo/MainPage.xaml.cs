using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinML_SuperRes_Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private esrgan_lite_op7_rs6_nchwModel superModelGen;
        private esrgan_lite_op7_rs6_nchwInput superInput = new esrgan_lite_op7_rs6_nchwInput();
        private esrgan_lite_op7_rs6_nchwOutput superOutput;
        private SoftwareBitmap selectedImage;
        public MainPage()
        {
            this.InitializeComponent();
            LoadSuperModelAsync();

        }

        private async Task LoadSuperModelAsync()
        {
            //Load a machine learning model
            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/esrgan_lite_op7_rs6_nchw.onnx"));
            superModelGen = await esrgan_lite_op7_rs6_nchwModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);
        }

        private async void pickImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".png");

            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;

            var inputFile = await fileOpenPicker.PickSingleFileAsync();

            if (inputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }

            using (IRandomAccessStream stream = await inputFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                selectedImage = await decoder.GetSoftwareBitmapAsync();
            }
            selectedImage = SoftwareBitmap.Convert(selectedImage, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            var selectedImgSrc = new SoftwareBitmapSource();
            OriginalImg.Source = selectedImgSrc;
            await selectedImgSrc.SetBitmapAsync(selectedImage);
        }

        private async void generateImage_Click(object sender, RoutedEventArgs e)
        {
            VideoFrame inputVidFrame = VideoFrame.CreateWithSoftwareBitmap(selectedImage);
            ImageFeatureValue loadedImg = ImageFeatureValue.CreateFromVideoFrame(inputVidFrame);

            superInput.input_0 = loadedImg;
            superOutput = await superModelGen.EvaluateAsync(superInput);

            var upsrc = new SoftwareBitmapSource();
            UpsclImg.Source = upsrc;

            SoftwareBitmap outImg = superOutput.Identity.VideoFrame.SoftwareBitmap;
            outImg = SoftwareBitmap.Convert(outImg, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            await upsrc.SetBitmapAsync(outImg);
        }

    }
}
