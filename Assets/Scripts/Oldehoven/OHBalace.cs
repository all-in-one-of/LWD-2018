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

/// <summary>
/// </summary>
public class OHBalace : MonoBehaviour
{
    /// <summary>
    ///     The force that is aplied to the oldehove
    /// </summary>
    private float _force;

    /// <summary>
    ///     The rigidbody
    /// </summary>
    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        _force += Input.GetAxis("Mouse X");
        if (Input.GetMouseButtonDown(0))
            _rigidbody.isKinematic = !_rigidbody.isKinematic;
#else
        _force = Input.acceleration.x * 50;
#endif
        _rigidbody.AddTorque(new Vector3(0, 0, -_force)*500);
    }
}