using Terminal.Gui;

namespace Khaos.Avalanche.ConsoleMonitor.SimpleCounter;

public class SimpleCounter : IViewable
{
    private readonly string _name;
    
    private uint _currentValue = 0;
    private View _view;

    public SimpleCounter(string name)
    {
        _name = name;
        _view = new Label(0, 0, $"{_name}: {_currentValue}");
    }

    public View GetView()
    {
        return _view;
    }

    public void BeforeRefresh()
    {
        _view.Text = $"{_name}: {_currentValue}";
    }

    public void Increment()
    {
        _currentValue++;
    }
}