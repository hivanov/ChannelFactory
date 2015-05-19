using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    /// <summary>
    /// fdsfs
    /// </summary>
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class TestService : ITestService
    {
        #region ITestService Members

        /// <summary>
        /// Copies me.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public int CopyMe(int x)
        {
            return x;
        }

        #endregion
    }
}
