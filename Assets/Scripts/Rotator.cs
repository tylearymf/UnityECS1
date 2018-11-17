using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Reflection;
using System.ComponentModel;

/// <summary>
/// ECS中的Componet，Unity官方也称其为ComponentData (内部只有纯数据，没有逻辑)
/// </summary>
public class Rotator : MonoBehaviour
{
    public float Speed = 100;
}

/*
 *      Unity ECS大致逻辑
 *      //找到相应的组件，执行对应的逻辑
        //1、Unity用反射拿到这个RotateStruct里面的所有字段，记住，只会拿字段，所以你在里面定义了其他成员是没用的。
        //2、然后拿到的这些字段做一些过滤
        //3、过滤条件1：字段类型如果是指针类型
        //              1.1：其指针类型是否实现IComponentData接口
        //              1.2：其指针类型是Entity
        //4、过滤条件2：字段类型如果不是指针类型
        //              2.1：其类型是不泛型时，其类型要继承自UnityEngine.Component
        //              2.2：其类型是泛型时，必须使用该类型的泛型：SubtractiveComponent<T>，且T必须满足2.1条件
        //5、Unity拿到这些Component之后，会查找所有Entity，找到包含这些Component的Entity，并返回 
 * 
 */


/// <summary>
/// System （ECS中的系统，内部实现具体逻辑）
/// </summary>
public class RotateSystem1 : ComponentSystem
{
    /// <summary>
    /// Components （组件的集合，也就是一堆Component）
    /// 注意：一个组件集合里面最多包含6个组件，这个是Unity规定死的，超出时会报错
    /// </summary>
    public struct RotateStruct
    {
        public Transform Transform;
        public Rotator Rotator;
    }

    protected override void OnUpdate()
    {
        var time = Time.deltaTime;
        foreach (var item in GetEntities<RotateStruct>())
        {
            item.Transform.Rotate(Vector3.up, item.Rotator.Speed * time);
        }
    }
}

/// <summary>
/// System2 第二种，使用注入
/// Inject特性的字段会在CreateManager之前附上值
/// </summary>
public class RotateSystem2 : ComponentSystem
{
    public struct RotateData
    {
        public ComponentArray<Rotator> Rotators;
        public ComponentArray<Transform> Transforms;
        public readonly int Length;
    }
    [Inject]
    RotateData mRotateData;

    protected override void OnUpdate()
    {
        var time = Time.deltaTime;
        for (int i = 0; i < mRotateData.Length; i++)
        {
            mRotateData.Transforms[i].Rotate(Vector3.up, mRotateData.Rotators[i].Speed * time);
        }
    }
}
