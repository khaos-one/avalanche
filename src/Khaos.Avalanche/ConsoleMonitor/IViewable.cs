using Terminal.Gui;

namespace Khaos.Avalanche.ConsoleMonitor;

public interface IViewable
{
    View GetView();
    void BeforeRefresh();
}