namespace _Net.Models;

public abstract class RepositoryBase
{
    protected readonly IConfiguration configuration;
    protected readonly string ConectionString;
    protected RepositoryBase(IConfiguration configuration)
    {
        this.configuration = configuration;
        ConectionString = configuration["ConnectionStrings:DefaultConnection"];
    }    
}
