using Hid.Net.Unity;
using HidLibrary;
using System.Collections.Generic;
using Trezor.Net;

public static class TrezorConnector
{
    private const int USAGE_PAGE = -256;
    private const int USAGE = 1;

    public static TrezorManager GetWindowsConnectedTrezor(EnterPinArgs enterPinCallback)
    {
        List<HidDevice> devices = new List<HidDevice>();
        devices.AddRange(HidDevices.Enumerate(TrezorManager.TrezorVendorId, TrezorManager.TrezorProductId));

        var device = devices.Find(d => d.Capabilities.UsagePage == USAGE_PAGE && d.Capabilities.Usage == USAGE);

        return device == null ? null : new TrezorManager(enterPinCallback, new UnityHIDDevice(device));
    }
}