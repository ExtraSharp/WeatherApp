namespace WPFUI.ViewModels;

// TODO
// slider to select month
// only one window instance
// add prec/sunshine to chart

public class ShellViewModel : Conductor<object>
{
    #region Private Members
    private ClimateChartViewModel _climateChartViewModelInstance;
    private ChangeDataViewModel _changeDataViewModelInstance;
    private int _previousMonth;
    private SeriesCollection _seriesCollection;
    private IWindowManager _windowManager = new WindowManager();
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
    private BitmapImage _weatherImage;
    private string _stationName;
    private readonly string _connectionStringName = "sqlite";
    private DataRepository _dataRepository;
    private DataFetchingService _dataFetchingService;
    #endregion

    #region Public Properties
    public string StationName
    {
        get { return _stationName; }
        set 
        { 
            _stationName = value;
            NotifyOfPropertyChange(() => StationName);
        }
    }

    public BitmapImage WeatherImage
    {
        get { return _weatherImage; }
        set
        {
            _weatherImage = value;
            NotifyOfPropertyChange(() => WeatherImage);
        }
    }

    public SeriesCollection SeriesCollection
    {
        get 
        { 
            return _seriesCollection; 
        }
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
        get { return _today; }
        set
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
        get { return _currentTemperature; }
        set
        {
            _currentTemperature = value;
            NotifyOfPropertyChange(() => CurrentTemperature);
        }
    }

    public double MaxTemp
    {
        get { return _maxTemp; }
        set
        {
            _maxTemp = value;
            NotifyOfPropertyChange(() => MaxTemp);
        }
    }

    public int Month
    {
        get { return _month; }
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
        get { return _day; }
        set
        {
            _day = value;
            NotifyOfPropertyChange(() => Day);
        }
    }

    public double MinTemp
    {
        get { return _minTemp; }
        set
        {
            _minTemp = value;
            NotifyOfPropertyChange(() => MinTemp);
        }
    }

    public double Precipitation
    {
        get { return _precipitation; }
        set
        {
            _precipitation = value;
            NotifyOfPropertyChange(() => Precipitation);
        }
    }

    public double SunshineHours
    {
        get { return _sunshineHours; }
        set
        {
            _sunshineHours = value;
            NotifyOfPropertyChange(() => SunshineHours);
        }
    }

    public double RecordHigh
    {
        get { return _recordHigh; }
        set
        {
            _recordHigh = value;
            NotifyOfPropertyChange(() => RecordHigh);
        }
    }

    public double RecordLow
    {
        get { return _recordLow; }
        set
        {
            _recordLow = value;
            NotifyOfPropertyChange(() => RecordLow);
        }
    }

    public int RecordHighYear
    {
        get { return _recordHighYear; }
        set
        {
            _recordHighYear = value;
            NotifyOfPropertyChange(() => RecordHighYear);
        }
    }

    public int RecordLowYear
    {
        get { return _recordLowYear; }
        set
        {
            _recordLowYear = value;
            NotifyOfPropertyChange(() => RecordLowYear);
        }
    }

    public bool IsChartVisible
    {
        get { return _isChartVisible; }
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
        get { return _isHistoricalDataVisible; }
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
        get { return _isNoDataVisible; }
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
        try
        {
            var uri = new Uri("pack://application:,,,/WPFUI;component/Images/clear_day.png");
            WeatherImage = new BitmapImage(uri);
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            // Log or display error message
        }
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

    public void Refresh()
    {
        SetToday();
        GetGraphDisplayData();
        GetCurrentWeather();
    }

    public async void GetCurrentWeather()
    {
        ApiAccess api = new ApiAccess();
        var currentWeather = await api.GetWeatherData();

        CurrentTemperature = currentWeather.weather.Temperature;
        StationName = currentWeather.sources[0].StationName;

        LoadWeatherImage();
    }

    void GetDailyMeans() 
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

    void DisplayDailyMeans(List<DayModel> days)
    {
        IsHistoricalDataVisible = true;
        IsNoDataVisible = false;

        MeanTemp = days.Average(x => x.MeanTemp);
        MaxTemp = days.Average(x => x.MaxTemp);
        MinTemp = days.Average(x => x.MinTemp);
        Precipitation = days.Average(x => x.Precipitation);
        SunshineHours = days.Average(x => x.SunshineHours);

        var recordHighData = days.OrderByDescending(x => x.MaxTemp).FirstOrDefault();
        if (recordHighData != null)
        {
            RecordHigh = recordHighData.MaxTemp;
            RecordHighYear = recordHighData.Year;
        }

        var recordLowData = days.OrderBy(x => x.MinTemp).FirstOrDefault();
        if (recordLowData != null)
        {
            RecordLow = recordLowData.MinTemp;
            RecordLowYear = recordLowData.Year;
        }
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
        var climateChartViewModel = new ClimateChartViewModel(_dataRepository);
        _windowManager.ShowWindowAsync(climateChartViewModel);
    }

    public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        Application.Current.Shutdown();

        return true;
    }
    #endregion
}
