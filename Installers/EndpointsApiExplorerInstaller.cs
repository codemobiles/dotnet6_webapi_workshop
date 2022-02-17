namespace dotnet_hero.Installers
{
    public class EndpointsApiExplorerInstaller : IInstallers
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
         services.AddEndpointsApiExplorer();
        }
    }
}