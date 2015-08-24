using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenSoftware.EjectABed.BedControl
{
    //Yellow - Signal
    // Black (-) Ground
    // Red (+) 5 Volts

    public class Servo : IBedCommand
    {
        public void Eject()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Sleep()
        {
            throw new NotImplementedException();
        }
    }
}
