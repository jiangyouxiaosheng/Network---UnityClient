using UnityEngine;

public class SyncHuman : BaseHuman
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
    }
    //攻击时调用的函数，设置朝向、播放攻击动画
    public void SyncAttack(float eulY)
    {
        transform.eulerAngles = new Vector3(0, eulY, 0);
        Attack();
    }
}