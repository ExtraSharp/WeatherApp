namespace WPFUI.ViewModels;
public class ClimateChartViewModel : Screen
{
    #region Private Members
    private BindableCollection<MonthModel> _availableMonths = new();
    private readonly IDataRepository _dataRepository;
    private readonly DataFetchingService _dataFetchingService;
    private readonly ClimateChartCalculationService _calculationService;
    private SeriesCollection _heatMapSeries;
    private BindableCollection<int> _availableYears;
    private MonthModel _dataFromSelectedItem;
    private MonthModel _dataToSelectedItem;
    private int _dataFromSelectedYear;
    private int _dataToSelectedYear;
    #endregion

    #region Public Properties
    public BindableCollection<MonthModel> AvailableMonths
    {
        get { return _availableMonths; }
        set
        {
            _availableMonths = value;
            NotifyOfPropertyChange(() => AvailableMonths);

        }
    }

    public SeriesCollection HeatMapSeries
    {
        get
        {
            return _heatMapSeries;
        }
        set
        {
            _heatMapSeries = value;
            NotifyOfPropertyChange(() => HeatMapSeries);
        }
    }

    public List<string> YAxisLabels { get; set; }

    public Func<double, string> XAxisLabelFormatter { get; } = value =>
    {
        var months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Year" };
        if (value >= 0 && value < 13)
            return months[(int)value];
        return "";
    };

    public Func<double, string> YAxisLabelFormatter { get; } = value => $"{value} °C";
    public BindableCollection<int> AvailableYears
    {
        get { return _availableYears; }
        set
        {
            _availableYears = value;
            NotifyOfPropertyChange(() => AvailableYears);
        }
    }

    public MonthModel DataFromSelectedItem
    {
        get { return _dataFromSelectedItem; }
        set
        {
            _dataFromSelectedItem = value;
            NotifyOfPropertyChange(() => DataFromSelectedItem);
        }
    }

    public int DataFromSelectedYear
    {
        get { return _dataFromSelectedYear; }
        set
        {
            _dataFromSelectedYear = value;
            NotifyOfPropertyChange(() => DataFromSelectedYear);
        }
    }

    public int DataToSelectedYear
    {
        get { return _dataToSelectedYear; }
        set
        {
            _dataToSelectedYear = value;
            NotifyOfPropertyChange(() => DataToSelectedYear);
        }
    }

    public MonthModel DataToSelectedItem
    {
        get { return _dataToSelectedItem; }
        set
        {
            _dataToSelectedItem = value;
            NotifyOfPropertyChange(() => DataToSelectedItem);
        }
    }
    #endregion

    #region Constructors
    public ClimateChartViewModel(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
        _calculationService = new ClimateChartCalculationService();
        _dataFetchingService = new DataFetchingService(_dataRepository);
        HeatMapSeries = new SeriesCollection();

        WireUpRangeSelectors();
        PopulateData();
    }
    #endregion

    #region Methods

    private void WireUpRangeSelectors()
    {
        var months = _dataRepository.GetAvailableMonths();
        var distinctYears = months.Select(m => m.Year).Distinct().ToList();
        AvailableYears = new BindableCollection<int>(distinctYears);

        if (AvailableYears.Any())
        {
            DataFromSelectedYear = AvailableYears.First();
            DataToSelectedYear = AvailableYears.Last();
        }
    }

    public void UseFilter()
    {
        if (IsValidDateRange())
        {
            PopulateData();
        }
        else
        {
            MessageBox.Show("Invalid date range", "Error", MessageBoxButton.OK, MessageBoxImage.Error);            
        }
    }

    public bool IsValidDateRange()
    {
        return DataFromSelectedYear <= DataToSelectedYear;
    }

