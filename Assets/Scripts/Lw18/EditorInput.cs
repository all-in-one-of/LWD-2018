// This file is part of Leeuwarden-2018
// 
// Copyright (c) 2016 sietze greydanus
// 
// Leeuwarden-2018 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3, as
// published by the Free Software Foundation.
// 
// Leeuwarden-2018 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Leeuwarden-2018. If not, see <http://www.gnu.org/licenses/>.
// 


using System;
using UnityEngine;

namespace Lw18
{
    /// <summary>
    ///     Class for multiplatform input.
    /// </summary>
    public class EditorInput
    {
        /// <summary>
        ///     Behaves as TouchPhase on mobile.
        ///     Behaves as Input.GetMouse on PC.
        /// </summary>
        public enum InputState
        {
            /// <summary>
            ///     Same as TouchPhase.Began or Input.GetMouseButtonDown(0).
            /// </summary>
            Began,

            /// <summary>
            ///     Same as TouchPhase.Moved or Input.GetMouseButton(0).
            /// </summary>
            Moving,

            /// <summary>
            ///     Same as TouchPhase.Ended or Input.GetMouseButtonUp(0).
            /// </summary>
            Ended,

            /// <summary>
            ///     Same as TouchPhase.Canceled.
            ///     returns false by default on PC.
            /// </summary>
            Canceled
        }

        /// <summary>
        ///     Gets the current InputState.
        /// </summary>
        /// <param name="touch">The current touch.</param>
        /// <param name="state">The state to check against.</param>
        /// <returns>True if the InputState matches the current input</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">touch;null</exception>
        public static bool GetState(Touch touch, InputState state)
        {
#if UNITY_EDITOR
            switch (state)
            {
                case InputState.Began:
                    return Input.GetMouseButtonDown(0);
                case InputState.Moving:
                    return Input.GetMouseButton(0);
                case InputState.Ended:
                    return Input.GetMouseButtonUp(0);
                case InputState.Canceled:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("touch", state, null);
            }
#else
            switch (state)
            {
                case InputState.Began:
                    return touch.phase == TouchPhase.Began;
                case InputState.Moving:
                    return touch.phase == TouchPhase.Moved;
                case InputState.Ended:
                    return touch.phase == TouchPhase.Ended;
                case InputState.Canceled:
                    return touch.phase == TouchPhase.Canceled;
                default:
                    throw new ArgumentOutOfRangeException("touchPhase", state, null);
            }

#endif //UNITY_EDITOR
        }
    }
}