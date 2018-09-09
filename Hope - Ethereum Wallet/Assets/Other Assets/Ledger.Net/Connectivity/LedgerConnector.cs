using Hid.Net.Unity;
using HidLibrary;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Net.Connectivity
{
    public static class LedgerConnector
    {
        private static readonly VendorProductIds[] WellKnownLedgerWallets = new VendorProductIds[] { new VendorProductIds(0x2c97), new VendorProductIds(0x2581, 0x3b7c) };
        private static readonly UsageSpecification[] UsageSpecification = new[] { new UsageSpecification(0xffa0, 0x01) };

        public static LedgerManager GetWindowsConnectedLedger()
        {
            List<HidDevice> devices = new List<HidDevice>();
            foreach (var ids in WellKnownLedgerWallets)
            {
                if (ids.ProductId == null)
                    devices.AddRange(HidDevices.Enumerate(ids.VendorId));
                else
                    devices.AddRange(HidDevices.Enumerate(ids.VendorId, ids.ProductId.Value));

            }
            var hidDevices = devices.Where(d => UsageSpecification == null
                || UsageSpecification.Length == 0
                || UsageSpecification.Any(u => (ushort)d.Capabilities.UsagePage == u.UsagePage && (ushort)d.Capabilities.Usage == u.Usage));

            return !hidDevices.Any() ? null : new LedgerManager(new UnityHIDDevice(hidDevices.First()));
        }
    }
}
