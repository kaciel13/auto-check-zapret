using AutoCheckZapret.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoCheckZapret.ViewModels
{
    /// <summary>
    /// ViewModel для версии Zapret с поддержкой INotifyPropertyChanged
    /// </summary>
    public class ZapretVersionViewModel : INotifyPropertyChanged
    {
        private readonly ZapretVersion _model;
        private bool _isDownloaded;
        private string _bypassMethodName;

        public ZapretVersionViewModel(ZapretVersion model)
        {
            _model = model;
            _isDownloaded = model.IsDownloaded;
            _bypassMethodName = model.BypassMethodName;
        }

        public string Number => _model.Number;
        public string DownloadUrl => _model.DownloadUrl;

        public bool IsDownloaded
        {
            get => _isDownloaded;
            set
            {
                if (_isDownloaded != value)
                {
                    _isDownloaded = value;
                    _model.IsDownloaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BypassMethodName
        {
            get => _bypassMethodName;
            set
            {
                if (_bypassMethodName != value)
                {
                    _bypassMethodName = value;
                    _model.BypassMethodName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ZapretVersion GetModel() => _model;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}