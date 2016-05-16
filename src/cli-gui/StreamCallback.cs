﻿using System;

namespace Xt {

    abstract class StreamCallback {

        int processed;
        protected string name;
        protected readonly Action<string> onError;
        protected readonly Action<string> onMessage;

        internal StreamCallback(string name, Action<string> onError, Action<string> onMessage) {
            this.name = name;
            this.onError = onError;
            this.onMessage = onMessage;
        }

        internal abstract void OnCallback(XtFormat format, bool interleaved, Array input, Array output, int frames);

        internal virtual void OnMessage(string message) {
            onMessage.Invoke(message);
        }

        internal void OnCallback(XtStream stream, Array input, Array output, int frames,
            double time, ulong position, bool timeValid, ulong error, object user) {

            if (error != 0) {
                onError("Stream callback error: " + XtPrint.ErrorToString(error));
                return;
            }

            XtFormat format = stream.GetFormat();
            bool interleaved = stream.IsInterleaved();
            OnCallback(format, interleaved, input, output, frames);
            processed += frames;
            if (processed < format.mix.rate * 3)
                return;

            processed = 0;
            XtLatency latency = stream.GetLatency();
            string formatString = "Stream {1}:{0}\tinput latency:{2}{0}\toutput latency:{3}{0}\t" +
                "buffer frames:{4}{0}\ttime:{5}{0}\tposition:{6}{0}\ttimeValid:{7}{0}\tuser:{8}.";
            OnMessage(string.Format(formatString, Environment.NewLine, name, latency.input,
                latency.output, stream.GetFrames(), time, position, timeValid, user));
        }
    }
}
