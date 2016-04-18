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

public class BHPPrisonerAI : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public bool Free;
    public bool Pooled;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void LateUpdate()
    {
        if (Pooled) return;


        if (Free && Physics.Raycast(transform.position, Vector3.down, 10) && Rigidbody.velocity.x < 2)
            Rigidbody.AddForce(transform.position.x < 0 ? 1 : -1, 0, 0);

        Free = true;
    }

}