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
    public class Delete : MonoBehaviour
    {
        private DragPrisoner _dragPrisoner;
        private ObjectPool _objectPool;

        public bool HasEscaped;

        public void Awake()
        {
            _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
            _dragPrisoner = GameObject.Find("DragObject").GetComponent<DragPrisoner>();
        }

        public void OnTriggerEnter(Collider other)
        {
            _dragPrisoner.Dragging = false;
            var o = other.gameObject;
            o.GetComponent<Rigidbody>().isKinematic = true;
            o.transform.position = new Vector3(Random.Range(-10, 10), -20);
            _objectPool.InActive.Add(o);
            ScoreCounter.UpdateScore(HasEscaped);
            o.transform.GetComponent<PrisonerAi>().Pooled = true;
            _objectPool.Active.Remove(o);
        }
    }
}