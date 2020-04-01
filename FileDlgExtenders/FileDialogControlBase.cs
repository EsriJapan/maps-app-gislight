//  Copyright (c) 2006, Gustavo Franco
//  Copyright ｩ Decebal Mihailescu 2007
//  Email:  dmihailescu@hotmail.com
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using Win32Types;

namespace ESRIJapan.FileDlgExtenders
{
    #region Base class

    public partial class FileDialogControlBase : UserControl
    {
        #region Delegates
        public delegate void PathChangedEventHandler(IWin32Window sender, string filePath);
        public delegate void FilterChangedEventHandler(IWin32Window sender, int index);
        #endregion

        #region Events
        //for weird reasons the designer wants the events public not protected    
        [Category("FileDialogExtenders")]
        public event PathChangedEventHandler EventFileNameChanged;
        [Category("FileDialogExtenders")]
        public event PathChangedEventHandler EventFolderNameChanged;
        [Category("FileDialogExtenders")]
        public event FilterChangedEventHandler EventFilterChanged;
        [Category("FileDialogExtenders")]
        public event CancelEventHandler EventClosingDialog;
        #endregion

        #region Constants Declaration
        private const SetWindowPosFlags UFLAGSHIDE =
            SetWindowPosFlags.SWP_NOACTIVATE |
            SetWindowPosFlags.SWP_NOOWNERZORDER |
            SetWindowPosFlags.SWP_NOMOVE |
            SetWindowPosFlags.SWP_NOSIZE |
            SetWindowPosFlags.SWP_HIDEWINDOW;
        #endregion

        #region Variables Declaration
        System.Windows.Forms.FileDialog _MSdialog;
        NativeWindow _dlgWrapper;
        private AddonWindowLocation _StartLocation = AddonWindowLocation.Right;
        private FolderViewMode _DefaultViewMode = FolderViewMode.Default;
        IntPtr _hFileDialogHandle = IntPtr.Zero;
        FileDialogType _FileDlgType;
        string _InitialDirectory = string.Empty;
        string _Filter = "All files (*.*)|*.*";
        string _FilterFileTypes = ""; // add
        string[] _FileTypes = null; // add

        string _DefaultExt = "jpg";
        string _FileName = string.Empty;
        string _Caption = "Save";
        string _OKCaption = "&エクスポート"; // modify
        int _FilterIndex = 1;
        bool _AddExtension = true;
        bool _CheckFileExists = true;
        bool _CheckPathExists = true; // add
        bool _EnableOkBtn = true;
        bool _DereferenceLinks = true;
        bool _ShowHelp;
        RECT _OpenDialogWindowRect = new RECT();
        IntPtr _hOKButton = IntPtr.Zero;
        private bool _hasRunInitMSDialog;
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        IntPtr _hListViewPtr;

        #endregion

        #region Constructors
        public FileDialogControlBase()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        static uint _originalDlgHeight, _originalDlgWidth;

        internal static uint OriginalDlgWidth
        {
            get { return FileDialogControlBase._originalDlgWidth; }
            set { FileDialogControlBase._originalDlgWidth = value; }
        }

        internal static uint OriginalDlgHeight
        {
            get { return FileDialogControlBase._originalDlgHeight; }
            set { FileDialogControlBase._originalDlgHeight = value; }
        }
    
        [Browsable(false)]
        public string[] FileDlgFileNames
        {
            get { return DesignMode ? null : MSDialog.FileNames; }
        }
        
        // add
        [Browsable(false)]
        public string FilterFileTypes
        {
            set { this._FilterFileTypes = value; }
            get { return this._FilterFileTypes; }
        }
        [Browsable(false)]
        public string Caption
        {
            set { this._Caption = value; }
            get { return this._Caption; }
        }
        [Browsable(false)]
        public string FileName
        {
            set { this._FileName = value; }
            get { return this._FileName; }
        }

        [Browsable(false)]
        public string Filter
        {
            set { this._Filter = value; }
            get { return this._Filter; }
        }

        [Browsable(false)]
        public int FilterIndex
        {
            set { this._FilterIndex = value; }
            get { return this._FilterIndex; }
        }
        // add end

