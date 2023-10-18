using System;
using System.Collections.Generic;
using VaultsII.Core;
using VaultsII.Views.HomePanelViews;

namespace VaultsII.Views {
    public class ViewControl : ObservableObject {
        public static ViewControl Instance {
            get { return instance ?? new ViewControl(); }
            set { instance = value; }
        }
        private static ViewControl instance;

        public StartView StartView { get; set; }
        public AlbumView AlbumView { get; set; }

        public object CurrentView { 
            get { return currentView; } 
            set {
                priorView = currentView;
                currentView = value;

                OnPropertyChanged();

                OnViewChanged?.Invoke(this, EventArgs.Empty);
            } 
        }
        private object currentView;

        private object priorView;

        public static readonly int StartIndex = 0;
        public static readonly int AlbumIndex = 1;

        public event EventHandler OnViewChanged;

        public void RevertView() => CurrentView = priorView ?? StartView;

        public void SetAlbumView() => CurrentView = AlbumView;

        public void SetStartView() => CurrentView = StartView;

        public void UpdateUI() => OnPropertyChanged();

        public ViewControl() { 
            StartView = new();
            AlbumView = new();

            CurrentView = StartView;

            Instance = this;
        }
    }
}
