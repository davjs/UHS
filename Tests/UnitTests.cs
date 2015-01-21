﻿using Cycles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {

        [TestMethod]
        public void TestMethod1()
        {
            TestingFramework.test("test");
        }

        [TestMethod]
        public void TestEnum()
        {
            TestingFramework.test("enum");
        }

        [TestMethod]
        public void TestMess()
        {
            TestingFramework.test("mess");
        }

        [TestMethod]
        public void NamespaceTest()
        {
            TestingFramework.test("namespace");
        }

        [TestMethod]
        public void MainTest()
        {
            TestingFramework.test("main");
        }

        [TestMethod]
        public void Classtest()
        {
            TestingFramework.test("classes");
        }

        [TestMethod]
        public void Structs()
        {
            TestingFramework.test("structs");
        }

        [TestMethod]
        public void Keywords()
        {
            TestingFramework.test("keywords");
        }

        [TestMethod]
        public void Templates()
        {
            TestingFramework.test("templates");
        }

        [TestMethod]
        public void BraceMacro()
        {
            TestingFramework.test("bracemacro");
        }

        [TestMethod]
        public void TestInclude()
        {
            TestingFramework.test("include");
        }
    }
}