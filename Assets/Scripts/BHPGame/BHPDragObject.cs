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

public class BHPDragObject : MonoBehaviour
{
    private float _dist;
    private Vector3 _offset;
    private Transform _prisonreTransform;
    private BHPPrisonerAI _bhpPrisonerAi;
    public bool Dragging;

    private void Update()
    {

#if UNITY_EDITOR
        checkTouch(default(Touch));
#else
        if (Input.touchCount != 1)
        {
            Dragging = false;
            return;
        }

        foreach (var touch in Input.touches)
        {
            checkTouch(touch);
        }
   
#endif //UNITY_EDITOR
    }

    void checkTouch(Touch touch)
    {
        Vector3 v3;

#if UNITY_EDITOR
        Vector3 pos = Input.mousePosition;
#else
        Vector3 pos = touch.position;
#endif //UNITY_EDITOR

        if (BHPInput.getState(touch, BHPInput.InputState.Began))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit))
            {
                _bhpPrisonerAi = hit.collider.GetComponent<BHPPrisonerAI>();

                if (_bhpPrisonerAi == null) return;

                _prisonreTransform = hit.transform;
                _bhpPrisonerAi.Rigidbody.isKinematic = true;
                _bhpPrisonerAi.Free = false;
                _dist = hit.transform.position.z - Camera.main.transform.position.z;
                v3 = new Vector3(pos.x, pos.y, _dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                _offset = _prisonreTransform.position - v3;
                Dragging = true;
            }
        }
        if (Dragging && BHPInput.getState(touch, BHPInput.InputState.Moving))
        {
            v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _dist);
            v3 = Camera.main.ScreenToWorldPoint(v3);
            _prisonreTransform.position = v3 + _offset;
            _bhpPrisonerAi.Free = false;
        }
        if (Dragging && (BHPInput.getState(touch, BHPInput.InputState.Ended) || BHPInput.getState(touch, BHPInput.InputState.Canceled)))
        {
            _prisonreTransform.GetComponent<Rigidbody>().isKinematic = false;
            Dragging = false;
        }
    }
}