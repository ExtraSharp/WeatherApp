using System;

namespace WPFUI.ViewModels;

// TODO
// slider to select month
// only one window instance
// add prec/sunshine to chart

public class ShellViewModel : Conductor<object>
{
    #region Private Members
    private int _previousMonth;
    private SeriesCollection? _seriesCollection;
    private readonly IWindowManager _windowManager = new WindowManager();
    private DateTime _today;
    private int _month;
    private int _day;
    private double _meanTemp;
    private double _maxTemp;
    private double _minTemp;
    private double _precipitation;
    private double _sunshineHours;
    private double _recordHigh;
    private double _recordLow;
    private int _recordHighYear;
    private int _recordLowYear;
    private bool _isChartVisible;
    private bool _isHistoricalDataVisible;
    private bool _isNoDataVisible;
    private double _currentTemperature;
    private BitmapImage? _weatherImage;
    private string? _stationName;
    private readonly string _connectionStringName = "sqlite";
    private readonly DataRepository _dataRepository;
    private readonly DataFetchingService _dataFetchingService;
    private double? _humidity;
    private double? _dewPoint;
    private double? _cloudCover;
    private string _weatherIcon;
    #endregion

    #region Public Properties
    public string WeatherIcon
    {
        get => _weatherIcon;
        set
        {
            _weatherIcon = value;
            NotifyOfPropertyChange(() => WeatherIcon);
        }
    }
    public double? CloudCover
    {
        get => _cloudCover;
        set
        {
            _cloudCover = value;
            NotifyOfPropertyChange(() => CloudCover);
        }
    }

    public double? DewPoint
    {
        get => _dewPoint;
        set
        {
            _dewPoint = value;
            NotifyOfPropertyChange(() => DewPoint);
        }
    }

    public double? Humidity
    {
        get => _humidity;
        set
        {
            _humidity = value;
            NotifyOfPropertyChange(() => Humidity);
        }
    }
    public string? StationName
    {
        get => _stationName;
        set 
        { 
            _stationName = value;
            NotifyOfPropertyChange(() => StationName);
        }
    }

    public BitmapImage? WeatherImage
    {
        get => _weatherImage;
        set
        {
            _weatherImage = value;
            NotifyOfPropertyChange(() => WeatherImage);
        }
    }

    public SeriesCollection? SeriesCollection
    {
        get => _seriesCollection;
        set 
        {
            _seriesCollection = value; 
            NotifyOfPropertyChange(() => SeriesCollection);
        }
    }

    public class ObservableValue
    {
        public int Date { get; set; }
        private double _value;

        public double Value
        {
            get => double.Parse(_value.ToString("0.00"));

            set => _value = value;
        }        
    }

    public DateTime Today
    {
        get => _today;
        private set
        {
            _today = value;
            NotifyOfPropertyChange(() => Today);
            Month = value.Month;
            Day = value.Day;
            GetDailyMeans();
        }
    }
    
    public double MeanTemp
    {
        get { return _meanTemp; }
        set
        {
            _meanTemp = value;
            NotifyOfPropertyChange(() => MeanTemp);
        }
    }

    public double CurrentTemperature
    {
        get => _currentTemperature;
        set
        {
            _currentTemperature = value;
            NotifyOfPropertyChange(() => CurrentTemperature);
        }
    }

    public double MaxTemp
    {
        get => _maxTemp;
        set
        {
            _maxTemp = value;
            NotifyOfPropertyChange(() => MaxTemp);
        }
    }

    public int Month
    {
        get => _month;
        set
        {
            _previousMonth = Month;
            _month = value;
            NotifyOfPropertyChange(() => Month);

            if (_previousMonth != Month)
            {
                GetGraphDisplayData();
            }
        }
    }

    public int Day
    {
        get => _day;
        set
        {
            _day = value;
            NotifyOfPropertyChange(() => Day);
        }
    }

    public double MinTemp
    {
        get => _minTemp;
        set
        {
            _minTemp = value;
            NotifyOfPropertyChange(() => MinTemp);
        }
    }

