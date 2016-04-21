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

using System.Collections;
using UnityEngine;

namespace Oldehoven
{
    /// <summary>
    ///     The AI for the toeristen;
    /// </summary>
    public class ToeristAi : MonoBehaviour
    {
        private Animator _animator;

        /// <summary>
        ///     The capsule for debuging the position that the tourist moves to.
        /// </summary>
        private GameObject _debug;

        /// <summary>
        ///     The direction that the tourist walks. Can only be -1 or 1.
        /// </summary>
        private int _dir;

        /// <summary>
        ///     True if the tourist is standing still.
        /// </summary>
        private bool _isIdle;

        private GameObject _mainMesh;
        private GameObject _ohCenter;

        /// <summary>
        ///     The parachute.
        /// </summary>
        private GameObject _parachute;

        /// <summary>
        ///     True if the parachute is deployed
        /// </summary>
        private bool _parachuteIsDeployed;

        /// <summary>
        ///     The rope from the parachute.
        /// </summary>
        private LineRenderer _paraRope;

        private float _pos;
        private Rigidbody _rigidbody;
        private Rigidbody _rigidbodyParachute;

        /// <summary>
        ///     The scorecounter.
        /// </summary>
        private Score _score;

        private int _speedModifier = 25;

        /// <summary>
        ///     The layermask for checking if the tourist is still on the oldehove
        /// </summary>
        public LayerMask TopCoLayerMask;

        public void Awake()
        {
            _mainMesh = transform.Find("Mesh").gameObject;
            _parachute = transform.Find("Parachute").gameObject;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbodyParachute = _parachute.GetComponent<Rigidbody>();
            _ohCenter = GameObject.Find("OHCenter");
            _animator = _mainMesh.GetComponent<Animator>();

            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            var mat = new Material(smr.material)
            {
                color = new Color(Random.value, Random.value, Random.value, 0.2f)
            };
            smr.material = mat;
            _parachute.GetComponent<MeshRenderer>().material = mat;

            _paraRope = _parachute.AddComponent<LineRenderer>();
            _paraRope.SetVertexCount(3);
            _paraRope.SetWidth(0.1f, 0.1f);
            _paraRope.material = Resources.Load<Material>("paraRopeMat");

            _parachute.SetActive(false);


            var rot = Random.value > 0.5f ? 90 : -90;
            _mainMesh.transform.eulerAngles = new Vector3(0, rot);

            _score = GameObject.Find("OHScore").GetComponent<Score>();

#if DEBUG
            _debug = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _debug.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            DestroyImmediate(_debug.GetComponent<CapsuleCollider>());
            _debug.GetComponent<MeshRenderer>().material = mat;
            _debug.transform.parent = transform;
#endif
        }

        /// <summary>
        ///     Updates the model and animation.
        /// </summary>
        private IEnumerator UpdateModel()
        {
            if (_parachuteIsDeployed) yield break;

            _isIdle = true;
            _animator.SetBool("IsIdle", _isIdle);

            yield return new WaitForSeconds(Random.Range(2, 4));

            if (_parachuteIsDeployed) yield break;

            _pos = Random.Range(-3, 3);
            var di = _ohCenter.transform.position.x + _pos - transform.position.x;
            _dir = di > 0 ? 1 : -1;
            var rot = di > 0 ? 90 : -90;
            _mainMesh.transform.eulerAngles = new Vector3(0, rot);
            _isIdle = false;
            _animator.SetBool("IsIdle", _isIdle);
            _speedModifier++;
        }

        private void Update()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, 50f, TopCoLayerMask) && !_parachuteIsDeployed)
            {
                _parachuteIsDeployed = true;
                _parachute.SetActive(true);
                _parachute.transform.parent = null;
                _animator.SetBool("IsIdle", true);
                _dir = transform.position.x > 0 ? 1 : -1;
                _score.TouristHasFallen();
            }

            if (_parachuteIsDeployed)
            {
                _paraRope.SetPosition(0,
                    _parachute.transform.position + _parachute.transform.InverseTransformVector(Vector3.left));
                _paraRope.SetPosition(2,
                    _parachute.transform.position + _parachute.transform.InverseTransformVector(Vector3.right));
                _paraRope.SetPosition(1, transform.position);

                _dir = transform.position.x > 0 ? 1 : -1;
                if (_rigidbodyParachute.angularVelocity.magnitude < 1)
                    _rigidbodyParachute.AddForce(new Vector3(_dir, 0)*_speedModifier);
            }

            if (!_isIdle && !_parachuteIsDeployed)
            {
                if (_rigidbody.angularVelocity.magnitude < 1)
                    _rigidbody.AddForce(new Vector3(_dir, 0)*_speedModifier);

                var dis = Mathf.Abs(_ohCenter.transform.position.x + _pos - transform.position.x);
                if (dis < 0.2)
                {
                    StartCoroutine(UpdateModel());
                }
#if DEBUG
                var tmp = new Vector3(_ohCenter.transform.position.x + _pos, transform.position.y + 2,
                    transform.position.z);
                _debug.transform.position = tmp;
#endif
            }
        }
    }
}