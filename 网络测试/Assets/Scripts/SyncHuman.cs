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
    //����ʱ���õĺ��������ó��򡢲��Ź�������
    public void SyncAttack(float eulY)
    {
        transform.eulerAngles = new Vector3(0, eulY, 0);
        Attack();
    }
}