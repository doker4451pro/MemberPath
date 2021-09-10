using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberPathAttribute : PropertyAttribute
{
    public Type type;

    public MemberPathAttribute(Type type)
    {
        this.type = type;
    }
}