    public double Precipitation
    {
        get => _precipitation;
        set
        {
            _precipitation = value;
            NotifyOfPropertyChange(() => Precipitation);
        }
    }

    public double SunshineHours
    {
        get => _sunshineHours;
        set
        {
            _sunshineHours = value;
            NotifyOfPropertyChange(() => SunshineHours);
        }
    }

    public double RecordHigh
    {
        get => _recordHigh;
        set
        {
            _recordHigh = value;
            NotifyOfPropertyChange(() => RecordHigh);
        }
    }

    public double RecordLow
    {
        get => _recordLow;
        set
        {
            _recordLow = value;
            NotifyOfPropertyChange(() => RecordLow);
        }
    }

    public int RecordHighYear
    {
        get => _recordHighYear;
        set
        {
            _recordHighYear = value;
            NotifyOfPropertyChange(() => RecordHighYear);
        }
    }

    public int RecordLowYear
    {
        get => _recordLowYear;
        set
        {
            _recordLowYear = value;
            NotifyOfPropertyChange(() => RecordLowYear);
        }
    }

    public bool IsChartVisible
    {
        get => _isChartVisible;
        set
        {
            if (_isChartVisible != value)
            {
                _isChartVisible = value;
                NotifyOfPropertyChange(() => IsChartVisible);
            }
        }
    }

    public bool IsHistoricalDataVisible
    {
        get => _isHistoricalDataVisible;
        set
        {
            if (_isHistoricalDataVisible != value)
            {
                _isHistoricalDataVisible = value;
                NotifyOfPropertyChange(() => IsHistoricalDataVisible);
            }
        }
    }

    public bool IsNoDataVisible
    {
        get => _isNoDataVisible;
        set
        {
            if (_isNoDataVisible != value)
            {
                _isNoDataVisible = value;
                NotifyOfPropertyChange(() => IsNoDataVisible);
            }
        }
    }
    #endregion

    #region Constructors
    public ShellViewModel()
    {
        _dataRepository = new DataRepository(GeneralHelpers.GetConnectionString(_connectionStringName));
        _dataFetchingService = new DataFetchingService(_dataRepository);

        SetToday();
        GetGraphDisplayData();
        GetCurrentWeather();
    } 
    #endregion

    #region Methods
    private void LoadWeatherImage()
    {
        var weatherIconUrIs = new Dictionary<string, string>
        {
            { "sunny", "clear_day.png" },
            { "cloudy", "cloudy.png" },
            { "partly-cloudy", "partly-cloudy.png" },
            { "rainy", "rainy.png" },
            { "thunderstorm", "thunderstorm.png" },
            { "clear-night", "clear-night.png" },
            { "cloudy-night", "cloudy-night.png" }
        };

        Uri uri;

        if (weatherIconUrIs.TryGetValue(WeatherIcon, out string? imageName))
        {
            uri = new Uri($"pack://application:,,,/WPFUI;component/Images/{imageName}");
        }
        else
        {
            uri = new Uri($"pack://application:,,,/WPFUI;component/Images/clear-day.png");
        }
        WeatherImage = new BitmapImage(uri);
    }

    void GetGraphDisplayData()
    {
        try
        {
            List<DayModel> daysToDisplay = _dataFetchingService.GetDaysForMonth(Today.Year, Month);
            PrepareGraphData(daysToDisplay);
            IsChartVisible = true;
        }
        catch (Exception)
        {
            IsChartVisible = false;
        }
    }

