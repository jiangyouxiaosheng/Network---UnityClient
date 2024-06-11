using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHuman : BaseHuman
{

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        //左键移动
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            //提前把可移动的地面的tag设置为Terrain
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                //发送Move协议给服务器端，以此来更新其他客户端的位置信息
                string sendStr = "Move|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                NetManager.Send(sendStr);
            }
        }
        //右键攻击
        if (Input.GetMouseButtonDown(1))
        {
            if (isAttacking) return;
            if (isMoving) return;
            //获得右击的方向，并转向
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            transform.LookAt(hit.point);
            Attack();
            //攻击时给服务器发送attack协议，然后其他客户端才会显示攻击的动画等。
            string sendStr = "Attack|";
            sendStr += NetManager.GetDesc() + ",";
            sendStr += transform.eulerAngles.y + ",";
            NetManager.Send(sendStr);

            //攻击判定，发射一条线段判断是否触碰到其他玩家（SyncHuman），表示攻击到了此玩家
            Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20 * transform.forward;
            if (Physics.Linecast(lineStart, lineEnd, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == gameObject)
                    return;
                SyncHuman h = hitObj.GetComponent<SyncHuman>();
                if (h == null)
                    return;
                //如果攻击到玩家之后，给服务器发送hit协议，
                sendStr = "Hit|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += h.desc + ",";
                NetManager.Send(sendStr);
            }
        }
    }
}