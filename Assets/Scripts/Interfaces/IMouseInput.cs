﻿using System;
using UnityEngine;


namespace DroneBase.Interfaces
{
    public interface IMouseInput
    {
        public event Action<Vector3> LeftMouseButtonClickPoint;
        public event Action<Vector3> RightMouseButtonClickPoint;

        float Scroll { get; }
        Vector3 GetMousePosition();
    }
}