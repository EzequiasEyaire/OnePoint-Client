﻿using System;
using RtspClientSharp;

namespace Client.GUI.Models
{
    interface IMainWindowModel
    {
        event EventHandler<string> StatusChanged;

        IVideoSource VideoSource { get; }

        void Start(ConnectionParameters connectionParameters);
        void Stop();
    }
}