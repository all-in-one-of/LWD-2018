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
///     spawns the prisoners
/// </summary>
public class BHPSpawner : MonoBehaviour
{
    private Animator _animator;

    /// <summary>
    ///     The object pool used for pooling the prisoners
    /// </summary>
    private BHPObjectPool _objectPool;

    private float _timer;
    private int spawns;

    public void Awake()
    {
        _objectPool = GameObject.Find("ObjectPool").GetComponent<BHPObjectPool>();
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
            }
            else
            {
                var o =
                    Instantiate(Resources.Load("Inmate"), transform.position + new Vector3(0.5f, -0.5f),
                        Quaternion.identity) as GameObject;
                _objectPool.Active.Add(o);
                o.transform.name = Random.Range(1000, 9999) + "";
                o.transform.SetParent(_objectPool.transform);
            }
        }
    }
}