using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Model_Controller
{

    /// <summary>
    /// Class to control the state of the Arduino
    /// </summary>
    class StateManager
    {
       
        public bool Thief { get; set; }
        public bool NEW { get; set; }

        public bool CONNECTION { get; set; }
        public bool THIEFDEC { get; set; }
        public bool COUNTING { get; set; }
      
        public void Initialize()
        {
            Thief = false;
          NEW=false;
          CONNECTION = false;
          THIEFDEC = false;
          COUNTING = false;

        }
    }
}
