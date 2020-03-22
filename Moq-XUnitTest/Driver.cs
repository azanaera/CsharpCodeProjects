using System;
using Moq_XUnitTest;

public class Driver
{
    private IVehicle vehicleToDrive;

    public Driver(IVehicle vehicleToDrive)
    {
        this.vehicleToDrive = vehicleToDrive;
    }

    public bool EvasiveManeuvers(bool alertOffendingDriver)
    {
        bool success = false;
        if (alertOffendingDriver)
            success = this.vehicleToDrive.ApplyBrakes() && this.vehicleToDrive.HonkHorn();
        else
            success = this.vehicleToDrive.ApplyBrakes();

        return success;
    }
}
