using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class BuildScript
    {
        public bool IsBuild
        {
            get;
            private set;
        }
        public Exception Error
        {
            get;
            private set;
        }
        public ILModule modIL
        {
            get;
            private set;
        }
        public ModuleConverter converterIL
        {
            get;
            private set;
        }
        public byte[] finalAVM
        {
            get;
            private set;
        }

        public BuildScript()
        {
        }
        public void Build(Stream fs, Stream fspdb)
        {
            this.IsBuild = false;
            this.Error = null;

            var log = new DefLogger();
            this.modIL = new ILModule(log);
            try
            {
                modIL.LoadModule(fs, fspdb);
            }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                this.Error = err;
                return;
            }

            converterIL = new ModuleConverter(log);
            ConvOption option = new ConvOption();
            //try
            {
                converterIL.Convert(modIL, option);
                finalAVM = converterIL.outModule.Build();
                IsBuild = true;
            }
            //catch (Exception err)
            //{
            //    this.Error = err;
            //    log.Log("Convert IL->ASM Error:" + err.ToString());
            //    return;
            //}
        }
    }
}
