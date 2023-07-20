using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using MPLATFORMLib;

namespace Sample1
{
    public partial class MainWindow : Window
    {
        private MPreviewClass _myPreview;
        private D3DImage _previewSource;
        private MFileClass _myFile;
        
        string pathToFile = @"udp://239.1.2.3:12345";

        public MainWindow()
        {

            InitializeComponent();
            StartPlaying();
            SetSourceName();


        }

        private void StartPlaying()
        {
            _myFile = new MFileClass();
            _myFile.FileNameSet(pathToFile, "");
            _myPreview = new MPreviewClass();
            _myPreview.PropsSet("wpf_preview", "true");
            _myPreview.PreviewEnable("", 1, 1);
            _myPreview.OnEventSafe += _myPreview_OnEventSafe;
            _myFile.FilePlayStart();
            _myPreview.ObjectStart(_myFile);
            _previewSource = new D3DImage();

        }

        private void SetSourceName()
        {        
        textBlock1.SetCurrentValue(TextBlock.TextProperty, pathToFile);
         
        }

        private IntPtr _mSavedPointer; // required to update image context;
        private void _myPreview_OnEventSafe(string bsChannelId, string bsEventName, string bsEventParam, object pEventObject)
        {
            if (bsEventName == "wpf_nextframe")
            {
                IntPtr pEventObjectPtr = Marshal.GetIUnknownForObject(pEventObject);
                if (pEventObjectPtr != _mSavedPointer)
                {
                    if (_mSavedPointer != IntPtr.Zero)
                        Marshal.Release(_mSavedPointer);

                    _mSavedPointer = pEventObjectPtr;
                    Marshal.AddRef(_mSavedPointer);

                    _previewSource.Lock();
                    _previewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                    //enableSoftwareFallback = true, 
                    //enable an ability D3DImage display the Direct3D content in software renderer like RDP                    
                    _previewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _mSavedPointer, true);

                    _previewSource.Unlock();
                    previewArea.Source = _previewSource;
                }

                if (pEventObjectPtr != IntPtr.Zero)
                    Marshal.Release(pEventObjectPtr);

                _previewSource.Lock();
                _previewSource.AddDirtyRect(new Int32Rect(0, 0, _previewSource.PixelWidth, _previewSource.PixelHeight));
                _previewSource.Unlock();
            }
            Marshal.ReleaseComObject(pEventObject);

  
        }

    }

}
