namespace CountdownProject;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell(); // Loads the navigation shell
    }
}
