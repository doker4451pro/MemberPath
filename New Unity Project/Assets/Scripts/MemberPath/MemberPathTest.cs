using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MemberPathTest : MonoBehaviour
{
    [SerializeReference]
    [MemberPath(typeof(ClassA))]
    public MemberPath testMeberPath;
}