    private void PopulateData()
    {
        HeatMapSeries.Clear();
        List<List<DayModel>> allMonths = _dataFetchingService.GetAllMonths();
        List<List<DayModel>> filteredMonths = new List<List<DayModel>>();

        foreach (var month in allMonths)
        {
            List<DayModel> filteredDays = month.Where(day => day.Year >= DataFromSelectedYear && day.Year <= DataToSelectedYear).ToList();

            // Skip empty months if you don't want to include them
            if (filteredDays.Count > 0)
            {
                filteredMonths.Add(filteredDays);
            }
        }

        List<double> overallStatistics = _calculationService.CalculateOverallStatistics(filteredMonths);
        List<double> monthlyStatistics = _calculationService.CalculateMeanMonthlyMaxMins(filteredMonths);

        var recordHighs = new ChartValues<HeatPoint>();
        var recordLows = new ChartValues<HeatPoint>();
        var dailyMaxValues = new ChartValues<HeatPoint>();
        var dailyMeanValues = new ChartValues<HeatPoint>();
        var dailyMinValues = new ChartValues<HeatPoint>();
        var meanMaxValues = new ChartValues<HeatPoint>();
        var meanMinValues = new ChartValues<HeatPoint>();
        //var precipitationValues = new ChartValues<HeatPoint>();

        PopulateHeatPointValues(overallStatistics, monthlyStatistics, recordHighs, recordLows, dailyMaxValues,
                                dailyMeanValues, dailyMinValues, meanMaxValues, meanMinValues);

        AddHeatSeries("Record Highs", recordHighs);
        AddHeatSeries("Mean Maximum", meanMaxValues);
        AddHeatSeries("Mean Daily Max", dailyMaxValues);
        AddHeatSeries("Daily Mean", dailyMeanValues);
        AddHeatSeries("Mean Daily Min", dailyMinValues);
        AddHeatSeries("Mean Minimum", meanMinValues);
        AddHeatSeries("Record Lows", recordLows);
        //AddHeatSeries("Precipitation", precipitationValues); 
        //AddHeatSeries("Sunshine Hours", new ChartValues<HeatPoint>()); // Add other series

        UpdateYAxisLabels();
    }

    private void PopulateHeatPointValues(List<double> overallStatistics, List<double> monthlyStatistics,
                                     ChartValues<HeatPoint> recordHighs, ChartValues<HeatPoint> recordLows,
                                     ChartValues<HeatPoint> dailyMaxValues, ChartValues<HeatPoint> dailyMeanValues,
                                     ChartValues<HeatPoint> dailyMinValues, ChartValues<HeatPoint> meanMaxValues,
                                     ChartValues<HeatPoint> meanMinValues)
    {

        for (int i = 0; i < 13; i++)
        {
            try
            {
                recordHighs.Add(new HeatPoint(i, 6, double.Parse(overallStatistics[i].ToString("0.0"))));
                meanMaxValues.Add(new HeatPoint(i, 5, double.Parse(monthlyStatistics[i].ToString("0.0"))));
                dailyMaxValues.Add(new HeatPoint(i, 4, double.Parse(overallStatistics[i + 26].ToString("0.0"))));
                dailyMeanValues.Add(new HeatPoint(i, 3, double.Parse(overallStatistics[i + 39].ToString("0.0"))));
                dailyMinValues.Add(new HeatPoint(i, 2, double.Parse(overallStatistics[i + 52].ToString("0.0"))));
                meanMinValues.Add(new HeatPoint(i, 1, double.Parse(monthlyStatistics[i + 13].ToString("0.0"))));
                recordLows.Add(new HeatPoint(i, 0, double.Parse(overallStatistics[i + 13].ToString("0.0"))));
                //precipitationValues.Add(new HeatPoint(i, 1, double.Parse(overallStatistics[i + 65].ToString("0.0"))));
            }
            catch (Exception)
            {
            }
        }
    }

    private void AddHeatSeries(string title, ChartValues<HeatPoint> values)
    {
        HeatMapSeries.Add(new HeatSeries
        {
            Title = title,
            Values = values,
            DataLabels = true,
            GradientStopCollection = CreateGradient(title)
        });
    }

    private void UpdateYAxisLabels()
    {
        YAxisLabels = HeatMapSeries.Select(s => s.Title).Reverse().ToList();
        NotifyOfPropertyChange(() => YAxisLabels);
    }

    private GradientStopCollection CreateGradient(string title)
    {
        if (title == "Precipitation")
        {
            return CreatePrecipitationGradient();
        }
        
        return new GradientStopCollection
        {
            new GradientStop { Offset = 0, Color = Colors.Blue },
            new GradientStop { Offset = 0.4, Color = Colors.White },
            new GradientStop { Offset = 0.7, Color = Colors.Orange },
            new GradientStop { Offset = 1, Color = Colors.Red }
        };
    }

    private GradientStopCollection CreatePrecipitationGradient()
    {
        return new GradientStopCollection
        {
            new GradientStop { Offset = 0, Color = Colors.LightGreen },
            new GradientStop { Offset = 0.7, Color = Colors.Green },
            new GradientStop { Offset = 1, Color = Colors.DarkGreen }
        };
    }
    #endregion
}
