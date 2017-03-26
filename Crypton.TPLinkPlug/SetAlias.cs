using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    public class SetAlias : IPlugCommand
    {

        public string Alias
        {
            get;
            set;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(new
            {
                system = new
                {
                    set_dev_alias = new
                    {
                        alias = Alias ?? string.Empty
                    }
                }
            });
        }
    }
}
