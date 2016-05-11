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


using Lw18;
using UnityEngine;

namespace BHPGame
{
    /// <summary>
    ///     Class for the draging behavior for prisoners
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class DragPrisoner : MonoBehaviour
    {
        /// <summary>
        ///     The distance between the prisoner and the camara
        /// </summary>
        private float _dist;

        /// <summary>
        ///     The object pool for the prisoners
        /// </summary>
        private ObjectPool _objectPool;

        /// <summary>
        ///     The offset from the center to the curent dragpoint
        /// </summary>
        private Vector3 _offset;

        /// <summary>
        ///     The AI from the selected prisoner
        /// </summary>
        private PrisonerAi _prisonerAi;

        /// <summary>
        ///     The transform from the selected prisoner
        /// </summary>
        private Transform _prisonerTransform;

        /// <summary>
        ///     True if the user is currently dragging a prisoner
        /// </summary>
        [HideInInspector] public bool Dragging;

        public void Start()
        {
            _objectPool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            CheckTouch(default(Touch));
#else
        if (Input.touchCount != 1)
        {
            Dragging = false;
            return;
        }
            CheckTouch(Input.touches[0]); 
#endif //UNITY_EDITOR
        }

        private void CheckTouch(Touch touch)
        {
            Vector3 v3;

#if UNITY_EDITOR
            var pos = Input.mousePosition;
#else
            var pos = touch.position;
#endif //UNITY_EDITOR

            if (EditorInput.GetState(touch, EditorInput.InputState.Began))
            {
                Dragging = false;
                _prisonerTransform = null;

                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(pos);

                if (Physics.Raycast(ray, out hit))
                {
                    _prisonerAi = hit.collider.GetComponent<PrisonerAi>();

                    if (_prisonerAi == null) return;

                    _prisonerTransform = hit.transform;
                    _prisonerAi.Rigidbody.isKinematic = true;
                    _dist = hit.transform.position.z - Camera.main.transform.position.z;
                    v3 = new Vector3(pos.x, pos.y, _dist);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    _offset = _prisonerTransform.position - v3;
                    Dragging = true;
                }
            }
            if (Dragging && EditorInput.GetState(touch, EditorInput.InputState.Moving))
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                if (_prisonerTransform != null) _prisonerTransform.position = v3 + _offset;
            }
            if (Dragging &&
                (EditorInput.GetState(touch, EditorInput.InputState.Ended) ||
                 EditorInput.GetState(touch, EditorInput.InputState.Canceled)))
            {
                Dragging = false;
                _objectPool.SetAllFree();
            }
        }
    }
}