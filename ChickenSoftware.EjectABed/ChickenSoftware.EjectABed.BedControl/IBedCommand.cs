using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSoftware.EjectABed.BedControl
{
    public interface IBedCommand
    {
        Boolean Initialize();
        void Eject();
        void Reset();
        void Sleep();

    }
}
