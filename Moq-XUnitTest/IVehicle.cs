using System;
using System.Collections.Generic;
using System.Text;

namespace Moq_XUnitTest
{
    public interface IVehicle
    {
        ///<summary>
        ///Honks the vehicle's horn.
        ///</summary>
        ///<returns>
        ///True if the action was successful.
        ///</returns>
        bool HonkHorn();

        ///<summary>
        ///Applies the vehicle's brakes.
        ///</summary>
        ///<returns>
        ///True if the action was successful.
        ///</returns>
        bool ApplyBrakes();
    }
}
