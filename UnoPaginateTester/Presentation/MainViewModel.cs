using System.Collections.ObjectModel;

namespace UnoPaginateTester.Presentation;

public partial class MainViewModel : ObservableObject
{
    public partial class DisplayItem : ObservableObject
    {
        public string Description { get; set; }

    }

    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private List<DisplayItem> dataItems = new List<DisplayItem>();

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        GoToSecond = new AsyncRelayCommand(GoToSecondView);

        DataItems = GenerateData();
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    }

    public IListFeed<DisplayItem> DataAuto =>
        ListFeed.AsyncPaginated<DisplayItem>(async (PageRequest pageRequest, CancellationToken ct) =>
            await GetDataAsync(pageSize: pageRequest.DesiredSize ?? 5, firstItemIndex: pageRequest.CurrentCount, ct));

    public async ValueTask<IImmutableList<DisplayItem>> GetDataAsync(uint pageSize, uint firstItemIndex, CancellationToken ct)
    {
        // convert to int for use with LINQ
        var (size, count) = ((int)pageSize, (int)firstItemIndex);

        // fake delay to simulate loading data
        await Task.Delay(TimeSpan.FromSeconds(1), ct);

        // this is where we would asynchronously load actual data from a remote data store
        var people = GenerateData();

        return people
            .Skip(count)
            .Take(size)
            .ToImmutableList();
    }

    List<DisplayItem> GenerateData()
    {
        var items = new List<DisplayItem>();
        for (var i = 0; i < 100; i++)
        {
            items.Add(new DisplayItem { Description = $"Item{i}" });

        }
        return items;
    }

}
