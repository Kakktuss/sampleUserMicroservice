using Autofac;
using UserApplication.EntityFrameworkDataAccess.Repositories;

namespace UserApplication
{
    public class RepositoryAutofacModule : Autofac.Module
    {
        private readonly string _connectionString;

        public RepositoryAutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // EF Core Configuration
            builder.RegisterType<CountryRepository>()
                .As<ICountryRepository>()
                .AsSelf();
            
            builder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .AsSelf();
            
            // Dapper Configuration
            builder.Register(c => new DapperDataAccess.Repositories.UserRepository(_connectionString))
                .As<DapperDataAccess.Repositories.IUserRepository>()
                .AsSelf();
        }
    }
}