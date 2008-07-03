﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace cslalighttest.Engine
{
  public class AsyncAsserter
  {
    public event AsyncAssertComplete Complete;
    private void OnComplete(Exception exception)
    {
      OnComplete(TestResult.Fail, exception);
    }
    private void OnComplete(TestResult status)
    {
      OnComplete(status, null);
    }
    protected virtual void OnComplete(TestResult status, Exception exception)
    {
      if (Complete != null)
        Complete(status, exception);
    }

    public virtual void Success()
    {
      OnComplete(TestResult.Success);
    }

    public virtual void Indeterminate()
    {
      OnComplete(TestResult.Indeterminate);
    }

    internal void Fail()
    {
      OnComplete(TestResult.Fail);
    }

    public void IsNotNull(object actual)
    {
      if (actual == null)
        OnComplete(new TestException("Assert.IsNotNull failed."));
    }

    public void IsNull(object actual)
    {
      if (actual != null)
        OnComplete(new TestException("Assert.IsNull failed."));
    }

    public void AreEqual(object expected, object actual)
    {
      if (!object.Equals(expected, actual))
      {
        string act;
        if (actual != null)
          act = actual.ToString();
        else
          act = "<null>";
        if (string.IsNullOrEmpty(act))
          act = "string.Empty";
        OnComplete(new TestException(string.Format("Assert.AreEqual failed; expected '{0}', was '{1}'.", expected.ToString(), act)));
      }
    }

    public void IsFalse(bool actual)
    {
      if (actual)
        OnComplete(new TestException("Assert.IsFalse failed."));
    }

    public void IsTrue(bool actual)
    {
      if (!actual)
        OnComplete(new TestException("Assert.IsTrue failed."));
    }

    public void Try(Action action)
    {
      try
      {
        action.Invoke();
      }
      catch (Exception ex)
      {
        OnComplete(ex);
      }
    }
  }

  public delegate void AsyncAssertComplete(TestResult status, Exception exception);
}
