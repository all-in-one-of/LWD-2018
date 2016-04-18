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

using System.Collections.Generic;
using UnityEngine;

public class BHPObjectPool : MonoBehaviour
{
    /// <summary>
    ///     The list of active inmates
    /// </summary>
    public List<GameObject> Active;

    /// <summary>
    ///     The list of inactive inmates(are placed outside view)
    /// </summary>
    public List<GameObject> InActive;

    /// <summary>
    ///     Amount of time spawned
    /// </summary>
    public int Spawns;
}