using Terminal.Gui;

namespace Khaos.Avalanche.ConsoleMonitor;

public class MonitorPane : IDisposable
{
    private readonly Toplevel _top;
    private readonly Window _window;

    private Pos _previousY = -1;
    private List<IViewable> _components = new();

    private Thread? _guiThread;
    private Timer? _refreshTimer;

    public MonitorPane()
    {
        Application.Init();
        _top = Application.Top;

        _window = new Window("Monitor")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        _top.Add(_window);
    }

    public void Add(params IViewable[] viewables)
    {
        foreach (var viewable in viewables)
        {
            var view = viewable.GetView();

            view.X = 0;
            view.Y = _previousY + 1;

            _previousY = view.Y;
            _window.Add(view);
            _components.Add(viewable);
        }
    }

    public void Begin()
    {
        if (_guiThread is not null)
        {
            throw new InvalidOperationException();
        }

        _guiThread = new Thread(() => Application.Run());
        _guiThread.Start();

        _refreshTimer = new Timer(
            _ =>
            {
                _components.ForEach(c => c.BeforeRefresh());
                Application.Refresh();
            },
            null,
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        if (_refreshTimer is not null)
        {
            _refreshTimer!.Dispose();
        }

        Application.Shutdown();
    }
}