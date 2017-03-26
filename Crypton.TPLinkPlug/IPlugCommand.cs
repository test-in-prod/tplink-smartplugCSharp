using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypton.TPLinkPlug
{
    /// <summary>
    /// Defines an interface for a SmartPlug command
    /// </summary>
    public interface IPlugCommand
    {
        /// <summary>
        /// Returns JSON command
        /// </summary>
        /// <returns></returns>
        string GetJson();
    }
}
