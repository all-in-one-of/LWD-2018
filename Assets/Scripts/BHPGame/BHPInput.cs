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

public class BHPInput
{
    public static bool getState(Touch touch, InputState state)
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
                throw new ArgumentOutOfRangeException("touchPhase", state, null);
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

    public enum InputState
    {
        Began,
        Moving,
        Ended,
        Canceled
    }

}