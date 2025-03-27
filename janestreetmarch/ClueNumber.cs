using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasersNMirrors.Core
{
    public class ClueNumber
    {
        private bool constant = false;
        private UInt32 number = 0;

        public bool Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        public uint Number
        {
            get => number;
            set
            {
                if (constant) return;
                number = value;
            }
        }

        public ClueNumber () { }

        public ClueNumber(UInt32 number)
        {
            this.number = number;
            if (number != 0) this.constant = true;
        }

        public ClueNumber (bool constant, UInt32 number)
        {
            this.constant = constant;
            this.number = number;
        }

        
    }
}
