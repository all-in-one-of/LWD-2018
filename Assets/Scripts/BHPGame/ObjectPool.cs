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

namespace BHPGame
{
    public class ObjectPool : MonoBehaviour
    {
        /// <summary>
        ///     The list of active prisoners
        /// </summary>
        public List<GameObject> Active;

        /// <summary>
        ///     The list of inactive inmates(are placed outside view)
        /// </summary>
        public List<GameObject> InActive;

        /// <summary>
        ///     Amount of spawns
        /// </summary>
        public int Spawns;

        /// <summary>
        ///     Sets all active prisoners free.
        /// </summary>
        public void SetAllFree()
        {
            foreach (var o in Active)
            {
                o.GetComponent<PrisonerAi>().Rigidbody.isKinematic = false;
            }
        }
    }
}