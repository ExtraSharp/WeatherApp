namespace WPFUI.ViewModels;

public class ChangeDataViewModel : Screen
{
    #region Private Members
    private BindableCollection<MonthModel> _availableMonths = new();
    private MonthModel _selectedMonth;
    private readonly IDataRepository _dataRepository;
    #endregion

    #region Public Properties
    public MonthModel SelectedMonth
    {
        get 
        { 
            return _selectedMonth; 
        }
        set 
        { 
            _selectedMonth = value;
            NotifyOfPropertyChange(() => SelectedMonth);
        }
    }   

	public BindableCollection<MonthModel> AvailableMonths
    {
		get { return _availableMonths; }
		set 
        {
            _availableMonths = value;
            NotifyOfPropertyChange(() => AvailableMonths);
               
        }
	}
    #endregion

    #region Constructors
    public ChangeDataViewModel(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
        DisplayAvailableData();
    }
    #endregion

    #region Methods
    private void DisplayAvailableData()
    {
        var months = _dataRepository.GetAvailableMonths();
        AvailableMonths = new BindableCollection<MonthModel>(months);

        SortAvailableMonths();
    }

    public void DeleteSelectedMonth()
    {
        if (SelectedMonth != null)
        {
            PerformDelete(SelectedMonth);
        }
        else
        {
            MessageBox.Show("Please select a month to delete first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
    }

    public void DeleteAllMonths()
    {
        if (AvailableMonths.Any())
        {
            PerformDelete(null);
        }
        else
        {
            MessageBox.Show("No data to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }        
    }

    private void PerformDelete(MonthModel monthToDelete)
    {
        string confirmationMessage = monthToDelete != null ? "Are you sure you want to delete the selected month?" : "Are you sure you want to delete all data?";
        MessageBoxResult result = MessageBox.Show(confirmationMessage, "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                if (monthToDelete != null)
                {
                    _dataRepository.DeleteMonth(monthToDelete);
                    AvailableMonths.Remove(monthToDelete);
                    MessageBox.Show("Month Deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    foreach (var item in AvailableMonths.ToList())
                    {
                        _dataRepository.DeleteMonth(item);
                        AvailableMonths.Remove(item);
                    }
                    MessageBox.Show("All data has been deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }

    public void SortAvailableMonths()
    {
        if (AvailableMonths != null && AvailableMonths.Any())
        {
            AvailableMonths = new BindableCollection<MonthModel>(AvailableMonths.OrderByDescending(m => m.Year).ThenByDescending(m => m.Month));
        }
    }

    public void AddFromFile()
    {
        string filePath = "";

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select a File",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            filePath = openFileDialog.FileName;
            SaveMonths(new List<MonthModel> { GeneralHelpers.ReadDataFromCsv(filePath) });
        }
    }

    public void AddFromFolder()
    {
        string folderPath = "";
        List<MonthModel> monthsToSave = new List<MonthModel>();

        OpenFolderDialog openFolderDialog = new OpenFolderDialog()
        {
            Tag = "Select a Folder containing .csv files"
        };

        if (openFolderDialog.ShowDialog() == true)
        {
            folderPath = openFolderDialog.FolderName;
            string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

            foreach (string csvFile in csvFiles)
            {
                monthsToSave.Add(GeneralHelpers.ReadDataFromCsv(csvFile));
            }
        }

        SaveMonths(monthsToSave);
    }

    public void SaveMonths(IEnumerable<MonthModel> months)
    {
        int savedCount = 0;
        int overwrittenCount = 0;
        bool overwriteAll = false;

        foreach (var month in months)
        {
            if (_dataRepository.MonthExists(month.Month, month.Year))
            {
                if (!overwriteAll)
                {
                    MessageBoxResult result = MessageBox.Show("Data for at least month already exists. Do you want to overwrite the existing data?", $"Confirm Overwrite", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        overwriteAll = true;
                        _dataRepository.UpdateMonth(month);
                        overwrittenCount++;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    _dataRepository.UpdateMonth(month);
                    overwrittenCount++;
                }
            }
            else
            {
                _dataRepository.CreateMonth(month);
                savedCount++;
                AvailableMonths.Add(month);
            }
        }

        string message = $"{savedCount} month(s) have been saved.";
        if (overwrittenCount > 0)
        {
            message += $"\n{overwrittenCount} month(s) have been overwritten.";
        }

        MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    #endregion
}
