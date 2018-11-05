using Data;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace HW9Part2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel() : this(new DefaultDataService(), new DefaultTrackingService())
        { }

        public MainViewModel(IDataService data, ITrackingService trackingSvc)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.trackingSvc = trackingSvc ?? throw new ArgumentNullException(nameof(trackingSvc));
            People = new ObservableCollection<Person>();
        }

        private IEnumerable<TrackingResult> trackingResults;
        public IEnumerable<TrackingResult> TrackingResults
        {
            get { return trackingResults; }
            set { trackingResults = value; OnPropertyChanged(nameof(TrackingResults)); }
        }

        private string trackingNumber;
        public string TrackingNumber
        {
            get { return trackingNumber; }
            set
            {
                trackingNumber = value;
                TrackingResults = trackingSvc.Track(value);
                OnPropertyChanged(nameof(TrackingNumber));
            }
        }

        private string weatherResponse;
        public string WeatherResponse
        {
            get { return weatherResponse; }
            set { weatherResponse = value; OnPropertyChanged(nameof(WeatherResponse)); }
        }

        private string lat;
        public string Lat
        {
            get { return lat; }
            set
            {
                lat = value;
                OnPropertyChanged(nameof(Lat));
            }
        }
        private string lng;
        public string Lng
        {
            get { return lng; }
            set
            {
                lng = value;
                OnPropertyChanged(nameof(lng));
            }
        }
        private SimpleCommand getWeather;
        public SimpleCommand GetWeather => getWeather ?? (getWeather = new SimpleCommand(async () =>
        {
            try
            {
                WeatherResponse = await data.GetConditionForLocationAsync(lat, lng);
            }
            catch (Exception ex)
            {
                WeatherResponse = $"Whoops!  Error: {ex.Message}";
            }
        }));

        private string gedcomPath;
        public string GedcomPath
        {
            get => gedcomPath;
            set
            {
                gedcomPath = value;
                OnPropertyChanged(nameof(GedcomPath));
                LoadGedcom.RaiseCanExecuteChanged();
            }
        }

        private string output;
        public string Output
        {
            get => output;
            set
            {
                output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        private readonly IDataService data;
        private readonly ITrackingService trackingSvc;
        private SimpleCommand loadGedcom;
        public SimpleCommand LoadGedcom => loadGedcom ?? (loadGedcom = new SimpleCommand(
        () => !IsBusy && data.FileExists(GedcomPath), //can execute
        async () => //execute
        {
            Output = "Loading...";
            IsBusy = true;
            foreach (var p in await data.GetPeopleFromGedcomAsync(GedcomPath))
                People.Add(p);
            Output = $"We found {People.Count} people in {GedcomPath}!";
            IsBusy = false;
        }));

        private SimpleCommand findFile;
        public SimpleCommand FindFile => findFile ?? (findFile = new SimpleCommand(
            () => !IsBusy,
            async () =>
            {
                GedcomPath = await data.FindFileAsync();
                LoadGedcom.RaiseCanExecuteChanged();
            }));

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                LoadGedcom.RaiseCanExecuteChanged();
                FindFile.RaiseCanExecuteChanged();
            }
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Person> People { get; private set; }

        private Person selectedPerson;
        public Person SelectedPerson
        {
            get => selectedPerson;
            set
            {
                selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

    }
}