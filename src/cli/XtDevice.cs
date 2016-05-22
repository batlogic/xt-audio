using System;
using System.Runtime.InteropServices;

/* Copyright (C) 2015-2016 Sjoerd van Kreel.
 *
 * This file is part of XT-Audio.
 *
 * XT-Audio is free software: you can redistribute it and/or modify it under the 
 * terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * XT-Audio is distributed in the hope that it will be useful, but WITHOUT ANY 
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with XT-Audio. If not, see<http://www.gnu.org/licenses/>.
 */
namespace Xt {

    public sealed class XtDevice : IDisposable {

        internal IntPtr d;

        internal XtDevice(IntPtr d) {
            this.d = d;
        }

        public override string ToString() {
            return GetName();
        }

        public XtSystem GetSystem() {
            return XtNative.XtDeviceGetSystem(d);
        }

        public void ShowControlPanel() {
            XtNative.HandleError(XtNative.XtDeviceShowControlPanel(d));
        }

        public void Dispose() {
            if (d != IntPtr.Zero)
                XtNative.XtDeviceDestroy(d);
            d = IntPtr.Zero;
        }

        public string GetName() {
            IntPtr name;
            XtNative.HandleError(XtNative.XtDeviceGetName(d, out name));
            return XtNative.FreeStringFromUtf8(name);
        }

        public int GetChannelCount(bool output) {
            int count;
            XtNative.HandleError(XtNative.XtDeviceGetChannelCount(d, output, out count));
            return count;
        }

        public XtBuffer GetBuffer(XtFormat format) {
            XtBuffer buffer = new XtBuffer();
            XtNative.Format native = XtNative.Format.ToNative(format);
            XtNative.HandleError(XtNative.XtDeviceGetBuffer(d, ref native, buffer));
            return buffer;
        }

        public XtMix GetMix() {
            IntPtr mix;
            XtNative.HandleError(XtNative.XtDeviceGetMix(d, out mix));
            XtMix result = mix == IntPtr.Zero ? null : (XtMix)Marshal.PtrToStructure(mix, typeof(XtMix));
            XtNative.XtAudioFree(mix);
            return result;
        }

        public bool SupportsAccess(bool interleaved) {
            bool supports;
            XtNative.HandleError(XtNative.XtDeviceSupportsAccess(d, interleaved, out supports));
            return supports;
        }

        public bool SupportsFormat(XtFormat format) {
            bool supports;
            XtNative.Format native = XtNative.Format.ToNative(format);
            XtNative.HandleError(XtNative.XtDeviceSupportsFormat(d, ref native, out supports));
            return supports;
        }

        public string GetChannelName(bool output, int index) {
            IntPtr name;
            XtNative.HandleError(XtNative.XtDeviceGetChannelName(d, output, index, out name));
            return XtNative.FreeStringFromUtf8(name);
        }

        public XtStream OpenStream(XtFormat format, bool interleaved, bool raw, double bufferSize, XtStreamCallback callback, object user) {
            IntPtr s;
            XtStream stream = new XtStream(this, raw, callback, null, user);
            XtNative.Format native = XtNative.Format.ToNative(format);
            stream.win32StreamCallback = new XtNative.StreamCallbackWin32(stream.StreamCallback);
            stream.linuxStreamCallback = new XtNative.StreamCallbackLinux(stream.StreamCallback);
            Delegate cbDelegate = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? (Delegate)stream.win32StreamCallback 
                : stream.linuxStreamCallback;
            IntPtr cb = Marshal.GetFunctionPointerForDelegate(cbDelegate);
            XtNative.HandleError(XtNative.XtDeviceOpenStream(d, ref native, interleaved, bufferSize, cb, IntPtr.Zero, out s));
            stream.Init(s);
            return stream;
        }
    }
}
