package com.xtaudio.xt;

public class PrintSimple {

    public static void main(String[] args) {

        try (XtAudio audio = new XtAudio(null, null, null, null)) {
            for (int s = 0; s < XtAudio.getServiceCount(); s++) {
                XtService service = XtAudio.getServiceByIndex(s);
                for (int d = 0; d < service.getDeviceCount(); d++)
                        System.out.println(service.getName() + ": " + service.getDeviceName(d));
            }
        }
    }
}
