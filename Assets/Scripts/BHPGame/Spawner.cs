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
using Random = UnityEngine.Random;

namespace BHPGame
{
    /// <summary>
    ///     spawns the prisoners
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        private Animator _animator;

        /// <summary>
        ///     The object pool used for the prisoners
        /// </summary>
        private ObjectPool _objectPool;

        private float _timer;

        public void Awake()
        {
            _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
            _animator = GetComponent<Animator>();
            _timer = Random.Range(0, 30);
        }

        private void Update()
        {
            _animator.SetBool("enabled", false);
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _objectPool.Spawns++;
                print(_objectPool.Spawns);
                _timer = Random.Range(5, 30);
                _animator.SetBool("enabled", true);
                if (_objectPool.InActive.Count > 0)
                {
                    var o = _objectPool.InActive[0];
                    o.transform.position = transform.position + new Vector3(0.5f, -0.5f);
                    o.GetComponent<Rigidbody>().isKinematic = false;
                    _objectPool.InActive.Remove(o);
                    o.transform.GetComponent<PrisonerAi>().Pooled = false;
                    _objectPool.Active.Add(o);
                }
                else
                {
                    var o =
                        Instantiate(Resources.Load("Inmate"), transform.position + new Vector3(0.5f, -0.5f),
                            Quaternion.identity) as GameObject;
                    _objectPool.Active.Add(o);
                    if (o == null) return;
                    o.transform.name = Guid.NewGuid().ToString();
                    o.transform.SetParent(_objectPool.transform);
                    o.transform.GetComponent<PrisonerAi>().Pooled = false;
                }
            }
        }
    }
}