        [Browsable(false)]
        public FileDialog MSDialog
        {
            set { _MSdialog = value; }
            get { return _MSdialog; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(AddonWindowLocation.Right)]
        public AddonWindowLocation FileDlgStartLocation
        {
            get { return _StartLocation; }
            set
            {
                _StartLocation = value;
                if (DesignMode)
                {
                    this.Refresh();
                }
            }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(FolderViewMode.Default)]
        public FolderViewMode FileDlgDefaultViewMode
        {
            get { return _DefaultViewMode; }
            set { _DefaultViewMode = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(FileDialogType.OpenFileDlg)]
        public FileDialogType FileDlgType
        {
            get { return _FileDlgType; }
            set { _FileDlgType = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("")]
        public string FileDlgInitialDirectory
        {
            get { return DesignMode ? _InitialDirectory : MSDialog.InitialDirectory; }
            set
            {
                _InitialDirectory = value;
                if (!DesignMode)
                    MSDialog.InitialDirectory = value;
            }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("")]
        public string FileDlgFileName
        {
            get { return DesignMode ? _FileName : MSDialog.FileName; }
            set { _FileName = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("")]
        public string FileDlgCaption
        {
            get { return _Caption; }
            set { _Caption = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("&Open")]
        public string FileDlgOkCaption
        {
            get { return _OKCaption; }
            set { _OKCaption = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("jpg")]
        public string FileDlgDefaultExt
        {
            get { return DesignMode ? _DefaultExt : MSDialog.DefaultExt; }
            set { _DefaultExt = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue("All files (*.*)|*.*")]
        public string FileDlgFilter
        {
            get { return DesignMode ? _Filter : MSDialog.Filter; }
            set { _Filter = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(1)]
        public int FileDlgFilterIndex
        {
            get { return DesignMode ? _FilterIndex : MSDialog.FilterIndex; }
            set { _FilterIndex = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(true)]
        public bool FileDlgAddExtension
        {
            get { return DesignMode ? _AddExtension : MSDialog.AddExtension; }
            set { _AddExtension = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(true)]
        public bool FileDlgEnableOkBtn
        {
            get { return _EnableOkBtn; }
            set
            {
                _EnableOkBtn = value;
                if (!DesignMode && MSDialog != null && _hOKButton != IntPtr.Zero)
                    Win32Types.NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn);
            }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(true)]
        public bool FileDlgCheckFileExists
        {
            get { return DesignMode ? _CheckFileExists : MSDialog.CheckFileExists; }
            set
            { _CheckFileExists = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(false)]
        public bool FileDlgShowHelp
        {
            get { return DesignMode ? _ShowHelp : MSDialog.ShowHelp; }
            set { _ShowHelp = value; }
        }

        [Category("FileDialogExtenders")]
        [DefaultValue(true)]
        public bool FileDlgDereferenceLinks
        {
            get { return DesignMode ? _DereferenceLinks : MSDialog.DereferenceLinks; }
            set { _DereferenceLinks = value; }
        }
        #endregion

        #region Virtuals

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                if (MSDialog != null)
                {
                    MSDialog.FileOk += new CancelEventHandler(FileDialogControlBase_ClosingDialog);
                    MSDialog.Disposed += new EventHandler(FileDialogControlBase_DialogDisposed);
                    MSDialog.HelpRequest += new EventHandler(FileDialogControlBase_HelpRequest);
                    //Win32Types.NativeMethods.EnableWindow(_hOKButton, _EnableOKBtn);
                    FileDlgEnableOkBtn = _EnableOkBtn;//that's desigh time value
                    NativeMethods.SetWindowText(_dlgWrapper.Handle, _Caption);

                    //will work only for open dialog, save dialog will not update
                    NativeMethods.SetWindowText(_hOKButton, _OKCaption);//SetDlgItemText fails too 
                    //bool res = 
                    //NativeMethods.SetDlgItemText(_FileDialogHandle, (int)ControlsId.ButtonOk, _OKCaption);
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (MSDialog != null)
            {
                MSDialog.FileOk -= new CancelEventHandler(FileDialogControlBase_ClosingDialog);
                MSDialog.Disposed -= new EventHandler(FileDialogControlBase_DialogDisposed);
                //if (MSDialog.ShowHelp)
                MSDialog.HelpRequest -= new EventHandler(FileDialogControlBase_HelpRequest);
                MSDialog.Dispose();
                MSDialog = null;
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual void OnFileNameChanged(IWin32Window sender, string fileName)
        {
            if (EventFileNameChanged != null)
                EventFileNameChanged(sender, fileName);

            this._FileName = fileName;

        }

        public void OnFolderNameChanged(IWin32Window sender, string folderName)
        {
            if (EventFolderNameChanged != null)
                EventFolderNameChanged(sender, folderName);
            UpdateListView();
        }

        private void UpdateListView()
        {
            _hListViewPtr = 
                Win32Types.NativeMethods.GetDlgItem(
                _hFileDialogHandle, (int)ControlsId.DefaultView);
            
            if (FileDlgDefaultViewMode != FolderViewMode.Default && 
                _hFileDialogHandle != IntPtr.Zero)
                NativeMethods.SendMessage(
                    new HandleRef(
                        this, _hListViewPtr), 
                        (int)Msg.WM_COMMAND, 
                        (IntPtr)(int)FileDlgDefaultViewMode, IntPtr.Zero);
        }

        internal void OnFilterChanged(IWin32Window sender, int index)
        {
            this._FilterIndex = index;

            if (EventFilterChanged != null)
            {
                EventFilterChanged(sender, index);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                Graphics gr = e.Graphics;
                {
                    HatchBrush hb = null;
                    Pen p = null;
                    try
                    {
                        switch (this.FileDlgStartLocation)
                        {
                            case AddonWindowLocation.Right:
                                hb = new System.Drawing.Drawing2D.HatchBrush(
                                    HatchStyle.NarrowHorizontal, Color.Black, Color.Red);
                                p = new Pen(hb, 5);
                                gr.DrawLine(p, 0, 0, 0, this.Height);
                                break;
                            case AddonWindowLocation.Bottom:
                                hb = new System.Drawing.Drawing2D.HatchBrush(
                                    HatchStyle.NarrowVertical, Color.Black, Color.Red);
                                p = new Pen(hb, 5);
                                gr.DrawLine(p, 0, 0, this.Width, 0);
                                break;
                            case AddonWindowLocation.BottomRight:
                            default:
                                hb = new System.Drawing.Drawing2D.HatchBrush(
                                    HatchStyle.Sphere, Color.Black, Color.Red);
                                p = new Pen(hb, 5);
                                gr.DrawLine(p, 0, 0, 4, 4);
                                break;
                        }
                    }
                    finally
                    {
                        if (p != null)
                            p.Dispose();
                        if (hb != null)
                            hb.Dispose();
                    }
                }
            }
            base.OnPaint(e);
        }


        #endregion

        #region Methods
        public DialogResult ShowDialog()
        {
            // add
            if (this._FilterFileTypes != null && this._FilterFileTypes.Length > 0)
            {
                this._FileTypes = this._FilterFileTypes.Split(',');
                
                StringBuilder strb = new StringBuilder();
                for (int i = 0; i < this._FileTypes.Length; i++)
                {
                    if (i == 0)
                    {
                        // 2010-12-15 del
                        //this._FileName = "*." + this._FileTypes[i].ToString().ToLower();
                        this._FileName = string.Empty;
                    }
                    else
                    {
                        strb.Append("|");
                    }

                    this._FileTypes[i] = this._FileTypes[i].ToUpper();

                    if (this._FileTypes[i].Equals("JPEG"))
                    {
                        strb.Append(
                            this._FileTypes[i].ToString() + " (*.jpg)|*.jpg");
                    }
                    else
                    {
                        strb.Append(
                            this._FileTypes[i].ToString() +
                                " (*." + this._FileTypes[i].ToLower() + ")" +
                                "|*." + this._FileTypes[i].ToLower());
                    }
                }
                if (strb.Length > 0)
                {
                    this._Filter = strb.ToString();
                }
            }
            // add end

            return ShowDialog(null);
        }
        protected virtual void OnPrepareMSDialog()
        {
            InitMSDialog();
        }
        private void InitMSDialog()
        {
            System.Reflection.PropertyInfo AutoUpgradeInfo = 
                MSDialog.GetType().GetProperty("AutoUpgradeEnabled"); 
            
            if (AutoUpgradeInfo != null) 
                AutoUpgradeInfo.SetValue(MSDialog, false, null);
            
            MSDialog.InitialDirectory = 
                _InitialDirectory.Length == 0 ? 
                Path.GetDirectoryName(Application.ExecutablePath) : _InitialDirectory;
            
            MSDialog.AddExtension = _AddExtension;
            MSDialog.Filter = _Filter;
            MSDialog.FilterIndex = _FilterIndex;
            MSDialog.CheckFileExists = _CheckFileExists;
            MSDialog.CheckPathExists = _CheckPathExists; // add

            MSDialog.DefaultExt = _DefaultExt;
            MSDialog.FileName = _FileName;
            MSDialog.DereferenceLinks = _DereferenceLinks;
            MSDialog.ShowHelp = _ShowHelp;
            _hasRunInitMSDialog = true;
        }


        public DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult returnDialogResult = DialogResult.Cancel;
            if (this.IsDisposed)
                return returnDialogResult;
            if (owner == null || owner.Handle == IntPtr.Zero)
            {
                WindowWrapper wr = 
                    new WindowWrapper(
                        System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
                owner = wr;
            }
            if (FileDlgType == FileDialogType.OpenFileDlg)
            {
                _dlgWrapper = new DialogWrapper<OpenFileDialog>(this);
            }
            else
            {
                _dlgWrapper = new DialogWrapper<SaveFileDialog>(this);
            }
            OnPrepareMSDialog();
            if (!_hasRunInitMSDialog)
                InitMSDialog();
            try
            {
                returnDialogResult = _MSdialog.ShowDialog(owner);
            }
            // Sometimes if you open a animated .gif on the preview and the Form is closed, 
            //.Net class throw an exception
            // Lets ignore this exception and keep closing the form.
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("unable to get the modal dialog handle", ex.Message);
            }
            return returnDialogResult;
        }
        #endregion


        #region event handlers
        void FileDialogControlBase_DialogDisposed(object sender, EventArgs e)
        {
            Dispose(true);
        }

        private void FileDialogControlBase_ClosingDialog(object sender, CancelEventArgs e)
        {
            if (EventClosingDialog != null)
            {
                EventClosingDialog(this, e);
            }
        }


        void FileDialogControlBase_HelpRequest(object sender, EventArgs e)
        {
            //this is a virtual call that should call the event in the subclass
            OnHelpRequested(new HelpEventArgs(new Point()));
        }

        #endregion
        #region helper types

        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public IntPtr Handle
            {
                get { return _hwnd; }
            }

            private IntPtr _hwnd;
        }
        #endregion

        private void FileDialogControlBase_Load(object sender, EventArgs e)
        {
        }
    }
    #endregion


}