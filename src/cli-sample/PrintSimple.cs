using System;

namespace Xt {

    public class PrintSimple {

        public static void Main(string[] args) {

            using (XtAudio audio = new XtAudio(null, IntPtr.Zero, null, null)) {
                for (int s = 0; s < XtAudio.GetServiceCount(); s++) {
                    XtService service = XtAudio.GetServiceByIndex(s);
                    for (int d = 0; d < service.GetDeviceCount(); d++)
                            Console.WriteLine(service.GetName() + ": " + service.GetDeviceName(d));
                }
            }
        }
    }
}
