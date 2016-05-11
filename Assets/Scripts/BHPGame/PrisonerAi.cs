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

using UnityEngine;

namespace BHPGame
{
    public class PrisonerAi : MonoBehaviour
    {
        public LayerMask LayerMask;

        [HideInInspector] public bool Pooled;

        [HideInInspector] public Rigidbody Rigidbody;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            if (Pooled) return;

            if (Physics.Raycast(transform.position, Vector2.down, 1, LayerMask) &&
                Rigidbody.velocity.magnitude < 2)
                Rigidbody.AddForce(transform.position.x > 0 ? 100 : -100, 0, 0);
        }
    }
}