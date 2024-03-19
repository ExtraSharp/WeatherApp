namespace WPFUI.ViewModels;
public class ClimateChartViewModel : Screen
{
    #region Private Members
    private readonly IDataRepository _dataRepository;
    private readonly DataFetchingService _dataFetchingService;
    private SeriesCollection? _heatMapSeries;
    private BindableCollection<int>? _availableYears;
    private MonthModel? _dataFromSelectedItem;
    private MonthModel? _dataToSelectedItem;
    private int _dataFromSelectedYear;
    private int _dataToSelectedYear;
    #endregion

    #region Public Properties
    public SeriesCollection? HeatMapSeries
    {
        get => _heatMapSeries;
        set
        {
            _heatMapSeries = value;
            NotifyOfPropertyChange(() => HeatMapSeries);
        }
    }

    public List<string>? YAxisLabels { get; set; }

    public Func<double, string> XAxisLabelFormatter { get; } = value =>
    {
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Year" };
        if (value is >= 0 and < 13)
            return months[(int)value];
        return "";
    };

    public Func<double, string> YAxisLabelFormatter { get; } = value => $"{value} °C";

    public BindableCollection<int>? AvailableYears
    {
        get => _availableYears;
        set
        {
            _availableYears = value;
            NotifyOfPropertyChange(() => AvailableYears);
        }
    }

    public MonthModel? DataFromSelectedItem
    {
        get => _dataFromSelectedItem;
        set
        {
            _dataFromSelectedItem = value;
            NotifyOfPropertyChange(() => DataFromSelectedItem);
        }
    }

    public int DataFromSelectedYear
    {
        get => _dataFromSelectedYear;
        set
        {
            _dataFromSelectedYear = value;
            NotifyOfPropertyChange(() => DataFromSelectedYear);
        }
    }

    public int DataToSelectedYear
    {
        get => _dataToSelectedYear;
        set
        {
            _dataToSelectedYear = value;
            NotifyOfPropertyChange(() => DataToSelectedYear);
        }
    }

    public MonthModel? DataToSelectedItem
    {
        get => _dataToSelectedItem;
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

        if (!AvailableYears.Any()) return;
        DataFromSelectedYear = AvailableYears.First();
        DataToSelectedYear = AvailableYears.Last();
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

    private bool IsValidDateRange()
    {
        return DataFromSelectedYear <= DataToSelectedYear;
    }

    private void PopulateData()
    {
        HeatMapSeries?.Clear();
        var allMonths = _dataFetchingService.GetAllMonths();
        var filteredMonths = allMonths.Select(month => month.Where(day => day.Year >= DataFromSelectedYear && day.Year <= DataToSelectedYear).ToList()).Where(filteredDays => filteredDays.Count > 0).ToList();

        var chartData = ClimateChartCalculationService.CalculateChartData(filteredMonths);

        CreateHeatSeries(chartData);

        UpdateYAxisLabels();
    }

    private void CreateHeatSeries(List<ClimateChartModel> chartData)
    {
        var recordHighs = new ChartValues<HeatPoint>();
        var recordLows = new ChartValues<HeatPoint>();
        var dailyMaxValues = new ChartValues<HeatPoint>();
        var dailyMeanValues = new ChartValues<HeatPoint>();
        var dailyMinValues = new ChartValues<HeatPoint>();
        var meanMaxValues = new ChartValues<HeatPoint>();
        var meanMinValues = new ChartValues<HeatPoint>();
        //var precipitationValues = new ChartValues<HeatPoint>();

        PopulateHeatPointValues(recordHighs, recordLows, dailyMaxValues, dailyMeanValues, dailyMinValues, meanMaxValues, meanMinValues, chartData);

        Dictionary<string, ChartValues<HeatPoint>> seriesData = new Dictionary<string, ChartValues<HeatPoint>>()
        {
            { "Record High", recordHighs },
            { "Mean Maximum", meanMaxValues },
            { "Mean Daily Max", dailyMaxValues },
            { "Daily Mean", dailyMeanValues },
            { "Mean Daily Min", dailyMinValues },
            { "Mean Minimum", meanMinValues },
            { "Record Low", recordLows }
        };

        foreach (var data in seriesData)
        {
            AddHeatSeries(data.Key, data.Value);
        }
    }

    private static void PopulateHeatPointValues(ChartValues<HeatPoint> recordHighs, ChartValues<HeatPoint> recordLows,
                                     ChartValues<HeatPoint> dailyMaxValues, ChartValues<HeatPoint> dailyMeanValues,
                                     ChartValues<HeatPoint> dailyMinValues, ChartValues<HeatPoint> meanMaxValues,
                                     ChartValues<HeatPoint> meanMinValues, List<ClimateChartModel> chartData)
    {

        for (var i = 0; i < chartData.Count; i++)
        {
            try
            {
                recordHighs.Add(new HeatPoint(i, 6, double.Parse(chartData[i].RecordHigh.ToString("0.0"))));
                meanMaxValues.Add(new HeatPoint(i, 5, double.Parse(chartData[i].MeanMax.ToString("0.0"))));
                dailyMaxValues.Add(new HeatPoint(i, 4, double.Parse(chartData[i].MeanDailyMax.ToString("0.0"))));
                dailyMeanValues.Add(new HeatPoint(i, 3, double.Parse(chartData[i].DailyMean.ToString("0.0"))));
                dailyMinValues.Add(new HeatPoint(i, 2, double.Parse(chartData[i].MeanDailyMin.ToString("0.0"))));
                meanMinValues.Add(new HeatPoint(i, 1, double.Parse(chartData[i].MeanMin.ToString("0.0"))));
                recordLows.Add(new HeatPoint(i, 0, double.Parse(chartData[i].RecordLow.ToString("0.0"))));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private void AddHeatSeries(string title, ChartValues<HeatPoint> values)
    {
        HeatMapSeries?.Add(new HeatSeries
        {
            Title = title,
            Values = values,
            DataLabels = true,
            GradientStopCollection = CreateGradient(title)
        });
    }

    private void UpdateYAxisLabels()
    {
        if (HeatMapSeries != null) YAxisLabels = HeatMapSeries.Select(s => s.Title).Reverse().ToList();
        NotifyOfPropertyChange(() => YAxisLabels);
    }

    private GradientStopCollection CreateGradient(string title)
    {
        if (title == "Precipitation")
        {
            return CreatePrecipitationGradient();
        }
        
        return
        [
            new GradientStop { Offset = 0, Color = Colors.Blue },
            new GradientStop { Offset = 0.4, Color = Colors.White },
            new GradientStop { Offset = 0.7, Color = Colors.Orange },
            new GradientStop { Offset = 1, Color = Colors.Red }
        ];
    }

    private GradientStopCollection CreatePrecipitationGradient()
    {
        return
        [
            new GradientStop { Offset = 0, Color = Colors.LightGreen },
            new GradientStop { Offset = 0.7, Color = Colors.Green },
            new GradientStop { Offset = 1, Color = Colors.DarkGreen }
        ];
    }
    #endregion
}