    void PrepareGraphData(List<DayModel> days)
    {
        try
        {
            var maxTemps = days.Select(day => new ObservableValue { Date = day.Day, Value = day.MaxTemp }).ToList();
            var minTemps = days.Select(day => new ObservableValue { Date = day.Day, Value = day.MinTemp }).ToList();
            var meanPrecipitation = days.Select(day => new ObservableValue { Date = day.Day, Value = day.Precipitation }).ToList();

            maxTemps.Sort((a, b) => a.Date.CompareTo(b.Date));
            minTemps.Sort((a, b) => a.Date.CompareTo(b.Date));
            meanPrecipitation.Sort((a, b) => a.Date.CompareTo(b.Date));

            var mapper = Mappers.Xy<ObservableValue>().X(value => value.Date).Y(value => value.Value);

            SeriesCollection = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    Title = "Max Temperatures in °C",
                    Values = new ChartValues<ObservableValue>(maxTemps),
                },
                new LineSeries
                {
                    Title = "Min Temperatures in °C",
                    Values = new ChartValues<ObservableValue>(minTemps),
                    ToolTip = "{Value:N2} °C"
                },
                new ColumnSeries
                {
                    Title = "Precipitation in mm",
                    Values = new ChartValues<ObservableValue>(meanPrecipitation),
                    ScalesYAt = 1,
                }
            };

            IsChartVisible = true;
        }
        catch (Exception ex)
        {
            IsChartVisible = false;
            MessageBox.Show($"{ex.Message}", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public void RefreshData()
    {
        SetToday();
        GetGraphDisplayData();
        GetCurrentWeather();
    }

    private async void GetCurrentWeather()
    {
        var api = new ApiAccess();
        var currentWeather = await api.GetWeatherData();

        if (currentWeather != null)
        {
            if (currentWeather.weather != null) CurrentTemperature = currentWeather.weather.Temperature;
            if (currentWeather.weather != null) DewPoint = currentWeather.weather.DewPoint;
            StationName = currentWeather.sources[0].StationName;
            Humidity = currentWeather.weather?.Humidity;
            CloudCover = currentWeather.weather?.CloudCover;
            WeatherIcon = currentWeather?.weather?.Icon switch
            {
                "clear-day" => "sunny",
                "cloudy" => "cloudy",
                "rainy" => "rainy",
                "partly-cloudy-day" => "partly-cloudy",
                "thunderstorm" => "thunderstorm",
                "clear-night" => "clear-night",
                "partly-cloudy-night" => "cloudy-night",
                _ => WeatherIcon
            };
        }

        LoadWeatherImage();
    }

    private void GetDailyMeans() 
    {
        try
        {
            var days = (Day == 0) ? _dataRepository.GetMonths(Month) : _dataRepository.GetDays(Month, Day);

            DisplayDailyMeans(days);
        }
        catch (Exception)
        {
            IsHistoricalDataVisible = false;
            IsNoDataVisible = true;
        }       
    }

    public void SetToday()
    {
        Today = DateTime.Now;
    }

    private void DisplayDailyMeans(IReadOnlyCollection<DayModel> days)
    {
        IsHistoricalDataVisible = true;
        IsNoDataVisible = false;

        MeanTemp = days.Average(x => x.MeanTemp);
        MaxTemp = days.Average(x => x.MaxTemp);
        MinTemp = days.Average(x => x.MinTemp);
        Precipitation = days.Average(x => x.Precipitation);
        SunshineHours = days.Average(x => x.SunshineHours);

        var recordHighData = days.MaxBy(x => x.MaxTemp);
        if (recordHighData != null)
        {
            RecordHigh = recordHighData.MaxTemp;
            RecordHighYear = recordHighData.Year;
        }

        var recordLowData = days.MinBy(x => x.MinTemp);
        if (recordLowData == null) return;
        RecordLow = recordLowData.MinTemp;
        RecordLowYear = recordLowData.Year;
    }

    public void NextDay()
    {
        Today = Today.AddDays(1);
    }

    public void PreviousDay()
    {
       Today = Today.AddDays(-1);
    }

    public void OpenDataView()
    {
        var changeDataViewModel = new ChangeDataViewModel(_dataRepository);
        _windowManager.ShowWindowAsync(changeDataViewModel);
    }

    public void OpenClimateChartView()
    {
        if (_isNoDataVisible)
        {
            MessageBox.Show("Please add some data first", "No Data to display", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        var climateChartViewModel = new ClimateChartViewModel(_dataRepository);
        _windowManager.ShowWindowAsync(climateChartViewModel);
    }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        Application.Current.Shutdown();

        return Task.FromResult(true);
    }
    #endregion
}
