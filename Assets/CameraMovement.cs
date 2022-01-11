using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform _playerTransform;

    void Start()
    {
        _playerTransform = FindObjectOfType<Player>().transform;
    }

    void LateUpdate()
    {
        MoveTowardPlayer();
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, 2, Mathf.Infinity);
        transform.position = pos;
    }

    void MoveTowardPlayer()
    {
        transform.position = new Vector3(_playerTransform.transform.position.x, _playerTransform.transform.position.y, -10);
        /*Vector3 desiredPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, -10);
        Vector3 smoothPosition = Vector3.MoveTowards(transform.position, desiredPosition, 1 * Time.time);
        transform.position = smoothPosition;*/

        //ClampPosition();
    }
}
