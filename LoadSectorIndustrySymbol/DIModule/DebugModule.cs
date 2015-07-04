using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Contracts;
using Logger;
using Ninject.Modules;

namespace LoadSectorIndustrySymbol.DIModule
{
    class DebugModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Log4NetLogger>().InSingletonScope()
                .WithConstructorArgument("loglevel", LogLevelEnum.Debug);

            //Bind<IEtfService>().To<EtfService>().InSingletonScope();

            //Bind<IOptionService>().To<OptionService>().InSingletonScope();
        }
    }
}
