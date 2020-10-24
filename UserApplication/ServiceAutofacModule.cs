using Autofac;
using UserApplication.Services;

namespace UserApplication
{
    public class ServiceAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>()
                .As<IUserService>()
                .AsSelf();
        }
    }
}