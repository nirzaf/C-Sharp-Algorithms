﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures.Trees;

public class TernaryTreeNode
{
    public virtual TernaryTreeNode Parent { get; private set; }
    protected virtual TernaryTreeNode[] childs { get; set; }

    public virtual TernaryTreeNode GetLeftChild => childs[0];
    public virtual TernaryTreeNode GetMiddleChild => childs[1];
    public virtual TernaryTreeNode GetRightChild => childs[2];

    public virtual bool FinalLetter { get; set; }
    public virtual char Value { get; set; }

    public TernaryTreeNode(TernaryTreeNode parent)
    {
        Parent = parent;
        childs = new TernaryTreeNode[3];
    }

    public TernaryTreeNode(TernaryTreeNode parent, char value, bool isFinal)
    {
        Parent = parent;
        Value = value;
        FinalLetter = isFinal;
        childs = new TernaryTreeNode[3];
    }

    public virtual TernaryTreeNode AddLeftChild(char value, bool isFinal)
    {
        childs[0] = new TernaryTreeNode(this, value, isFinal);
        return childs[0];
    }
    public virtual TernaryTreeNode AddRightChild(char value, bool isFinal)
    {
        childs[2] = new TernaryTreeNode(this, value, isFinal);
        return childs[2];
    }
    public virtual TernaryTreeNode AddMiddleChild(char value, bool isFinal)
    {
        childs[1] = new TernaryTreeNode(this, value, isFinal);
        return childs[1];
    }
}