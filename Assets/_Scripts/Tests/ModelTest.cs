using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SampleTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ModelTestSimplePasses()
    {
        Assert.AreEqual(1, Flawless.Model.Test.x);
    }
}
